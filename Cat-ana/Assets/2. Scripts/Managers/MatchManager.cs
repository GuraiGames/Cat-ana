﻿using System.Collections;
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
    public Text opp1, opp2, opp3, player;
    public Text timer, turn_type_text;
    public Text latency_text;

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
    }

    // Update is called once per frame
    void Update()
    {
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

        GameObject player_go = Instantiate(player_prefab);
        Player script = player_go.GetComponent<Player>();
        script.SetInitialPlayerInfo(pos, id, local_player);
    }

    public void StartNewTurn(RTPacket _packet)
    {
        string t_type = (string)_packet.Data.GetString(1);
        int duration = (int)_packet.Data.GetInt(2);

        //turn_type_text.text = t_type;
        turn_info.turn_time_left = duration;

        switch(t_type)
        {
            case "strategy":
                turn_info.turn = turn_type.strategy;
                break;

            case "action":
                turn_info.turn = turn_type.action;
                break;
        }
    }

    public void UpdateOponentPosition(RTPacket _packet)
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
            player_script.GetNavigationEntity().MoveTo(pos_x, pos_y);
        }
    }

    private void DecreaseTurnTime()
    {
        if (turn_info.turn_time_left > 0)
        {
            turn_info.turn_time_left -= Time.deltaTime;
            timer.text = (turn_info.turn_time_left / 1000).ToString();
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

            if (player_script.IsPlayer())
            {
                player = players[i];
                break;
            }
        }

        return player;
    }

    public void SendPlayerPos(string id, int pos_x, int pos_y, int shadow_x, int shadow_y, bool attack, bool reveled)
    {
        if (turn_info.turn != turn_type.action)
            return;

        GameObject player = null;

        for(int i = 0; i < players.Count; i++)
        {
            Player pl = players[i].GetComponent<Player>();

            if(pl.GetNetworkId() == id)
            {
                player = players[i];
                break;
            }
        }

        if(player != null)
        {
            using (RTData data = RTData.Get())
            {
                data.SetString(1, id);
                data.SetInt(2, pos_x);
                data.SetInt(3, pos_y);
                data.SetInt(4, shadow_x);
                data.SetInt(5, shadow_y);
                data.SetString(6, attack.ToString());
                data.SetString(7, reveled.ToString());

                RT_manager.SendData(122, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data, new int[] { 0 }); // send to peerId -> 0, which is the server
            }
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
}
