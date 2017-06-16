using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollSpawn : MonoBehaviour {

    public GameObject[] scroll_go;

    [SerializeField]
    private float distance_x = 0;

    [SerializeField]
    private float distance_y = 0;

    // Use this for initialization
    void Start () {
       for (int i = 0; i < scroll_go.Length; i++)
        {
            scroll_go[i].SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.A))
        {
            for (int i = 0; i < scroll_go.Length; i++)
            {
                if (!scroll_go[i].activeSelf)
                {
                    scroll_go[i].SetActive(true);
                    break;
                }
            }
        }
	}
}
