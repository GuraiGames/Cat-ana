using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterUI : MonoBehaviour
{
    private GameManager game_manager;

    [SerializeField]
    private InputField username;

    [SerializeField]
    private InputField display_name;

    [SerializeField]
    private InputField password;

    void OnEnable()
    {
        game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void ButtonPressed()
    {
        string response = game_manager.GetNetworkManager().Register(username.text, password.text, display_name.text);

        if(response == "SUCCES")
        {
            game_manager.GetUIManager().EnableWindow("login_register");
            game_manager.GetUIManager().DisableWindow("register");
        }
    }

}
