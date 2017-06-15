using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterUI : MonoBehaviour
{
    private GameManager game_manager;

    [SerializeField]
    private int password_min_lenght;

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
        Debug.Log("Register attemt. User: " + username.text + "Pass: " + password.text);

        if (username.text == "" || display_name.text == "" && password.text == "")
        {
            Debug.Log("Register error: There is an empty field");
            return;
        }
        if(password.text.Length < password_min_lenght)
        {
            Debug.Log("Register error: Password should be longuer than " + password_min_lenght + " words");
            return;
        }

        new GameSparks.Api.Requests.RegistrationRequest()
        .SetDisplayName(display_name.text)
        .SetPassword(password.text)
        .SetUserName(username.text)
        .Send((response) =>
        {
            if (response.HasErrors)
            {
                Debug.Log("Register error: " + response.Errors.JSON.ToString());
            }
            else
            {
                Debug.Log("Register succes");
                game_manager.GetUIManager().EnableWindow("login_register");
                game_manager.GetUIManager().DisableWindow("register");
            }
        }
        );
    }
}
