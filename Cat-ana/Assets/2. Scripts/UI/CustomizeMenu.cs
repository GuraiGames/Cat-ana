using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeMenu : MonoBehaviour {

    [SerializeField]
    private Button skins_button;

    int tot_skins_amount = 10; 

    [SerializeField]
    private Button weapons_button;

    int tot_weapons_amount = 10;

    [SerializeField]
    private Button cards_button;

    int tot_cards_amount = 10;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

   public void SkinsButtonPressed()
    {

        List<string> tags = new List<string>();

        tags.Add("SKIN"); 
        
        new GameSparks.Api.Requests.ListVirtualGoodsRequest()
        .SetTags(tags)
        .Send((response) => {

            if(response.HasErrors)
            {
                Debug.Log("Error: skin list can not be loaded");
            }

            else
            {
                Debug.Log("Skin list loaded succesfully");
            }

            //GSData scriptData = response.ScriptData;
            //GSEnumerable<var> virtualGoods = response.VirtualGoods;
        });
    }

    void WeaponsButtonPressed()
    {
        List<string> tags = new List<string>();

        tags.Add("WEAPON");

        new GameSparks.Api.Requests.ListVirtualGoodsRequest()
        .SetTags(tags)
        .Send((response) => {

            if (response.HasErrors)
            {
                Debug.Log("Error: skin list can not be loaded");
            }

            else
            {
                Debug.Log("Skin list loaded succesfully");
            }

            //GSData scriptData = response.ScriptData;
            //GSEnumerable<var> virtualGoods = response.VirtualGoods;
        });
    }

    void CardsButtonPressed()
    {
        List<string> tags = new List<string>();

        tags.Add("CARD");

        new GameSparks.Api.Requests.ListVirtualGoodsRequest()
        .SetTags(tags)
        .Send((response) => {

            if (response.HasErrors)
            {
                Debug.Log("Error: skin list can not be loaded");
            }

            else
            {
                Debug.Log("Skin list loaded succesfully");
            }

            //GSData scriptData = response.ScriptData;
            //GSEnumerable<var> virtualGoods = response.VirtualGoods;
        });
    }
}
