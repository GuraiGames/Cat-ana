using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorTimer : MonoBehaviour {

    [SerializeField]
    private GameObject go;

    public float timer = 0;

    private void Start()
    {
        timer = 2;
    }

    // Update is called once per frame
    void Update () {
		if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            go.SetActive(false);
        }
	}

    public void RestartTimer(float time)
    {
        timer = time;
    }
}
