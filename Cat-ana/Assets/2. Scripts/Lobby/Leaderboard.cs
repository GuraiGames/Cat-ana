using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour {

    public GameObject PlayerScore;
    public Text[] Top10_user;
    public Text[] Top10_score;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public void RefreshLeaderboard()
    {
        new GameSparks.Api.Requests.LeaderboardDataRequest().SetLeaderboardShortCode("GLOBAL_LEADERBOARD").SetEntryCount(10).Send((response) => {
            if (!response.HasErrors)
            {
                Debug.Log("Found Leaderboard Data...");
                int i = 0;
                foreach (GameSparks.Api.Responses.LeaderboardDataResponse._LeaderboardData entry in response.Data)
                {
                    int rank = (int)entry.Rank;
                    Top10_user[i].text = entry.UserName;
                    Top10_score[i].text = entry.JSONData["SCORE"].ToString() + "Rank: " + rank;
                }
            }
            else
            {
                Debug.Log("Error Retrieving Leaderboard Data...");
            }
        });
    }
}
