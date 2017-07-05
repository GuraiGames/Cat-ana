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
            }
        }
	}

    public GameObject GetClosestNavPoint()
    {
        GameObject ret = null;

        List<GameObject> points = nav_map.GetPoints();

        float closest_distance = 0;

        if (points.Count > 0)
        {
            closest_distance = Vector3.Distance(gameObject.transform.position, points[0].transform.position);
            ret = points[0];
        }

        for (int i = 0; i < points.Count; i++)
        {
            if(Vector3.Distance(gameObject.transform.position, points[i].transform.position) < closest_distance)
            {
                closest_distance = Vector3.Distance(gameObject.transform.position, points[i].transform.position);
                ret = points[i];
            }
        }

        return ret;
    }

    List<GameObject> GetWalkableTiles()
    {
        List<GameObject> ret = new List<GameObject>();

        ret = nav_map.GetPointsFromExpansion(tile_distance + 1, GetClosestNavPoint());

        return ret;
    }

    public void MoveTo(GameObject nav_point_target)
    {
        if (is_moving)
            return;

        path = nav_map.GetPath(GetClosestNavPoint(), nav_point_target);
        target_point = nav_point_target;
        is_moving = true;
    }

    public void MoveTo(int grid_x, int grid_y)
    {
        MoveTo(nav_map.GridToWorldPoint(grid_x, grid_y));
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
}
