using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameSparks.RT;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    //Match info
    public MatchInfo match;

    public MatchInfo GetMatchInfo()
    {
        return match;
    }
    // -----

    //MatchMaking
    public void NR_4PMatchMaking()
    {
        new GameSparks.Api.Requests.MatchmakingRequest().SetMatchShortCode("4P_NRMATCH")
            .SetSkill(0)
            .Send((response) =>
            {
                if (response.HasErrors)
                {
                    Debug.LogError("MatchMakingError \n" + response.Errors.JSON);
                }
            });
    }

    // -----

    // Player Connection states
    
    public void OnPlayerConnectedToGame(int _peerId)
    {
        Debug.Log("GSM| Player Connected, " + _peerId);
    }

    public void OnPlayerDisconnected(int _peerId)
    {
        Debug.Log("GSM| Player Disconnected, " + _peerId);
    }
    // -----

    //Real time session ready
    public void OnRTReady(bool _isReady)
    {
        if (_isReady)
        {
            Debug.Log("GSM| RT Session Connected...");
        }

    }
    // -----

    // Packet Handling
    public void OnPacketReceived(RTPacket _packet)
    {
        switch(_packet.OpCode)
        {
            // Add here a case for each OpCode. OpCodes documentation is available at drive/Code/OpCodes
            case 100: //All players connected to the server
                Debug.Log("Loading Game scene...");
                UnityEngine.SceneManagement.SceneManager.LoadScene("2.Game");
                break;
            case 101: //Sync Clock
                {
                    MatchManager match_manager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
                    match_manager.CalculateConnectionDelays(_packet);
                    break;
                }
            case 102://player match info
                {
                    Debug.Log("Recieved player " + _packet.Data.GetInt(1) + " info");
                    MatchManager match_manager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
                    match_manager.SetPlayersInfo(_packet);
                    break;
                }
        }
    }
    // -----

    public void OnMatchReady(bool _isReady)
    {
        if (_isReady)
        {
            Debug.Log("Connected with the Match...");
            //Change to game scene
        }
    }

}
