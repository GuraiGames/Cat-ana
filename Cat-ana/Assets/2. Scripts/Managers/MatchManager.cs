﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.RT;
using System;

public class MatchManager : MonoBehaviour
{

    private GameManager game_manager = null;

    private int server_delay, latency, round_trip; //All in ms

    public Text latency_text;

	// Use this for initialization
	void Start ()
    {
        game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        StartCoroutine(SendTimeStamp());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator SendTimeStamp()
    {
        // send a packet with our current time
        using (RTData data = RTData.Get())
        {
            Debug.Log("Clock Sync tick");
            data.SetLong(1, (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds); // get the current time as unix timestamp
            game_manager.GetGameSparksRTManager().SendData(101, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE, data, new int[] { 0 }); // send to peerId -> 0, which is the server
        }
        yield return new WaitForSeconds(5f); // wait 5 seconds
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
}