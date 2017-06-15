using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.RT;

public class LobbyUI : MonoBehaviour {

    public Button PlayBttn;
    public NetworkManager NetManager;

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

        gameSparksRTunity.Configure(_message,
            (peerID) => { OnPlayerConnectedToGame(peerID); },
            (peerID) => { OnPlayerDisconnected(peerID); },
            (ready) => { OnRTReady(ready); },
            (packet) => { OnPacketReceived(packet); });

        gameSparksRTunity.Connect();

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

    private void OnPlayerConnectedToGame(int _peerId)
    {
        Debug.Log("GSM| Player Connected, " + _peerId);
    }

    private void OnPlayerDisconnected(int _peerId)
    {
        Debug.Log("GSM| Player Disconnected, " + _peerId);
    }

    private void OnRTReady(bool _isReady)
    {
        if (_isReady)
        {
            Debug.Log("GSM| RT Session Connected...");
        }

    }

    private void OnPacketReceived(RTPacket _packet)
    {
    }

    private void OnMatchReady(bool _isReady)
    {
        if (_isReady)
        {
            Debug.Log("Connected with the Match...");
            //Change to game scene
        }
    }

    // Use this for initialization
    void Start()
    {
        PlayBttn.onClick.AddListener(() =>
        {
            NetManager.NR_4PMatchMaking();
            Debug.Log("Serching For Players..");
        });

        GameSparks.Api.Messages.MatchNotFoundMessage.Listener = (message) =>
        {
            Debug.Log("No Match Found...");
        };

        GameSparks.Api.Messages.MatchFoundMessage.Listener += OnMatchFound;
    }


}
