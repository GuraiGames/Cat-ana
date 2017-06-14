using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterBttn : MonoBehaviour {

    public Text user_name, display_name, password, confirm_password;
    public GameObject panel;


    [SerializeField]
    private Text error_message; 

    public void RegisterBttnClick()
    {

        if (user_name == confirm_password)
        {
            new GameSparks.Api.Requests.RegistrationRequest().SetDisplayName(display_name.text)
           .SetPassword(password.text)
           .SetUserName(user_name.text)
           .Send((response) =>
           { 
                if (response.HasErrors)
               {
                   panel.SetActive(true);
                   error_message = panel.GetComponentInChildren<Text>();               

                   error_message.text = "dasd";

               }
               else
               {
                   ChangeScene a = new ChangeScene();
                   a.SetScene("Login");
                   a.ChangeToScene();
               }
           });

        }
        else
        {
            panel.SetActive(true);

            error_message = panel.GetComponentInChildren<Text>();

            error_message.text = "Passwords not matching.";
        }
       
    }
}
