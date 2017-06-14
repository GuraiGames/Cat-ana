using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Requests;

public class ChangeNameBttn : MonoBehaviour {

    public Text display_name, old_password, new_password;
    public GameObject panel;
    public GameObject main_window;

    public void ChangeNameBttnClick()
    {
        new ChangeUserDetailsRequest().SetDisplayName(display_name.text)
            .SetNewPassword(new_password.text)
            .SetOldPassword(old_password.text)
            .Send((response) => {
                if (!response.HasErrors)
                    {
                        main_window.SetActive(false);
                    }
                    else
                    {
                        panel.SetActive(true);
                    }
            });
    }
}
