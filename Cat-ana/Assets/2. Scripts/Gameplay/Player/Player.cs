using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private NavigationEntity navigation_entity = null;

	// Use this for initialization
	void Start ()
    {
        navigation_entity = gameObject.GetComponent<NavigationEntity>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
