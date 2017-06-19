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

    [SerializeField]
    private Text name;

    [SerializeField]
    private GameObject error_panel;

    [SerializeField]
    private GameObject[] to_active;

    void Start ()
    {
        game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        error_panel.SetActive(false);
    }
	
	public void ButtonPressed()
    {
        Debug.Log("Login attempt. User: " + username.text + "Pass: " + password.text);

        Text curr_text = error_panel.GetComponentInChildren<Text>();

        if (username.text == "" || password.text == "")
        {
            curr_text.text = "Fill all blanks before log in";

            for (int i = 0; i < to_active.Length; i++)
            {
                to_active[i].SetActive(false);
            }

            error_panel.SetActive(true);
            return;
        }

        new GameSparks.Api.Requests.AuthenticationRequest().
            SetUserName(username.text)
            .SetPassword(password.text)
            .Send((response) =>
            {
                if (response.HasErrors)
                {
                    Debug.Log("Login error: " + response.Errors.JSON.ToString());

                    // USE response.Errors.GetString("DETAILS"); to get the error type
                    for (int i = 0; i < to_active.Length; i++)
                    {
                        to_active[i].SetActive(false);
                    }

                    string error_message = response.Errors.GetString("DETAILS");

                    if (error_message == "UNRECOGNISED")
                    {
                        curr_text.text = "Wrong username/password combination";
                    }

                    else
                    {
                        curr_text.text = "Servers unavailable. Try again later";
                    }

                    error_panel.SetActive(true);

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
                    name.text = response.DisplayName.ToString();

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
