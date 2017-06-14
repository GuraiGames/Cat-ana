﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginBttn : MonoBehaviour
{

    public Text user_name, password, error_text;

    public GameObject panel;
    public GameObject user_name_go;
    public GameObject password_go;
    public GameObject register_go;
    public GameObject login_go;

    private int panel_message;

    public void LoginBttnClick()
    {
        new GameSparks.Api.Requests.AuthenticationRequest().SetUserName(user_name.text)
            .SetPassword(password.text)
            .Send((response) =>
            {
                if (response.HasErrors)
                {
                    user_name_go.SetActive(false);
                    password_go.SetActive(false);
                    register_go.SetActive(false);
                    login_go.SetActive(false);

                    //Identify Error and set corresponding text
                    string error = response.Errors.GetString("DETAILS");

                    Debug.Log(error);

                    if (error == "UNRECOGNISED")
                    {
                        error_text.text = "Invalid User/Password combination.";
                    }
                    else
                    {
                        error_text.text = "Error login in. Try again later.";
                    }

                    panel.SetActive(true);
                    
                }
                else
                {
                    Debug.Log("Login OK!");
                    //Go to lobby
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
                }
            });
    }
}