using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollSpawn : MonoBehaviour
{

    public GameObject[] scroll_go;
    public Text[] timer_txt;

    public float[] timer = new float[4];

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < scroll_go.Length; i++)
        {
            scroll_go[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            for (int i = 0; i < scroll_go.Length; i++)
            {
                if (!scroll_go[i].activeSelf)
                {
                    scroll_go[i].SetActive(true);
                    timer[i] = 3600*2;
                    break;
                }
            }
        }

        for (int i = 0; i < scroll_go.Length; i++)
        {
            if (scroll_go[i].activeSelf)
            {
                string hours;
                string minutes = ((timer[i] % 3600) / 60).ToString("00");

                if (minutes == "60")
                {
                    hours = Mathf.Floor(timer[i] / 3600 + 1).ToString("00");
                    minutes = "00";
                }
                else
                {
                    hours = Mathf.Floor(timer[i] / 3600).ToString("00");      
                }

                timer_txt[i].text = hours + ":" + minutes;
                timer[i] -= Time.deltaTime;
            }
        }

    }
}
