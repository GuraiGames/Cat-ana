using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    [SerializeField]
    private GameObject login_register;

    [SerializeField]
    private GameObject register;

    [SerializeField]
    private GameObject loby;


	void Awake ()
    {
        AddWindow(login_register, "login_register");

        AddWindow(register, "register");

        AddWindow(loby, "lobby");
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
}
