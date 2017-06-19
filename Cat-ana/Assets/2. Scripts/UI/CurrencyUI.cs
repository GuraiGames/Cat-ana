using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks;

public class CurrencyUI : MonoBehaviour {

    [SerializeField]
    private Text coin;

    [SerializeField]
    private Text gem;

    private int coins = -1;
    private int gems = -1;

    // Use this for initialization
    void Start () {
        RefreshCurrency();
        coin.text = coins.ToString();
        gem.text = gems.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        coin.text = coins.ToString();
        gem.text = gems.ToString();

        if (Input.GetKeyDown(KeyCode.A))
        {
            ModifyCurrency(50, 1, true);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ModifyCurrency(25, 1, false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ModifyCurrency(50, 2, true);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            ModifyCurrency(25, 2, false);
        }
    }

    public void RefreshCurrency()
    {
        new GameSparks.Api.Requests.AccountDetailsRequest()
            .Send((request) =>
            {
                coins = (int)request.Currency1;
                gems = (int)request.Currency2;
            });
    }

    public void ModifyCurrency(int amount, int reference, bool add_or_spend) // reference 1 == coins, reference 2 == gems // add_or_spend == true -> ADD_COINS; add_or_spend == false -> SPEND_COINS
    {
        if (amount != 0)
        {
            string event_key = "";
            if (add_or_spend)
            {
                event_key = "ADD_COINS";
            }
            else
            {
                event_key = "SPEND_COINS";
            }

            new GameSparks.Api.Requests.LogEventRequest().SetEventKey(event_key)
                .SetEventAttribute("AMOUNT", amount)
                .SetEventAttribute("CURRENCY_REF", reference)
                .Send((response) =>
                {
                    if (!response.HasErrors)
                    {
                        RefreshCurrency();
                    }
                });
        }
    }
}
