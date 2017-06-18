using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api;

public class ScrollSpawn : MonoBehaviour
{
    public enum rarity
    {
        r_null,
        r_common,
        r_uncommon,
        r_rare
    }

    public GameObject[] scroll_go;
    public Text[] timer_txt;
    public float[] timer = new float[4];
    public rarity[] scroll_rarity = new rarity[4];

    [SerializeField]
    private int active_scroll_index = -1;

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

                if (i == active_scroll_index)
                    timer[i] -= Time.deltaTime;
            }
        }

    }

    public void SetScroll(GameSparks.Core.GSData data, long time_now)
    {
        for (int i = 0; i < 4; i++)
        {
            string scroll_type_num = "scroll" + i + "_type";
            scroll_type_num = data.GetString(scroll_type_num);
            if (scroll_type_num == "r_null")
            {
                scroll_rarity[i] = rarity.r_null;
            }
            else
            {
                if (scroll_type_num == "r_common")
                {
                    scroll_rarity[i] = rarity.r_common;
                }
                else if (scroll_type_num == "r_uncommon")
                {
                    scroll_rarity[i] = rarity.r_uncommon;
                }
                else if (scroll_type_num == "r_rare")
                {
                    scroll_rarity[i] = rarity.r_rare;
                }

                string scroll_finish = "scroll" + i + "_finish";
                Debug.Log(scroll_finish);
                long time_finish = (long)data.GetLong(scroll_finish);

                if (time_finish - time_now <= 0)
                {
                    timer[i] = 0;
                }
                else
                {
                    timer[i] = (time_finish - time_now) / 1000;
                }

                active_scroll_index = (int)data.GetInt("active_scroll");

                scroll_go[i].SetActive(true);
            }
            Debug.Log("/n Rarity: " + scroll_type_num);

        }
    }
}
