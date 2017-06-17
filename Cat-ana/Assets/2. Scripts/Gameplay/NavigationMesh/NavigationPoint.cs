using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationPoint : MonoBehaviour
{
    private float tile_size = 2;

    public NavigationMap nav_map = null;

    private BoxCollider coll = null;

    [SerializeField]
    private List<GameObject> neighbours = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        coll = gameObject.AddComponent<BoxCollider>();
        coll.isTrigger = true;

        coll.size = new Vector3(tile_size, 0.1f, tile_size);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(new Vector3(transform.position.x + (tile_size / 2), transform.position.y, transform.position.z + (tile_size / 2)),
            new Vector3(transform.position.x + (tile_size / 2), transform.position.y, transform.position.z - (tile_size / 2)), Color.green);

        Debug.DrawLine(new Vector3(transform.position.x - (tile_size / 2), transform.position.y, transform.position.z + (tile_size / 2)),
            new Vector3(transform.position.x - (tile_size / 2), transform.position.y, transform.position.z - (tile_size / 2)), Color.green);

        Debug.DrawLine(new Vector3(transform.position.x + (tile_size / 2), transform.position.y, transform.position.z - (tile_size / 2)),
            new Vector3(transform.position.x - (tile_size / 2), transform.position.y, transform.position.z - (tile_size / 2)), Color.green);

        Debug.DrawLine(new Vector3(transform.position.x + (tile_size / 2), transform.position.y, transform.position.z + (tile_size / 2)),
            new Vector3(transform.position.x - (tile_size / 2), transform.position.y, transform.position.z + (tile_size / 2)), Color.green);

        List<GameObject> neig = GetNeighbours();

        for (int i = 0; i < neig.Count; i++)
        {
            NavigationPoint np = neig[i].GetComponent<NavigationPoint>();

            if (!np.IsNeighbour(gameObject))
                return;

            Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, transform.position.z),
                new Vector3(neig[i].transform.position.x, neig[i].transform.position.y, neig[i].transform.position.z), Color.green);
        }
    }

    public bool IsNeighbour(GameObject go)
    {
        if (GetNeighbours().Contains(go))
            return true;
        return false;
    }

    public List<GameObject> GetNeighbours()
    {
        List<GameObject> ret = new List<GameObject>();

        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours[i] != null)
            {
                ret.Add(neighbours[i]);
            }
        }

        return ret;
    }

    public void AddBeighbour(GameObject neighbour)
    {
        neighbours.Add(neighbour);
    }

    private void OnMouseDown()
    {
        nav_map.target_point = gameObject;
    }

}
