﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public string Login(string name, string password)
    {
        Debug.Log("Login attempt. User: " + name + "Pass: " + password);

        string ret = "SUCCES";

        new GameSparks.Api.Requests.AuthenticationRequest().
            SetUserName(name)
            .SetPassword(password)
            .Send((response) =>
            {
                if (response.HasErrors)
                {
                    ret = response.Errors.JSON.ToString();
                    Debug.Log("Login error: " + response.Errors.JSON.ToString());
                }
                else
                {
                    Debug.Log("Login succes");
                }
            });

        return ret;
    }

    public string Register(string name, string password, string player_username)
    {
        Debug.Log("Register attemt. User: " + name + "Pass: " + password);

        string ret = "SUCCES";

        new GameSparks.Api.Requests.RegistrationRequest()
        .SetDisplayName(name)
        .SetPassword(password)
        .SetUserName(player_username)
        .Send((response) =>
        {
            if (response.HasErrors)
            {
                ret = response.Errors.JSON.ToString();
                Debug.Log("Register error: " + response.Errors.JSON.ToString());
            }
            else
            {
                Debug.Log("Register succes");
            }
        }
        );

        return ret;
    }
}