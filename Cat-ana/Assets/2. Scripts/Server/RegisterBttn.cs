using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterBttn : MonoBehaviour {

    public Text user_name, display_name, password;
    public GameObject panel;

    public void RegisterBttnClick()
    {
        new GameSparks.Api.Requests.RegistrationRequest().SetDisplayName(display_name.text)
            .SetPassword(password.text)
            .SetUserName(user_name.text)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    ChangeScene a = new ChangeScene();
                    a.SetScene("Login");
                    a.ChangeToScene();
                }
                else
                {
                    panel.SetActive(true);
                }
            });
    }
}
