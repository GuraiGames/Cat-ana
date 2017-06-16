using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;
using System;

public class MatchManager : MonoBehaviour
{

    private GameManager game_manager;

    private int server_ms_delay = 0;

	// Use this for initialization
	void Start ()
    {
        game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        SendTimeStamp();
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
}
