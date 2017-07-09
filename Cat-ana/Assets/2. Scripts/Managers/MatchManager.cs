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

    // Server
    private int server_delay, latency, round_trip; //All in ms
    private int opponent_count = 0;

    // Map
    private NavigationMap nav_map = null;

    // Players
    [SerializeField]
    private GameObject player_prefab;
    private List<GameObject> players = new List<GameObject>();

    // Turn
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

        attack_button.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        DecreaseTurnTime();

        PerformActions();
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

                attack_button.enabled = true;
                attack_button.interactable = true;
                break;

            case "Actions":
                turn_info.turn = turn_type.action;

                attack_button.interactable = false;
                attack_button.enabled = false;
                break;
        }


        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i].GetComponent<Player>();
            player.AdvanceTurn(turn_info);
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

        GameObject player = null;
        Player player_script = null;

        for (int i = 0; i < players.Count; i++)
        {
            player_script = players[i].GetComponent<Player>();

            if (player_script.GetNetworkId() == id)
            {
                player = players[i];
                break;
            }
        }

        if (player != null)
        {
            if (attack == "true")
                player_script.SetAttack(true);
            else
                player_script.SetAttack(false);

            if (revealed == "true")
                player_script.SetStealth(false);
            else
                player_script.SetStealth(true);

            player_script.GetNavigationEntity().MoveTo(pos_x, pos_y);
            player_script.GetPlayerShadow().GetNavigationEntity().MoveTo(shadow_x, shadow_y);
            player_script.GetPlayerShadow().AddPosition(nav_map.GridToWorldPoint(pos_x, pos_y));

            Debug.Log("Recived player pos. Id: " + id + ", x:" + pos_x + ", y:" + pos_y);

            players_ready_perform_actions++;
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

    public void SendPlayerPos(string id, int pos_x, int pos_y, int shadow_x, int shadow_y, bool attack, bool reveled)
    {
        GameObject player = null;
        Player player_script = null;

        for(int i = 0; i < players.Count; i++)
        {
            player_script = players[i].GetComponent<Player>();

            if(player_script.GetNetworkId() == id)
            {
                player = players[i];
                break;
            }
        }

        if(player != null)
        {
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

                RT_manager.SendData(122, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data, new int[] { 0 }); // send to peerId -> 0, which is the server

                Debug.Log("Sending position to x:" + pos_x + ", y:" + pos_y);
            }
        }   
    }

    public void ClientAttack()
    {
        Player player = GetClientPlayer().GetComponent<Player>();

        player.SetAttack(true);

        string id = player.GetNetworkId();
        int pos_x = (int)player.GetTargetPos().x;
        int pos_y = (int)player.GetTargetPos().y;
        int shadow_x = (int)player.GetPlayerShadow().GetNextPos().x;
        int shadow_y = (int)player.GetPlayerShadow().GetNextPos().y;
        bool attack = player.GetAttack();
        bool revealed = player.GetStealth();

        SendPlayerPos(id, pos_x, pos_y, shadow_x, shadow_y, attack, revealed);
    }

    public void TileClicked(int x, int y)
    {
        if (GetTurnInfo().turn == turn_type.strategy)
        {
            Player client_player = GetClientPlayer().GetComponent<Player>();

            GameObject player_pos = client_player.GetNavigationEntity().GetClosestNavPoint();
            
            string id = client_player.GetNetworkId();
            int pos_x = x;
            int pos_y = y;
            int shadow_x = (int)client_player.GetPlayerShadow().GetNextPos().x;
            int shadow_y = (int)client_player.GetPlayerShadow().GetNextPos().y;
            bool attack = client_player.GetAttack();
            bool revealed = client_player.GetStealth();

            SendPlayerPos(id, pos_x, pos_y, shadow_x, shadow_y, attack, revealed);

            nav_map.PlaceMarker(nav_map.GridToWorldPoint(pos_x, pos_y).transform.position);
        }
    }

    public void PerformActions()
    {
        if (turn_info.turn == turn_type.action && players_ready_perform_actions == 4)
        {
            bool can_perform = true;

            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i].GetComponent<Player>();

                if (player.IsMoving())
                    can_perform = false;
            }

            if (can_perform)
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
                    if(player.GetStealth())
                    {
                        if(player.GetVisible())
                            player.GainStealth();
                    }
                    else
                    {
                        if (!player.GetVisible())
                            player.LoseStealth();
                    }
                }

                players_ready_perform_actions = 0;
            }
        }
    }

    public List<GameObject> GetPlayers()
    {
        return players;
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
}
