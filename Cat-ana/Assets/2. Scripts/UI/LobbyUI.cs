using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.RT;

public class LobbyUI : MonoBehaviour {

    public Button PlayBttn;

    private MatchInfo match;

    GameManager game_manager = null;

    public MatchInfo GetMatchInfo()
    {
        return match;
    }

    private GameSparksRTUnity gameSparksRTunity;

    public GameSparksRTUnity GetRTSession()
    {
        return gameSparksRTunity;
    }

    private void OnMatchFound(GameSparks.Api.Messages.MatchFoundMessage _message)
    {
        Debug.Log("Match Found!...");

        GameManager game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        GameSparksRTUnity RT_manager = game_manager.GetGameSparksRTManager();
        NetworkManager net_manager = game_manager.GetNetworkManager();

        net_manager.match = new MatchInfo(_message);

        RT_manager.Configure(_message,
            (peerID) => { net_manager.OnPlayerConnectedToGame(peerID); },
            (peerID) => { net_manager.OnPlayerDisconnected(peerID); },
            (ready) => { net_manager.OnRTReady(ready); },
            (packet) => { net_manager.OnPacketReceived(packet); });
        RT_manager.Connect();

        //Uncoment that to Debug the Match info
        /*Debug.Log("Match Found...");
        Debug.Log("Host URL:" + _message.Host);
        Debug.Log("Port:" + _message.Port);
        Debug.Log("Access Token:" + _message.AccessToken);
        Debug.Log("MatchId:" + _message.MatchId);
        Debug.Log("Opponents:" + _message.Participants.Count());
        Debug.Log("_________________");
       
        foreach (GameSparks.Api.Messages.MatchFoundMessage._Participant player in _message.Participants)
        {
            Debug.Log("Player:" + player.PeerId + " User Name:" + player.DisplayName); // add the player number and the display name to the list
        }*/


    }

    // Use this for initialization
    void Start()
    {
        game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        GameSparks.Api.Messages.MatchNotFoundMessage.Listener = (message) =>
        {
            Debug.Log("No Match Found...");
        };

        GameSparks.Api.Messages.MatchFoundMessage.Listener += OnMatchFound;
    }

    public void PlayButton()
    {
        game_manager.GetNetworkManager().NR_4PMatchMaking();
        Debug.Log("Serching For Players..");
    }


}
