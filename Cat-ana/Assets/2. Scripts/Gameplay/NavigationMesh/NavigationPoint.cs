using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationPoint : MonoBehaviour
{
    private float tile_size = 2;

    [HideInInspector]
    public NavigationMap nav_map = null;
    public MatchManager match_manager = null;
    Player client_player = null;

    private BoxCollider coll = null;

    [SerializeField]
    private List<GameObject> neighbours = new List<GameObject>();

    [SerializeField]
    public bool is_spawn_point;

    // Use this for initialization
    void Start()
    {
        match_manager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();

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
                new Vector3(neig[i].transform.position.x, neig[i].transform.position.y, neig[i].transform.position.z), Color.red);
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
        if (match_manager.GetTurnInfo().turn == MatchManager.turn_type.strategy)
        {
            if (client_player == null)
                client_player = match_manager.GetClientPlayer().GetComponent<Player>();

            GameObject player_pos = client_player.GetNavigationEntity().GetClosestNavPoint();

            List<GameObject> points = nav_map.GetPointsFromExpansion(1, player_pos);

            bool is_range_point = false;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i] == gameObject)
                {
                    is_range_point = true;
                    break;
                }
            }

            if (is_range_point)
            {
                string id = client_player.GetNetworkId();
                int pos_x = (int)nav_map.WorldPointToGrid(gameObject).x;
                int pos_y = (int)nav_map.WorldPointToGrid(gameObject).y;

                match_manager.SendPlayerPos(id, pos_x, pos_y, 0, 0, false, false);
            }
        }

        //client_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //GameObject player_pos = client_player.GetNavigationEntity().GetClosestNavPoint();

        //List<GameObject> points = nav_map.GetPointsFromExpansion(2, player_pos);

        //bool is_range_point = false;
        //for (int i = 0; i < points.Count; i++)
        //{
        //    if (points[i] == gameObject)
        //    {
        //        is_range_point = true;
        //        break;
        //    }
        //}

        //if (is_range_point)
        //{
        //    string id = client_player.GetNetworkId();
        //    int pos_x = (int)client_player.GetNavigationEntity().GetGridPos().x;
        //    int pos_y = (int)client_player.GetNavigationEntity().GetGridPos().y;

        //    match_manager.SendPlayerPos(id, pos_x, pos_y, 0, 0, false, false);
        //}
    }

}
