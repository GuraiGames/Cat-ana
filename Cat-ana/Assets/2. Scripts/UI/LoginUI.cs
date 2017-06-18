using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour {

    private GameManager game_manager;

    [SerializeField]
    private InputField username;

    [SerializeField]
    private InputField password;

    [SerializeField]
    private ScrollSpawn scroll;


    void Start ()
    {
        game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}
	
	public void ButtonPressed()
    {
        Debug.Log("Login attempt. User: " + username.text + "Pass: " + password.text);

        new GameSparks.Api.Requests.AuthenticationRequest().
            SetUserName(username.text)
            .SetPassword(password.text)
            .Send((response) =>
            {
                if (response.HasErrors)
                {
                    Debug.Log("Login error: " + response.Errors.JSON.ToString());

                    // USE response.Errors.GetString("DETAILS"); to get the error type
                }
                else
                {
                    Debug.Log("Login succes");
                    game_manager.GetUIManager().DisableWindow("login_register");
                    game_manager.GetUIManager().DisableWindow("register");
                    game_manager.GetUIManager().EnableWindow("lobby");
                    game_manager.GetUIManager().EnableWindow("scroll_ui");
                    game_manager.GetUIManager().EnableWindow("global_ui");

                    game_manager.playerID = response.UserId.ToString();

                    new GameSparks.Api.Requests.LogEventRequest()
                      .SetEventKey("CREATE_SCROLLS")
                      .Send((scroll_response) => {
                          if (scroll_response.HasErrors)
                          {
                              Debug.Log("Error");
                          }
                          else
                          {
                              Debug.Log("NICE");
                              new GameSparks.Api.Requests.LogEventRequest()
                                .SetEventKey("GET_SCROLLS")
                                .Send((scroll_response2) =>
                                {
                                    if (!scroll_response2.HasErrors)
                                    {
                                        Debug.Log("Scrolls found");

                                        GameSparks.Core.GSData data = scroll_response2.ScriptData.GetGSData("player_scrolls");
                                        GameSparks.Core.GSData time = scroll_response2.ScriptData.GetGSData("time_now");
                                        scroll.SetScroll(data, (long)time.GetLong("current_time"));
                                    }
                                    else
                                    {
                                        Debug.Log("Error finding scrolls");
                                    }
                                });
                          }

                      });

                    

                }
            });

    }
}
