using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Api.Messages;

public class MatchInfo
{
    private string hostURL;
    public string GetHostURL() { return this.hostURL; }

    private string acccessToken;
    public string GetAccessToken() { return this.acccessToken; }

    private int portID;
    public int GetPortID() { return this.portID; }

    private string matchID;
    public string GetMatchID() { return this.matchID; }

    private List<Player> playerList = new List<Player>();
    public List<Player> GetPlayerList()
    {
        return playerList;
    }

    public MatchInfo(MatchFoundMessage _message)
    {
        portID = (int)_message.Port;
        hostURL = _message.Host;
        acccessToken = _message.AccessToken;
        matchID = _message.MatchId;
        // we loop through each participant and get their peerId and display name //
        foreach (MatchFoundMessage._Participant p in _message.Participants)
        {
            playerList.Add(new Player(p.DisplayName, p.Id, (int)p.PeerId));
        }
    }

    public class Player
    {
        public Player(string _displayName, string _id, int _peerId)
        {
            this.displayName = _displayName;
            this.id = _id;
            this.peerId = _peerId;
        }

        public string displayName;
        public string id;
        public int peerId;
        public bool isOnline;
    }
}