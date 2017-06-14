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
        string response = game_manager.GetNetworkManager().Login(username.text, password.text);

        if(response == "SUCCES")
        {
            game_manager.GetUIManager().DisableWindow("login_register");
            game_manager.GetUIManager().DisableWindow("register");
            game_manager.GetUIManager().EnableWindow("lobby");
        }
	}
}
