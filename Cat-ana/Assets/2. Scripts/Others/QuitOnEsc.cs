using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitOnEsc : MonoBehaviour 
{
	// Private functions
	private void Start () 
	{
		
	}
	
	private void Update () 
	{
        if (Input.GetKey("escape"))
            Application.Quit();
    }
	
	
	// Public functions
	
	// Public variables
	
	// Private variables
	
}
