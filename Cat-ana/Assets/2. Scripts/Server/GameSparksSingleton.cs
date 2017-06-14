using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSparksSingleton : MonoBehaviour {

    private static GameSparksSingleton instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
}
