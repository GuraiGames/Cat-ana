using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeNameUI : MonoBehaviour {

    private GameManager game_manager;

    [SerializeField]
    private InputField display_name;

    [SerializeField]
    private InputField old_password;

    [SerializeField]
    private InputField new_password;

    void Start()
    {
        game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void ButtonPressed()
    {
        string response = game_manager.GetNetworkManager().ChangeName(display_name.text, old_password.text, new_password.text);

        if (response == "SUCCES")
        {
            game_manager.GetUIManager().DisableWindow("change_name_settings");
            game_manager.GetUIManager().EnableWindow("settings");
        }
    }
}
