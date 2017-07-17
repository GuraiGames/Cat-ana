using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeMenu : MonoBehaviour {

    [SerializeField]
    private Button skins_button;

    int tot_skins_amount = 0; 

    [SerializeField]
    private Button weapons_button;

    int tot_weapons_amount = 0;

    [SerializeField]
    private Button cards_button;

    int tot_cards_amount = 0;

    [SerializeField]
    private GameObject background_panel;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

   public void SkinsButtonPressed()
    {

        Image img = GameObject.Find("Background Panel").GetComponent<Image>();
        img.color = UnityEngine.Color.red; 

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

                // Here we load all the skins locked -----
                      
                GameSparks.Core.GSData scriptData = response.ScriptData;
                GameSparks.Core.GSEnumerable<GameSparks.Api.Responses.ListVirtualGoodsResponse._VirtualGood> virtualGoods = response.VirtualGoods;

                foreach (var skin in virtualGoods)
                {
                    tot_skins_amount++;                   
                }

                for(int i = 0; i < tot_skins_amount; i++)
                {

                }

                // -----

                new GameSparks.Api.Requests.AccountDetailsRequest().Send((details_response) =>
                {
                    if(details_response.HasErrors)
                    {
                        Debug.Log("too bad :(");
                    }
                    else
                    {
                        // Here we "unlock" the ones that the player has -----

                        Debug.Log("I've got the info muahahaha");

                        foreach(var skin in details_response.VirtualGoods.BaseData)
                        {
                            string skin_code = skin.Key.ToString();

                            Debug.Log(skin_code);
                        }

                        // -----

                        
                    }
                });
            }

          
        });
    }

    public void WeaponsButtonPressed()
    {

        Image img = GameObject.Find("Background Panel").GetComponent<Image>();
        img.color = UnityEngine.Color.green;

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

    public void CardsButtonPressed()
    {
        Image img = GameObject.Find("Background Panel").GetComponent<Image>();
        img.color = UnityEngine.Color.yellow;

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
