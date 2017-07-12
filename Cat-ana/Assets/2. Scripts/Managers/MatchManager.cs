using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.RT;
using System;

public class MatchManager : MonoBehaviour
{
    private GameManager game_manager = null;
    private GameSparksRTUnity RT_manager = null;
    private NetworkManager net_manager = null;

    // UI
    [SerializeField]
    private Text opp1, opp2, opp3, player;

    [SerializeField]
    private Text timer, turn_type_text;

    [SerializeField]
    private Text latency_text;

    [SerializeField]
    private Button attack_button;

    [SerializeField]
    private Text state_text;

    [SerializeField]
    private Text client_life_text;

    [SerializeField]
    private Button finish_player_turn;

    [SerializeField]
    private Button card1, card2, card3;

    // Server
    private int server_delay, latency, round_trip; //All in ms
    private int opponent_count = 0;

    // Map
    private NavigationMap nav_map = null;
    private int turn = 0;

    // Players
    [SerializeField]
    private GameObject player_prefab;
    private List<GameObject> players = new List<GameObject>();

    // Card
    string last_clicked_card = "";
    public Player target = null;

    // Turn
    public action curr_action = action.wait;
    private TurnInfo turn_info = new TurnInfo();
    int players_ready_perform_actions = 0;

    // Use this for initialization
    void Start()
    {
        game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        RT_manager = game_manager.GetGameSparksRTManager();
        net_manager = game_manager.GetNetworkManager();
        nav_map = GameObject.FindGameObjectWithTag("Map").GetComponent<NavigationMap>();

        StartCoroutine(SendTimeStamp());

        // Inform the server that the match is ready
        using (RTData data = RTData.Get())
        {
            data.SetLong(1, 0);
            RT_manager.SendData(103, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data, new int[] { 0 }); // send to peerId -> 0, which is the server
        }

        StartCoroutine(DelayTurnStart());

        turn_info.turn = turn_type.stop;
        curr_action = action.wait;

        attack_button.gameObject.SetActive(false);
        finish_player_turn.gameObject.SetActive(false);
        state_text.gameObject.SetActive(false);

        card1.gameObject.SetActive(false);
        card2.gameObject.SetActive(false);
        card3.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch(curr_action)
        {
            case action.wait:
                break;
            case action.card_select_target:
                state_text.gameObject.SetActive(true);
                state_text.text = "Choose a target";

                if (turn_info.turn != turn_type.strategy)
                {
                    state_text.gameObject.SetActive(false);

                    curr_action = action.wait;
                }
                
                break;
            case action.card_target_selected:
                {
                    if (turn_info.turn != turn_type.strategy)
                    {
                        state_text.gameObject.SetActive(false);
                        break;
                    }

                    Player client = GetClientPlayer().GetComponent<Player>();
                    CardUseSend(last_clicked_card, (int)client.GetNavigationEntity().GetGridPos().x, (int)client.GetNavigationEntity().GetGridPos().y, client.GetInstanceID().ToString(), target.GetInstanceID().ToString());

                    state_text.gameObject.SetActive(false);

                    curr_action = action.wait;
                }
                break;
            case action.check_if_all_players_finished_moving:
                {
                    if (turn_info.turn == turn_type.action && players_ready_perform_actions == players.Count)
                    {
                        bool can_perform = true;

                        for (int i = 0; i < players.Count; i++)
                        {
                            Player player = players[i].GetComponent<Player>();

                            if (player.IsMoving())
                                can_perform = false;
                        }

                        if (can_perform)
                            curr_action = action.perform_attacks;
                    }
                }
                break;
            case action.perform_attacks:
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        Player player = players[i].GetComponent<Player>();

                        // Attack
                        if (player.GetAttack())
                        {
                            player.Attack();
                        }

                        // SetStealth
                        if (player.GetStealth())
                        {
                            if (player.GetVisible())
                                player.GainStealth();
                        }
                        else
                        {
                            if (!player.GetVisible())
                                player.LoseStealth();
                        }
                    }

                    players_ready_perform_actions = 0;
                    curr_action = action.wait;
                }
                break;
            case action.client_die:
                {
                    finish_player_turn.gameObject.SetActive(false);
                    attack_button.gameObject.SetActive(false);
                    nav_map.DeleteMarker();

                    curr_action = action.wait;
                }
                break;
        }

        DecreaseTurnTime();
    }

    private IEnumerator SendTimeStamp()
    {
        // send a packet with our current time
        using (RTData data = RTData.Get())
        {
            data.SetLong(1, (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds); // get the current time as unix timestamp
            RT_manager.SendData(101, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data, new int[] { 0 }); // send to peerId -> 0, which is the server
        }
        yield return new WaitForSeconds(2f); // wait 5 seconds
        StartCoroutine(SendTimeStamp()); // send the timestamp again
    }

    public void CalculateConnectionDelays(RTPacket _packet)
    {
        // calculate the time taken from the packet to be sent from the client and then for the server to return it //
        round_trip = (int)((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds - _packet.Data.GetLong(1).Value);
        latency = round_trip / 2; // the latency is half the round-trip time

        //Update Latency text
        latency_text.text = "Latency: " + latency + " ms";

        // calculate the server-delay from the server time minus the current time
        int serverDelta = (int)(_packet.Data.GetLong(2).Value - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
        server_delay = serverDelta + latency; // the server_delay is the serverdelta plus the latency
    }

    public void SetPlayersInfo(RTPacket _packet)
    {
        Debug.Log(net_manager.match);

        for (int p = 0; p < net_manager.match.GetPlayerList().Count; p++)
        {
            if (net_manager.match.GetPlayerList()[p].id == _packet.Data.GetString(1))
            {
                if (net_manager.match.GetPlayerList()[p].id == game_manager.playerID)
                {
                    player.text = net_manager.match.GetPlayerList()[p].displayName;
                }
                else
                {
                    switch (opponent_count)
                    {
                        case 0:
                            opp1.text = net_manager.match.GetPlayerList()[p].displayName;
                            break;
                        case 1:
                            opp2.text = net_manager.match.GetPlayerList()[p].displayName;
                            break;
                        case 2:
                            opp3.text = net_manager.match.GetPlayerList()[p].displayName;
                            break;
                    }
                    opponent_count++;
                }
                    SpawnPlayer(_packet, _packet.Data.GetString(1));
            }
        }
    }

    private IEnumerator DelayTurnStart()
    {
        yield return new WaitForSeconds(5f);
        using (RTData data = RTData.Get()) //send player ready to start the match to the server
        {
            Debug.Log("Send Ready to server..");
            data.SetLong(1, 0);
            RT_manager.SendData(104, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data, new int[] { 0 }); // send to peerId -> 0, which is the server
        }
    }

    public void SpawnPlayer(RTPacket _packet, string id)
    {
        bool local_player = false;

        for (int p = 0; p < net_manager.match.GetPlayerList().Count; p++)
        {
            if (id == game_manager.playerID)
            {
                local_player = true;
                break;
            }
        }

        int pos_x = (int)_packet.Data.GetInt(5);
        int pos_y = (int)_packet.Data.GetInt(6);

        Vector3 pos = nav_map.GridToWorldPoint(pos_x, pos_y).transform.position;

        // Create GameObjects
        GameObject player_go = Instantiate(player_prefab);

        GameObject player_shadow_go = Instantiate(player_prefab);

        // Setup Player
        Player player_script = player_go.GetComponent<Player>();
        player_script.SetInitialPlayerInfo(pos, id, local_player, player_shadow_go);
        player_script.SetStealth(true);

        PlayerShadow player_shadow_script = player_shadow_go.GetComponent<PlayerShadow>();
        player_shadow_script.SetInitialPlayerShadowInfo(player_go);
        player_shadow_script.Appear();

        if (local_player)
        {
            player_go.GetComponent<Renderer>().material.color = new Color(0, 0.3f, 0);
            player_shadow_go.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
            SetLifeText(3);
        }
        else
        {
            player_script.Desappear();
        }

        // Add player
        players.Add(player_go);
    }

    public void StartNewTurn(RTPacket _packet)
    {
        string t_type = (string)_packet.Data.GetString(1);
        int duration = (int)_packet.Data.GetInt(2);

        turn_type_text.text = t_type;
        turn_info.turn_time_left = duration;

        switch(t_type)
        {
            case "Strategy":
                turn_info.turn = turn_type.strategy;

                state_text.gameObject.SetActive(false);
                attack_button.gameObject.SetActive(true);
                finish_player_turn.gameObject.SetActive(true);

                for (int i = 0; i < players.Count; i++)
                {
                    Player player = players[i].GetComponent<Player>();

                    if (player.IsDead() && turn > 1)
                    {
                        KillPlayer(players[i]);
                        continue;
                    }

                    player.AdvanceTurn(turn_info);
                }

                turn++;
                break;

            case "Actions":
                turn_info.turn = turn_type.action;

                state_text.gameObject.SetActive(true);
                state_text.text = "Actions!";

                finish_player_turn.gameObject.SetActive(false);
                attack_button.gameObject.SetActive(false);
                break;
        }
    }

    public void UpdatePlayersPosition(RTPacket _packet)
    {
        string id = _packet.Data.GetString(1);
        int pos_x = (int)_packet.Data.GetInt(2);
        int pos_y = (int)_packet.Data.GetInt(3);
        int shadow_x = (int)_packet.Data.GetInt(4);
        int shadow_y = (int)_packet.Data.GetInt(5);
        string attack = _packet.Data.GetString(6);
        string revealed = _packet.Data.GetString(7);
        int life = (int)_packet.Data.GetInt(8);
        int num_card = (int)_packet.Data.GetInt(9);

        GameObject player = null;
        Player player_script = null;

        player = GetPlayerById(id);

        if (player != null)
        {
            player_script = player.GetComponent<Player>();

            if (attack == "true")
                player_script.SetAttack(true);
            else
                player_script.SetAttack(false);

            if (revealed == "true")
                player_script.SetStealth(false);
            else
                player_script.SetStealth(true);

            player_script.MoveTo(pos_x, pos_y);
            player_script.GetPlayerShadow().GetNavigationEntity().MoveTo(shadow_x, shadow_y);
            player_script.GetPlayerShadow().AddPosition(nav_map.GridToWorldPoint(pos_x, pos_y));
            player_script.SetLife(life);
            player_script.SetNumCards(num_card);

            Debug.Log("Recived player pos. Id: " + id + ", x:" + pos_x + ", y:" + pos_y);

            players_ready_perform_actions++;

            curr_action = action.check_if_all_players_finished_moving;
        }
    }

    public void ClientCardObtained(RTPacket _packet)
    {
        string name = _packet.Data.GetString(1);

        Player client_player = GetClientPlayer().GetComponent<Player>();

        client_player.TakeCard(name);

        UpdateCardsUI();
    }

    public void UpdateCardsUI()
    {
        Player client_player = GetClientPlayer().GetComponent<Player>();

        switch (client_player.GetCardsCount())
        {
            case 0:
                card1.gameObject.SetActive(false);
                card2.gameObject.SetActive(false);
                card3.gameObject.SetActive(false);
                break;
            case 1:
                card1.gameObject.SetActive(true);
                card1.GetComponentInChildren<Text>().text = client_player.GetCardNameByIndex(0);
                break;
            case 2:
                card1.gameObject.SetActive(true);
                card1.GetComponentInChildren<Text>().text = client_player.GetCardNameByIndex(0);

                card2.gameObject.SetActive(true);
                card2.GetComponentInChildren<Text>().text = client_player.GetCardNameByIndex(1);
                break;
            case 3:
                card1.gameObject.SetActive(true);
                card1.GetComponentInChildren<Text>().text = client_player.GetCardNameByIndex(0);

                card2.gameObject.SetActive(true);
                card2.GetComponentInChildren<Text>().text = client_player.GetCardNameByIndex(1);

                card3.gameObject.SetActive(true);
                card3.GetComponentInChildren<Text>().text = client_player.GetCardNameByIndex(2);
                break;
        }
    }

    public void CardUseRecieved(RTPacket _packet)
    {
        string card_name = _packet.Data.GetString(1);
        int pos_x = (int)_packet.Data.GetInt(2);
        int pos_y = (int)_packet.Data.GetInt(3);
        string id = _packet.Data.GetString(4);
        string target_id = _packet.Data.GetString(5);

        GameObject player = null;
        Player player_script = null;

        GameObject target = null;
        Player target_player_script = null;

        player = GetPlayerById(id);
        target = GetPlayerById(target_id);

        if(player != null)
            player_script = player.GetComponent<Player>();
        
        if (target != null)
            target_player_script = target.GetComponent<Player>();

        player_script.UseCard(card_name, target_player_script);

        UpdateCardsUI();
    }

    public void CardUseSend(string card_name, int pos_x, int pos_y, string id, string target_id)
    {
        using (RTData data = RTData.Get())
        {
            data.SetString(1, card_name);
            data.SetInt(2, pos_x);
            data.SetInt(3, pos_y);
            data.SetString(1, id);
            data.SetString(1, target_id);

            RT_manager.SendData(122, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data, new int[] { 0 }); // send to peerId -> 0, which is the server
        }
    }

    private void DecreaseTurnTime()
    {
        if (turn_info.turn_time_left > 0)
        {
            turn_info.turn_time_left -= Time.deltaTime*1000;
            int time_left = (int)(turn_info.turn_time_left / 1000);

            timer.text = time_left.ToString();
        }
        else
            turn_info.turn_time_left = 0;
    }

    public TurnInfo GetTurnInfo()
    {
        return turn_info;
    }

    public GameObject GetClientPlayer()
    {
        GameObject player = null;

        for (int i = 0; i < players.Count; i++)
        {
            Player player_script = players[i].GetComponent<Player>();

            if (player_script.IsClient())
            {
                player = players[i];
                break;
            }
        }

        return player;
    }

    public GameObject GetPlayerById(string id)
    {
        GameObject ret = null;

        for (int i = 0; i < players.Count; i++)
        {
            Player player_script = players[i].GetComponent<Player>();

            if (player_script.GetInstanceID().ToString() == id)
            {
                ret = players[i];
                break;
            }
        }

        return ret;
    }

    public void SendPlayerPos(string id, int pos_x, int pos_y, int shadow_x, int shadow_y, bool attack, bool reveled, int num_cards)
    {
        GameObject player = null;
        Player player_script = null;

        player = GetPlayerById(id);

        if(player != null)
        {
            player_script = player.GetComponent<Player>();

            player_script.SetTargetPos(new Vector2(pos_x, pos_y));

            using (RTData data = RTData.Get())
            {
                data.SetString(1, id);
                data.SetInt(2, pos_x);
                data.SetInt(3, pos_y);
                data.SetInt(4, shadow_x);
                data.SetInt(5, shadow_y);
                data.SetString(6, attack.ToString().ToLower());
                data.SetString(7, reveled.ToString().ToLower());
                data.SetInt(9, num_cards);

                RT_manager.SendData(122, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data, new int[] { 0 }); // send to peerId -> 0, which is the server

                Debug.Log("Sending position to x:" + pos_x + ", y:" + pos_y);
            }
        }   
    }

    public void ClientAttack()
    {
        Player player = GetClientPlayer().GetComponent<Player>();

        player.SetAttack(true);

        attack_button.gameObject.SetActive(false);
    }

    public void TileClicked(int x, int y)
    {
        if (GetTurnInfo().turn == turn_type.strategy)
        {
            Player client_player = GetClientPlayer().GetComponent<Player>();

            GameObject player_pos = client_player.GetNavigationEntity().GetClosestNavPoint();

            client_player.SetTargetPos(new Vector2(x, y));

            nav_map.PlaceMarker(nav_map.GridToWorldPoint(x, y).transform.position);
        }
    }

    public void EndPlayerTurn()
    {
        Player player = GetClientPlayer().GetComponent<Player>();

        string id = player.GetNetworkId();
        int pos_x = (int)player.GetTargetPos().x;
        int pos_y = (int)player.GetTargetPos().y;
        int shadow_x = (int)player.GetPlayerShadow().GetNextPos().x;
        int shadow_y = (int)player.GetPlayerShadow().GetNextPos().y;
        bool attack = player.GetAttack();
        bool revealed = player.GetStealth();
        int num_cards = player.GetCardsCount();

        SendPlayerPos(id, pos_x, pos_y, shadow_x, shadow_y, attack, revealed, num_cards);

        finish_player_turn.gameObject.SetActive(false);
        attack_button.gameObject.SetActive(false);

        state_text.gameObject.SetActive(true);
        state_text.text = "Waiting for the other players...";
    }

    public List<GameObject> GetPlayers()
    {
        return players;
    }

    public void KillPlayer(GameObject player)
    {
        Player player_script = player.GetComponent<Player>();

        if (player_script == null)
            return;

        players.Remove(player);

        player_script.GetPlayerShadow().gameObject.SetActive(false);
        player.SetActive(false);

        if (player_script.IsClient())
            curr_action = action.client_die;
    }

    public void SetLifeText(int set)
    {
        if (set > 0)
            client_life_text.text = "Life: " + set;
        else
            client_life_text.text = "Ur ded m8";
    }

    public void OnCardClick(int card)
    {
        if (turn_info.turn != turn_type.strategy)
            return;

        string name = "";

        switch(card)
        {
            case 1:
                name = card1.GetComponentInChildren<Text>().text;
                break;
            case 2:
                name = card2.GetComponentInChildren<Text>().text;
                break;
            case 3:
                name = card3.GetComponentInChildren<Text>().text;
                break;
        }

        switch (name)
        {
            case "Raven":
                last_clicked_card = "Raven";
                curr_action = action.card_select_target;
                break;

            case "Stealth":
                last_clicked_card = "Stealth";
                Player client = GetClientPlayer().GetComponent<Player>();

                CardUseSend("Stealth", (int)client.GetNavigationEntity().GetGridPos().x, (int)client.GetNavigationEntity().GetGridPos().y, client.GetInstanceID().ToString(), "0");
                break;
        }
    }

    public struct TurnInfo
    { 
        public turn_type turn;
        public float turn_time_left;
    }

    public enum turn_type
    {
        strategy,
        action,
        stop,
    }

    public enum action
    {
        wait,
        player_finished_moving,
        check_if_all_players_finished_moving,
        perform_attacks,
        client_die,

        card_select_target,
        card_target_selected,
    }
}
