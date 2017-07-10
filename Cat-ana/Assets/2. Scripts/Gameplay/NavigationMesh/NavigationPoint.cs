using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationPoint : MonoBehaviour
{
    [HideInInspector]
    public NavigationMap nav_map = null;

    [SerializeField]
    private List<GameObject> neighbours = new List<GameObject>();

    [SerializeField]
    public bool is_spawn_point;

    private float tile_size = 2;

    private BoxCollider coll = null;

    private List<Player> players_on_tile = new List<Player>();

    // Managers
    private MatchManager match_manager = null;

    private GameManager game_manager = null;

    private EventManager event_manager = null;

    Player client_player = null;
    
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

    public Vector3 AskForPlayerPosition(GameObject player)
    {
        Vector3 ret = new Vector3(0, 0, 0);

        List<GameObject> players = match_manager.GetPlayers();
        Player player_script = null;

        for(int i = 0; i < players.Count; i++)
        {
            if(players[i] == player)
            {
                player_script = player.GetComponent<Player>();
                break;
            }
        }

        if (player_script != null)
        {
            if(!players_on_tile.Contains(player_script))
                 players_on_tile.Add(player_script);

            ret = GetTilePartitionPosition(players_on_tile.Count);
        }

        return ret;
    }

    public void PlayerLeavesTile(GameObject player)
    {
        List<GameObject> players = match_manager.GetPlayers();
        Player player_script = null;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == player)
            {
                player_script = player.GetComponent<Player>();
                break;
            }
        }

        if (player_script != null)
        {
            players_on_tile.Remove(player_script);
        }
    }

    private Vector3 GetTilePartitionPosition(int index)
    {
        Vector3 ret = new Vector3(-1, -1, -1);

        if (index > 4 || index <= 0)
        {
            switch(index)
            {
                case 1:
                    ret = new Vector3(gameObject.transform.position.x - (tile_size / 3), gameObject.transform.position.y, gameObject.transform.position.z + (tile_size / 3));
                    break;
                case 2:
                    ret = new Vector3(gameObject.transform.position.x + (tile_size / 3), gameObject.transform.position.y, gameObject.transform.position.z + (tile_size / 3));
                    break;
                case 3:
                    ret = new Vector3(gameObject.transform.position.x - (tile_size / 3), gameObject.transform.position.y, gameObject.transform.position.z - (tile_size / 3));
                    break;
                case 4:
                    ret = new Vector3(gameObject.transform.position.x + (tile_size / 3), gameObject.transform.position.y, gameObject.transform.position.z - (tile_size / 3));
                    break;
            }
        }

        return ret;
    }

    private void OnMouseDown()
    {
        if (client_player == null)
            client_player = match_manager.GetClientPlayer().GetComponent<Player>();

        // Check if point is on range
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
            Vector2 pos = nav_map.WorldPointToGrid(gameObject);

            match_manager.TileClicked((int)pos.x, (int)pos.y);
        }
    }
}
