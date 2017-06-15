using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameSparksUnity game_sparks_manager;

    [SerializeField]
    private GameSparksRTUnity game_sparks_manager_rt;

    [SerializeField]
    private GameObject ui_manager = null;

    [SerializeField]
    private GameObject network_manager = null;

    public GameSparksUnity GetGameSparksManager()
    { 
        return game_sparks_manager;
    }

    public GameSparksRTUnity GetGameSparksRTManager()
    {
        return game_sparks_manager_rt;
    }

    public UIManager GetUIManager()
    {
        return ui_manager.gameObject.GetComponent<UIManager>();
    }

    public NetworkManager GetNetworkManager()
    {
        return network_manager.gameObject.GetComponent<NetworkManager>();
    }
}
