using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsTimer : MonoBehaviour {

    [SerializeField]
    private float timer = 10f;

    [SerializeField]
    private GameObject about_window;

    // Update is called once per frame
    void Update () {
        timer -= Time.deltaTime;

        if (timer <= 0.5f)
        {
            about_window.SetActive(false);
        }
	}

    public void SetTimer(float time)
    {
        timer = time;
    }
}
