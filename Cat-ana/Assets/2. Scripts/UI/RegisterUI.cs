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
        Debug.Log("Register attemt. User: " + username.text + "Pass: " + password.text);

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
