using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Requests;

public class ChangeNameBttn : MonoBehaviour {

    public Text display_name;
    public GameObject panel, old_password, new_password, main_window, settings_window;

    public void ChangeNameBttnClick()
    {
        new ChangeUserDetailsRequest().SetDisplayName(display_name.text)
            .SetNewPassword(new_password.GetComponent<InputField>().text)
            .SetOldPassword(old_password.GetComponent<InputField>().text)
            .Send((response) => {
                if (!response.HasErrors)
                    {
                        main_window.SetActive(false);
                        settings_window.SetActive(true);
                    }
                    else
                    {
                        panel.SetActive(true);
                    }
            });
    }
}
