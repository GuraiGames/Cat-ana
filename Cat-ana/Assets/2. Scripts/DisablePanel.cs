using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePanel : MonoBehaviour {

    public GameObject panel;

    void Awake()
    {
        panel.SetActive(false);
    }
}
