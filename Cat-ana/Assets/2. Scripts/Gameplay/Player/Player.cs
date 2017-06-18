using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private NavigationEntity navigation_entity = null;

    private int _network_id = 0;

    private bool _is_player = false;

	// Use this for initialization
	void Start ()
    {
        navigation_entity = gameObject.GetComponent<NavigationEntity>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetInitialPlayerInfo(Vector3 pos, int network_id, bool is_player)
    {
        gameObject.transform.position = pos;
        _network_id = network_id;
        _is_player = is_player;
    }

    public int GetNetworkId() { return _network_id; }

    public bool IsPlayer() { return _is_player; }
}
