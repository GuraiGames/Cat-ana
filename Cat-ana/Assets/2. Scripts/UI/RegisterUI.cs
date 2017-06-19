using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterUI : MonoBehaviour
{
    private GameManager game_manager;

    [SerializeField]
    private int fields_min_lenght;

    [SerializeField]
    private int fields_max_lenght;

    [SerializeField]
    private InputField username;

    [SerializeField]
    private InputField display_name;

    [SerializeField]
    private InputField password;

    [SerializeField]
    private InputField confirm_password;

    [SerializeField]
    private GameObject error_panel;

    [SerializeField]
    private GameObject[] to_active;

    private Text curr_error_text;

    void OnEnable()
    {
        game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        curr_error_text = error_panel.GetComponentInChildren<Text>();
        error_panel.SetActive(false);
    }

    void SetErrorText(string text)
    {
        error_panel.gameObject.SetActive(true);

        for (int i = 0; i < to_active.Length; i++)
        {
            to_active[i].SetActive(false);
        }


        curr_error_text.text = text;
    }

    void CleanErrorText()
    {
        error_panel.gameObject.SetActive(false);

        for (int i = 0; i < to_active.Length; i++)
        {
            to_active[i].SetActive(true);
        }

        curr_error_text.text = "error text";
    }

    void ClearFields()
    {
        username.text = "";
        display_name.text = "";
        password.text = "";
    }

    public void ButtonPressed()
    {
        Debug.Log("Register attemt. User: " + username.text + "Pass: " + password.text);

        if (username.text == "" || display_name.text == "" && password.text == "")
        {
            Debug.Log("Register error: There are empty fields");
            SetErrorText("Register error: There are empty fields");
            return;
        }

        if(password.text.Length < fields_min_lenght || username.text.Length < fields_min_lenght || display_name.text.Length < fields_min_lenght)
        {
            Debug.Log("Register error: Fields should be longuer than " + fields_min_lenght + " words");
            SetErrorText("Register error: Fields should be longuer than " + fields_min_lenght + " words");
            return;
        }

        if(password.text.Length > fields_max_lenght || username.text.Length > fields_max_lenght || display_name.text.Length > fields_max_lenght)
        {
            Debug.Log("Register error: Fields should be less than " + fields_max_lenght + " words");
            SetErrorText("Register error: Fields should be less than " + fields_max_lenght + " words");
            return;
        }

        if (confirm_password.text != password.text)
        {
            Debug.Log("Register error: Passwords not matching");
            SetErrorText("Register error: Passwords not matching");
            return;
        }

        new GameSparks.Api.Requests.RegistrationRequest()
        .SetDisplayName(display_name.text)
        .SetPassword(password.text)
        .SetUserName(username.text)
        .Send((response) =>
        {

          

            if (response.HasErrors)
            {
                Debug.Log("Register error: " + response.Errors.JSON.ToString());

                if(response.Errors.JSON.ToString() == "TAKEN")
                {
                    SetErrorText("Register error: Username already taken");
                }

                SetErrorText("Register error: " + response.Errors.JSON.ToString());
            }
            else
            {
                Debug.Log("Register succes");
                game_manager.GetUIManager().EnableWindow("login_register");
                game_manager.GetUIManager().DisableWindow("register");

                CleanErrorText();
                ClearFields();
            }
        }
        );
    }
}
