using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NavigationEntity : MonoBehaviour
{
    GameManager game_manager = null;
    EventManager event_manager = null;

    // Map
    private GameObject map;
    private NavigationMap nav_map = null;

    // Movement
    [SerializeField]
    private float speed;
    GameObject target_point = null;
    private List<Vector3> path = new List<Vector3>();
    private bool is_moving = false;

    // TileDistance
    private int tile_distance = 0;
    private List<GameObject> walkable_points = new List<GameObject>();

    // Use this for initialization
    void Start ()
    {
        game_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        event_manager = game_manager.GetEventManager();
        map = GameObject.FindGameObjectWithTag("Map");
        nav_map = map.gameObject.GetComponent<NavigationMap>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(is_moving)
        {
            if (Vector3.Distance(gameObject.transform.position, target_point.transform.position) < 0.3f)
            {
                is_moving = false;
                target_point = null;
            }
            else if (Vector3.Distance(gameObject.transform.position, path[path.Count-1]) < 0.2f)
            {
                path.RemoveAt(path.Count - 1);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, path[path.Count - 1], speed * Time.deltaTime);
                gameObject.transform.eulerAngles = new Vector3(gameObject.transform.position.x, AngleFromTwoPoints(new Vector2(transform.position.z, transform.position.x), new Vector2(path[path.Count - 1].x, path[path.Count - 1].z)) + 75, gameObject.transform.position.z);
            }
        }
	}

    public NavigationMap GetNavigationMap()
    {
        return nav_map;
    }

    public GameObject GetClosestNavPoint()
    {
        GameObject ret = null;

        ret = nav_map.GetClosestNavPoint(transform.position);

        return ret;
    }

    List<GameObject> GetWalkableTiles()
    {
        List<GameObject> ret = new List<GameObject>();

        ret = nav_map.GetPointsFromExpansion(tile_distance + 1, GetClosestNavPoint());

        return ret;
    }

    public bool MoveTo(GameObject nav_point_target)
    {
        if (is_moving)
            return false;

        GameObject curr_pos = GetClosestNavPoint();

        if (curr_pos == nav_point_target)
            return false;

        path = nav_map.GetPath(curr_pos, nav_point_target);
        target_point = nav_point_target;
        is_moving = true;

        return true;
    }

    public bool MoveTo(int grid_x, int grid_y)
    {
        return MoveTo(nav_map.GridToWorldPoint(grid_x, grid_y));
    }

    public bool IsMoving() { return is_moving; }

    public GameObject GetPos()
    {
        return GetClosestNavPoint();
    }

    public Vector2 GetGridPos()
    {
        Vector2 ret = new Vector2();

        ret = nav_map.WorldPointToGrid(GetPos());

        return ret;
    }

    public NavigationMap GetNavMap() { return nav_map; }

    public List<Vector3> GetCurrentPath()
    {
        return path;
    }

    float AngleFromTwoPoints(Vector2 p1, Vector2 p2)
    {
        return Mathf.Atan2(p2.y - p1.y, p2.x - p1.x) * Mathf.Rad2Deg;
    }
}
