using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginBttn : MonoBehaviour
{

    public Text user_name, password;
    public GameObject panel;

    public void LoginBttnClick()
    {
        new GameSparks.Api.Requests.AuthenticationRequest().SetUserName(user_name.text)
            .SetPassword(password.text)
            .Send((response) =>
            {
                if (response.HasErrors)
                {
                    panel.SetActive(true);
                }
                else
                {
                    Debug.Log("Login OK!");
                    //Go to lobby
                }
            });
    }
}