using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeNameUI : MonoBehaviour {

    private GameManager game_manager;

    [SerializeField]
    private InputField display_name;

    [SerializeField]
    private InputField old_password;

    [SerializeField]
    private InputField new_password;

    void Start()
    {
        game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void ButtonPressed()
    {
        Debug.Log("Change attemt. Display Name: " + display_name.text + "Pass: " + new_password.text);

        new GameSparks.Api.Requests.ChangeUserDetailsRequest()
        .SetDisplayName(display_name.text)
        .SetOldPassword(old_password.text)
        .SetNewPassword(new_password.text)
        .Send((response) =>
        {
            if (response.HasErrors)
            {
                Debug.Log("Register error: " + response.Errors.JSON.ToString());
            }
            else
            {
                Debug.Log("Change succes");
                game_manager.GetUIManager().DisableWindow("change_name_settings");
                game_manager.GetUIManager().EnableWindow("settings");
            }
        }
        );

    }
}
