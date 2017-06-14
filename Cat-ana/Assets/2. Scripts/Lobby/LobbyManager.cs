using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LobbyManager : MonoBehaviour {

    public Button PlayBttn;
    public Matchmaking MM_Manager;

    private void OnMatchFound(GameSparks.Api.Messages.MatchFoundMessage _message)
    {
        Debug.Log("Match Found!...");

        Debug.Log("Match Found...");
        Debug.Log("Host URL:" + _message.Host);
        Debug.Log("Port:" + _message.Port);
        Debug.Log("Access Token:" + _message.AccessToken);
        Debug.Log("MatchId:" + _message.MatchId);
        Debug.Log("Opponents:" + _message.Participants.Count());
        Debug.Log("_________________");
       
        foreach (GameSparks.Api.Messages.MatchFoundMessage._Participant player in _message.Participants)
        {
            Debug.Log("Player:" + player.PeerId + " User Name:" + player.DisplayName); // add the player number and the display name to the list
        }
    }

    void Start()
    {
        PlayBttn.onClick.AddListener(() =>
        {
            MM_Manager.NR_4PMatchMaking();
            Debug.Log("Serching For Players..");
        });

        GameSparks.Api.Messages.MatchNotFoundMessage.Listener = (message) =>
        {
            Debug.Log("No Match Found...");
        };

        GameSparks.Api.Messages.MatchFoundMessage.Listener += OnMatchFound;
    }
}
