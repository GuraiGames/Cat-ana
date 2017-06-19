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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CreateScroll(2, 0);
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

                if (i == active_scroll_index && timer[i] > 0)
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
                string scroll_start = "scroll" + i + "_start";

                long time_finish = (long)data.GetLong(scroll_finish);
                long time_start = (long)data.GetLong(scroll_start);

                active_scroll_index = (int)data.GetInt("active_scroll");

                if (i == active_scroll_index)
                {
                    if (time_finish - time_now <= 0)
                    {
                        timer[i] = 0;
                    }
                    else
                    {
                        timer[i] = (time_finish - time_now) / 1000;
                    }
                }
                else
                {
                    timer[i] = (time_finish - time_start) / 1000;
                }
 
                scroll_go[i].SetActive(true);
            }
            Debug.Log("/n Rarity: " + scroll_type_num);

        }
    }

    public void OpenScroll(int scroll_num)
    {
        if (scroll_num == active_scroll_index)
        {
            if (timer[active_scroll_index] <= 0)
            {
                switch (scroll_rarity[active_scroll_index])
                {
                    case rarity.r_common:
                        scroll_go[active_scroll_index].SetActive(false);
                        break;
                    case rarity.r_uncommon:
                        scroll_go[active_scroll_index].SetActive(false);
                        break;
                    case rarity.r_rare:
                        scroll_go[active_scroll_index].SetActive(false);
                        break;
                }
            }
        }
    }

    public void CreateScroll(int type, int scroll_num)
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("DB_ADD_SCROLL")
            .SetEventAttribute("TYPE", type)
            .SetEventAttribute("SCROLL_NUM", scroll_num)
            .Send((response) =>
            {
                if (response.HasErrors)
                {
                    Debug.LogError("Scroll not created \n" + response.Errors.JSON);
                }
                else
                {
                    new GameSparks.Api.Requests.LogEventRequest()
                                .SetEventKey("GET_SCROLLS")
                                .Send((scroll_response) =>
                                {
                                    if (!scroll_response.HasErrors)
                                    {
                                        Debug.Log("Scrolls found");

                                        GameSparks.Core.GSData data = scroll_response.ScriptData.GetGSData("player_scrolls");
                                        GameSparks.Core.GSData time = scroll_response.ScriptData.GetGSData("time_now");
                                        SetScroll(data, (long)time.GetLong("current_time"));
                                    }
                                    else
                                    {
                                        Debug.Log("Error finding scrolls");
                                    }
                                });
                }
            });
    }
}
