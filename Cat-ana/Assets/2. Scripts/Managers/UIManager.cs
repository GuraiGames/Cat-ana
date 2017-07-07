using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField]
    private GameObject login_register;

    [SerializeField]
    private GameObject register;

    [SerializeField]
    private GameObject lobby;

    [SerializeField]
    private GameObject change_name_settings;

    [SerializeField]
    private GameObject settings;

    [SerializeField]
    private GameObject error;

    [SerializeField]
    private GameObject scroll_ui;

    [SerializeField]
    private GameObject global_ui;

    [SerializeField]
    private GameObject scrollable_panel;

    void Awake ()
    {
        AddWindow(login_register, "login_register");
        AddWindow(register, "register");
        AddWindow(lobby, "lobby");
        AddWindow(change_name_settings, "change_name_settings");
        AddWindow(settings, "settings");
        AddWindow(error, "error");
        AddWindow(scroll_ui, "scroll_ui");
        AddWindow(global_ui, "global_ui");
    }

    void Start()
    {
        SetEnableWindow("login_register", true);
    }

    private void AddWindow(GameObject canvas, string name)
    { 
        Window w = new Window(canvas, name);
        w.SetEnabled(false);
        windows.Add(w);
    }

    public GameObject GetWindowCanvas(string name)
    {
        GameObject ret = null;

        for (int i = 0; i < windows.Count; i++)
        {
            if (windows[i].GetName() == name)
            {
                ret = windows[i].GetCanvas();
                break;
            }
        }

        return ret;
    }
	
    public void SetEnableWindow(string name, bool set)
    {
        for(int i = 0; i<windows.Count; i++)
        {
            if(windows[i].GetName() == name)
            {
                windows[i].SetEnabled(set);
                break;
            }
        }
    }

    public void EnableWindow(string name)
    {
        for (int i = 0; i < windows.Count; i++)
        {
            if (windows[i].GetName() == name)
            {
                windows[i].SetEnabled(true);
                break;
            }
        }
    }

    public void DisableWindow(string name)
    {
        for (int i = 0; i < windows.Count; i++)
        {
            if (windows[i].GetName() == name)
            {
                windows[i].SetEnabled(false);
                break;
            }
        }
    }

    public void ToggleWindow(string name)
    {

        for (int i = 0; i < windows.Count; i++)
        {
            if (windows[i].GetName() == name)
            {
                if (windows[i].GetCanvas().activeSelf)
                {
                    windows[i].SetEnabled(false);
                }
                else
                {
                    windows[i].SetEnabled(true);
                }
                break;
            }
        }
    }

    class Window
    {
        public Window(GameObject _canvas, string _name)
        {
            canvas = _canvas;
            name = _name;
        }

        public void SetEnabled(bool set)
        {
            canvas.SetActive(set);
        }

        public string GetName()
        {
            return name;
        }

        public GameObject GetCanvas()
        {
            return canvas;
        }

         GameObject canvas = null;
         string name = "";
    }

    private List<Window> windows = new List<Window>();

    public void ToggleScroll()
    {
        scrollable_panel.GetComponent<ScrollRect>().enabled = !scrollable_panel.GetComponent<ScrollRect>().enabled;
    }

}
