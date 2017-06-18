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

    public Text opp1, opp2, opp3, player;
    public Text timer, turn_type;

    private int server_delay, latency, round_trip; //All in ms
    private int opponent_count = 0;

    public Text latency_text;

    private List<GameObject> players = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        RT_manager = game_manager.GetGameSparksRTManager();
        net_manager = game_manager.GetNetworkManager();

        StartCoroutine(SendTimeStamp());

        // Inform the server that the match is ready
        using (RTData data = RTData.Get())
        {
            data.SetLong(1, 0);
            RT_manager.SendData(103, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data, new int[] { 0 }); // send to peerId -> 0, which is the server
        }

        StartCoroutine(DelayTurnStart());
    }

    // Update is called once per frame
    void Update()
    {

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
                break;
            }
        }

        SetupPlayers();
    }

    public void SetTurn(RTPacket _packet)
    {
        turn_type.text = _packet.Data.GetString(1);
        int time_sec;
        time_sec = (int)_packet.Data.GetInt(2) / 1000;
        timer.text = time_sec.ToString();
    }

    public void DecrementTimer()
    {
        int remaining_time = int.Parse(timer.text);
        remaining_time -= 1;
        timer.text = remaining_time.ToString();
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

    public void SetupPlayers()
    {

    }

    private void SpawnPlayer()
    {

    }

    public void UpdateOponentsPosition(RTPacket _packet)
    {

    }
}
