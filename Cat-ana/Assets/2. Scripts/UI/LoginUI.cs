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
                }
            });

    }
}
