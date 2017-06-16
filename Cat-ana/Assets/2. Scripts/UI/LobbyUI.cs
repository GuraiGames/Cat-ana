using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.RT;

public class LobbyUI : MonoBehaviour {

    public Button PlayBttn;

    private MatchInfo match;

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

        match = new MatchInfo(_message);

        GameManager game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        game_manager.GetGameSparksRTManager().Configure(_message,
            (peerID) => { game_manager.GetNetworkManager().OnPlayerConnectedToGame(peerID); },
            (peerID) => { game_manager.GetNetworkManager().OnPlayerDisconnected(peerID); },
            (ready) => { game_manager.GetNetworkManager().OnRTReady(ready); },
            (packet) => { game_manager.GetNetworkManager().OnPacketReceived(packet); });

        game_manager.GetGameSparksRTManager().Connect();

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
        PlayBttn.onClick.AddListener(() =>
        {
            GameManager game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            game_manager.GetNetworkManager().NR_4PMatchMaking();
            Debug.Log("Serching For Players..");
        });

        GameSparks.Api.Messages.MatchNotFoundMessage.Listener = (message) =>
        {
            Debug.Log("No Match Found...");
        };

        GameSparks.Api.Messages.MatchFoundMessage.Listener += OnMatchFound;
    }


}
