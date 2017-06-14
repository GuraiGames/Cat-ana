using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject game_sparks_manager;

    [SerializeField]
    private GameObject ui_manager = null;

    [SerializeField]
    private GameObject network_manager = null;

    public GameObject GetGameSparksManager()
    { 
        return game_sparks_manager;
    }

    public UIManager GetUIManager()
    {
        return ui_manager.gameObject.GetComponent<UIManager>(); ;
    }

    public NetworkManager GetNetworkManager()
    {
        return network_manager.gameObject.GetComponent<NetworkManager>();
    }
}
