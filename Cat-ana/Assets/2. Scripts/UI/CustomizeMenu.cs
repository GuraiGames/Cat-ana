using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeMenu : MonoBehaviour {

    [SerializeField]
    private Button skins_button;

    [SerializeField]
    private Button weapons_button;

    [SerializeField]
    private Button cards_button;

    [SerializeField]
    private GameObject background_panel;

    [SerializeField]
    private GameObject item_slot;

    public int tot_items_amount = 0;
    int unlocked_items = 0;
    float distance_between_slots = 5.5f;

    RectTransform rt;

    List<GameObject> displayed_slots = new List<GameObject>();

    Vector3 curr_pos = new Vector3(0.2f, 0, 0);

    // Use this for initialization
    void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ButtonPressed(string tag)
    {
        if (tag == "CARD")
            Debug.Log("d");

        for (var i = 0; i < displayed_slots.Count; i++)
        {
            Destroy(displayed_slots[i]);
        }

        displayed_slots.Clear();

        Image img = GameObject.Find("Background Panel").GetComponent<Image>();
       

        if (tag == "SKIN")
            img.color = UnityEngine.Color.red;

        else if (tag == "WEAPON")
            img.color = UnityEngine.Color.blue;

        else if (tag == "CARD")
            img.color = UnityEngine.Color.yellow;


        List<string> tags = new List<string>();
        tags.Add(tag);

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

                // Here we load all the skins locked -----

                GameSparks.Core.GSData scriptData = response.ScriptData;
                GameSparks.Core.GSEnumerable<GameSparks.Api.Responses.ListVirtualGoodsResponse._VirtualGood> virtualGoods = response.VirtualGoods;

                foreach (var skin in virtualGoods)
                {
                    tot_items_amount++;
                }

                Debug.Log("TOTAL" + tag + "S: " + tot_items_amount);

                rt = GameObject.Find("Slot Panel").GetComponent<RectTransform>();
                rt.position = GameObject.Find("Slot Panel").GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

                rt.sizeDelta = new Vector2(1731 + (305 * tot_items_amount), 773);
                rt.localPosition = new Vector3(rt.localPosition.x + 245*tot_items_amount, rt.localPosition.y, rt.localPosition.y);

                for (int i = 0; i < tot_items_amount; i++)
                {        
                    
                
                    GameObject new_slot = Instantiate(item_slot, new Vector3(0,0,0), Quaternion.identity) as GameObject;
                    new_slot.transform.parent = GameObject.Find("Slot Panel").transform;
                    new_slot.transform.position = new Vector3(curr_pos.x, 0, 0); 

                    new_slot.transform.localScale = new Vector3(new_slot.transform.localScale.x / 110, new_slot.transform.localScale.y / 110, new_slot.transform.localScale.z / 110);

                    curr_pos.x += distance_between_slots;

                    displayed_slots.Add(new_slot); 
                }

                // -----

                new GameSparks.Api.Requests.AccountDetailsRequest().Send((details_response) =>
                {
                    if (details_response.HasErrors)
                    {
                        Debug.Log("too bad :(");
                    }
                    else
                    {
                        // Here we "unlock" the ones that the player has -----

                        Debug.Log("I've got the info muahahaha");

                        foreach (var weapon in details_response.VirtualGoods.BaseData)
                        {
                            string skin_code = weapon.Key.ToString();
                            unlocked_items++;

                            Debug.Log(skin_code);
                        }

                        

                        Debug.Log("UNLOCKED" + tag + "S: " + unlocked_items);

                        // -----


                    }

                    tags.Clear();
                    tot_items_amount = 0;
                    unlocked_items = 0;
                    curr_pos = new Vector3(0,0,0);

                });
            }


        });

       
    }


   public void SkinsButtonPressed()
    {

        
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
                Debug.Log("Error: weapons list can not be loaded");
            }

            else
            {
                Debug.Log("weapons list loaded succesfully");
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
                Debug.Log("Error: cards list can not be loaded");
            }

            else
            {
                Debug.Log("Card list loaded succesfully");
            }

            //GSData scriptData = response.ScriptData;
            //GSEnumerable<var> virtualGoods = response.VirtualGoods;
        });
    }
}
