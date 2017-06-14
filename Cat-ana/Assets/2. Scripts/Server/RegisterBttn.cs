using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterBttn : MonoBehaviour {

    public Text user_name, display_name;
    public GameObject panel, password;

    [SerializeField]
    private Text error_message;

    public void RegisterBttnClick()
    {
        new GameSparks.Api.Requests.RegistrationRequest().SetDisplayName(display_name.text)
           .SetPassword(password.GetComponent<InputField>().text)
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
}
