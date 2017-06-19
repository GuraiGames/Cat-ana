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
    public int scroll_to_active = -1;

    [SerializeField]
    private GameObject error_panel;
    public ErrorTimer error_timer;

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
            CreateScroll(0, 0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            CreateScroll(2, 1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CreateScroll(2, 2);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            CreateScroll(3, 3);
        }

        for (int i = 0; i < scroll_go.Length; i++)
        {
            if (scroll_rarity[i] == rarity.r_null)
            {
                scroll_go[i].SetActive(false);
            }
            if (scroll_go[i].activeSelf)
            {
                if (timer[i] > 0)
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
                else
                {
                    timer_txt[i].text = "Ready";
                }
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
                scroll_go[i].SetActive(false);
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
                    switch (scroll_rarity[i])
                    {
                        case rarity.r_common:
                            timer[i] = 3600 * 2;
                            break;
                        case rarity.r_uncommon:
                            timer[i] = 3600 * 6;
                            break;
                        case rarity.r_rare:
                            timer[i] = 3600 * 24;
                            break;
                    }
                }
 
                scroll_go[i].SetActive(true);
            }
            Debug.Log("/n Rarity: " + scroll_type_num);

        }
    }

    public void OpenScroll(int scroll_num)
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("OPEN_SCROLL")
            .SetEventAttribute("SCROLL_NUM", scroll_num)
            .Send((response) =>
            {
                if (response.HasErrors)
                {
                    Debug.Log("Scroll not found");
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

    public void CreateScroll(int type, int scroll_num)
    {
        string type_txt = "";

        switch (type)
        {
            case 0:
                type_txt = "r_null";
                break;
            case 1:
                type_txt = "r_common";
                break;
            case 2:
                type_txt = "r_uncommon";
                break;
            case 3:
                type_txt = "r_rare";
                break;
        }

        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("DB_ADD_SCROLL")
            .SetEventAttribute("TYPE", type_txt)
            .SetEventAttribute("SCROLL_NUM", scroll_num)
            .Send((response) =>
            {
                if (response.HasErrors)
                {
                    Debug.LogError("Scroll not created \n" + response.Errors.JSON);
                }
                else
                {
                    OpenScroll(scroll_num);
                }
            });
    }

    public void SetToActive(int to_active)
    {
        scroll_to_active = to_active;
    }

    public void SetActiveScroll()
    {
        if (active_scroll_index == -1)
        {
            if (scroll_rarity[scroll_to_active] != rarity.r_null)
            {
                new GameSparks.Api.Requests.LogEventRequest().SetEventKey("DB_SET_ACTIVE_SCROLL")
                    .SetEventAttribute("SCROLL_NUM", scroll_to_active)
                    .Send((response) =>
                    {
                        if (!response.HasErrors)
                        {
                            active_scroll_index = scroll_to_active;
                        }
                    });
            }
        }
        else
        {
            error_panel.SetActive(true);
            error_timer.RestartTimer(2);
        }
    }
}
