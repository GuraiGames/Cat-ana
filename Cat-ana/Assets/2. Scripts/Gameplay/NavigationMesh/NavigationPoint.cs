using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationPoint : MonoBehaviour {

    [SerializeField]
    private float tile_size;

    private BoxCollider coll = null;

	// Use this for initialization
	void Start ()
    {
        coll = gameObject.AddComponent<BoxCollider>();

        coll.size = new Vector3(tile_size, 0.1f, tile_size);
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.DrawLine(new Vector3(transform.position.x + (tile_size / 2), transform.position.y, transform.position.z + (tile_size / 2)),
            new Vector3(transform.position.x + (tile_size / 2), transform.position.y, transform.position.z - (tile_size / 2)), Color.green);

        Debug.DrawLine(new Vector3(transform.position.x - (tile_size / 2), transform.position.y, transform.position.z + (tile_size / 2)),
            new Vector3(transform.position.x - (tile_size / 2), transform.position.y, transform.position.z - (tile_size / 2)), Color.green);

        Debug.DrawLine(new Vector3(transform.position.x + (tile_size / 2), transform.position.y, transform.position.z - (tile_size / 2)),
            new Vector3(transform.position.x - (tile_size / 2), transform.position.y, transform.position.z - (tile_size / 2)), Color.green);

        Debug.DrawLine(new Vector3(transform.position.x + (tile_size / 2), transform.position.y, transform.position.z + (tile_size / 2)),
            new Vector3(transform.position.x - (tile_size / 2), transform.position.y, transform.position.z + (tile_size / 2)), Color.green);

    }

    void OnMouseDown()
    {

    }
}
