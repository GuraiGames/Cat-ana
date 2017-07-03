using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private NavigationEntity navigation_entity = null;
    private PlayerShadow _shadow = null;

    private string _network_id = "";
    private bool _is_player = false;
    private bool stealth = false;

	void Start ()
    {
        navigation_entity = gameObject.GetComponent<NavigationEntity>();
	}

    public void SetInitialPlayerInfo(Vector3 pos, string network_id, bool is_player, GameObject shadow)
    {
        gameObject.transform.position = pos;
        _network_id = network_id;
        _is_player = is_player;
        _shadow = shadow.GetComponent<PlayerShadow>();
        gameObject.GetComponent<PlayerShadow>().enabled = false;
    }

    public string GetNetworkId() { return _network_id; }

    public bool IsPlayer() { return _is_player; }

    public NavigationEntity GetNavigationEntity() { return navigation_entity; }

    public void Desappear()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public void Appear()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    public bool OnStealth() { return stealth; }

    public void SetStealth(bool set)
    {
        stealth = set;
    }

    public PlayerShadow GetPlayerShadow()
    {
        return _shadow.GetComponent<PlayerShadow>();
    }
}
