using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Facebook;
using Facebook.Unity;
using GameSparks.Api;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class FacebookLoginUI : MonoBehaviour {

    //General player information variables
    public string fbAuthToken = "";

    void Awake()
    {
        //load authtoken from playerprefs and see if it's been filled in
        Load();
        if (fbAuthToken != "")
        {
            GameSparksFBLogBackIn();
        }
    }

    public void CallFBInit()
    {
        //initialize facebook
        FB.Init(this.OnInitComplete, this.OnHideUnity);
    }

    private void OnInitComplete()
    {
        Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
        CallFBLogin();
    }

    private void CallFBLogin()
    {
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, GameSparksFBLogin);
    }

    private void GameSparksFBLogin(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            fbAuthToken = AccessToken.CurrentAccessToken.TokenString;
            Save();
            new FacebookConnectRequest()
                .SetAccessToken(AccessToken.CurrentAccessToken.TokenString)
                .SetSwitchIfPossible(true)
                .SetSyncDisplayName(true)
                .Send((response) => {
                    if (response.HasErrors)
                    {
                        Debug.Log("Something failed when connecting with Facebook - " + result.Error);
                    }
                    else
                    {
                        Debug.Log("Gamesparks Facebook login successful");
                    }
                });
        }
    }

    private void GameSparksFBLogBackIn()
    {
        new FacebookConnectRequest()
            .SetAccessToken(fbAuthToken)
            .Send((response) => {
                if (response.HasErrors)
                {
                    Debug.Log("Something failed when connecting with Facebook on log back in");
                }
                else
                {
                    Debug.Log("Gamesparks Facebook login successful on log back in");
                }
            });
    }

    private void OnHideUnity(bool isGameShown)
    {

    }

    void Save()
    {
        PlayerPrefs.SetString("fbAuthToken", fbAuthToken);
    }

    void Load()
    {
        fbAuthToken = PlayerPrefs.GetString("fbAuthToken");
    }

}
