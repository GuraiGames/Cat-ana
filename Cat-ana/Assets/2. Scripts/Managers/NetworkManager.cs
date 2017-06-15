﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameSparks.RT;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    //MatchMaking

    public void NR_4PMatchMaking()
    {
        new GameSparks.Api.Requests.MatchmakingRequest().SetMatchShortCode("4P_NRMATCH")
            .SetSkill(0)
            .Send((response) =>
            {
                if (response.HasErrors)
                {
                    Debug.LogError("MatchMakingError \n" + response.Errors.JSON);
                }
            });
    }
}
