using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationMap : MonoBehaviour
{
    [SerializeField]
    private int size_x;

    [SerializeField]
    private int size_y;

    [SerializeField]
    public GameObject nav_map_point;

    [SerializeField]
    public GameObject nav_map_points_parent;

    private List<GameObject> points = new List<GameObject>();
    private GameObject[,] grid;

    [SerializeField]
    private GameObject position_marker;
    private GameObject position_marker_instance = null;

    void Start()
    {
        // Get map points
        for (int i = 0; i < nav_map_points_parent.transform.childCount; i++)
        {
            GameObject current_point_go = nav_map_points_parent.transform.GetChild(i).gameObject;

            NavigationPoint np = current_point_go.GetComponent<NavigationPoint>();
            np.nav_map = this;

            List<GameObject> nb = np.GetNeighbours();

            for(int y = 0; y < nb.Count; y++)
            {
                NavigationPoint npn = nb[y].GetComponent<NavigationPoint>();

                if(!npn.IsNeighbour(current_point_go))
                {
                    npn.AddBeighbour(current_point_go);
                }
            }

            points.Add(current_point_go);
        }

        grid = TransformMapToGrid(GetPoints(), size_x, size_y);

    }

    public List<GameObject> GetPoints()
    {
        return points;
    }

    public GameObject[,] GetGrid()
    {
        return grid;
    }

    public bool IsPoint(GameObject go)
    {
        if (points.Contains(go))
            return true;
        return false;
    }

    public List<Vector3> GetPath(GameObject starting_point, GameObject ending_point)
    {
        List<Vector3> ret = new List<Vector3>();

        if (IsPoint(starting_point) && IsPoint(ending_point))
        {
            // Expand --------- (BFS)

            List<PPoint> frontier = new List<PPoint>();
            List<PPoint> memory_points = new List<PPoint>();

            PPoint point = new PPoint();
            point.point = starting_point;
            frontier.Add(point);

            while (frontier.Count > 0)
            {
                memory_points.Add(frontier[0]);

                if (frontier[0].point == ending_point)
                    break;

                NavigationPoint np = frontier[0].point.GetComponent<NavigationPoint>();

                List<GameObject> neigh = np.GetNeighbours();

                for (int i = 0; i < neigh.Count; i++)
                {
                    PPoint npoint = new PPoint();
                    npoint.parent = frontier[0].point;
                    npoint.point = neigh[i];

                    bool exists = false;
                    for (int y = 0; y < memory_points.Count; y++)
                    {
                        if (memory_points[y].point == npoint.point)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                        frontier.Add(npoint);
                }

                frontier.RemoveAt(0);
            }

            // ----------------

            // Backtrack ------

            PPoint curr_point = memory_points[memory_points.Count - 1];

            while (memory_points.Count > 0)
            {
                ret.Add(curr_point.point.transform.position);

                if (curr_point.point == starting_point)
                {
                    Debug.Log("Pathfinding finished correctly");
                    break;
                }

                memory_points.RemoveAt(memory_points.Count - 1);

                for (int i = 0; i < memory_points.Count; i++)
                {
                    if (memory_points[i].point == curr_point.parent)
                    {
                        curr_point = memory_points[i];
                        break;
                    }
                }
            }

            // ----------------
        }

        return ret;
    }

    struct PPoint
    {
        public GameObject parent;
        public GameObject point;
    }

    private GameObject[,] TransformMapToGrid(List<GameObject> p, int map_size_x, int map_size_y)
    {
        GameObject[,] ret = new GameObject[map_size_x, map_size_y];

        List<GameObject> points = new List<GameObject>();
        
        for(int z = 0; z < p.Count; z++)
        {
            points.Add(p[z]);
        }

        int curr_x = 0;
        int curr_y = 0;

        while (points.Count > 0)
        {
            float smaller_x = points[0].transform.position.x;
            float smaller_y = points[0].transform.position.z;

            int smaller_point_index = 0;

            for (int i = 0; i < points.Count; i++)
            {
                Vector3 curr_point = points[i].transform.position;

                if (curr_point.x < smaller_x)
                {
                    smaller_x = curr_point.z;
                }
            }

            for(int y = 0; y < points.Count; y++)
            {
                Vector3 curr_point = points[y].transform.position;

                if (curr_point.x == smaller_x)
                {
                    if (curr_point.z < smaller_y)
                    {
                        smaller_y = curr_point.x;
                        smaller_point_index = y;
                    }
                }
            }

            ret[curr_y, curr_x] = points[smaller_point_index];
            points.RemoveAt(smaller_point_index);

            if(curr_y < map_size_y-1)
            {
                curr_y++;
            }
            else
            {
                curr_y = 0;
                curr_x++;
            }
        }

        return ret;
    }

    public List<GameObject> GetPointsFromExpansion(int iterations, GameObject starting_point)
    {
        int iterations_count = 0;

        List<GameObject> memory_points = new List<GameObject>();

        if (IsPoint(starting_point))
        {
            List<GameObject> frontier = new List<GameObject>();

            frontier.Add(starting_point);

            while (frontier.Count != 0)
            {

                memory_points.Add(frontier[0]);

                NavigationPoint np = frontier[0].GetComponent<NavigationPoint>();
                List<GameObject> neigh = np.GetNeighbours();

                if (iterations_count < iterations)
                {
                    iterations_count++;

                    for (int y = 0; y < neigh.Count; y++)
                    {
                        bool exists = false;
                        for (int z = 0; z < memory_points.Count; z++)
                        {
                            if (memory_points[z] == neigh[y])
                            {
                                exists = true;
                                break;
                            }
                        }

                        if (!exists)
                            frontier.Add(neigh[y]);
                    }
                }

                frontier.RemoveAt(0);
            }
              
        }

        return memory_points;
    }

    public GameObject GridToWorldPoint(int index_x, int index_y)
    {
        GameObject ret = null;

        ret = grid[index_x, index_y];

        return ret;
    }

    public Vector2 WorldPointToGrid(GameObject point)
    {
        Vector2 ret = new Vector2(0, 0);

        for(int i = 0; i < size_x; i++)
        {
            for(int y = 0; y < size_y; y++)
            {
                if(grid[i, y] == point)
                {
                    ret.x = i;
                    ret.y = y;
                    return ret;
                }
            }
        }

        return new Vector2(0, 0);
    }


    public void PlaceMarker(Vector3 pos)
    {
        if (position_marker_instance != null)
        {
            Destroy(position_marker_instance);
            position_marker_instance = null;
        }

        position_marker_instance = Instantiate(position_marker, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity);
    }

    public void DeleteMarker()
    {
        Destroy(position_marker_instance);
        position_marker_instance = null;
    }

    public GameObject GetClosestNavPoint(Vector3 pos)
    {
        GameObject ret = null;

        List<GameObject> points = GetPoints();

        float closest_distance = 0;

        if (points.Count > 0)
        {
            closest_distance = Vector3.Distance(pos, points[0].transform.position);
            ret = points[0];
        }

        for (int i = 0; i < points.Count; i++)
        {
            if (Vector3.Distance(pos, points[i].transform.position) < closest_distance)
            {
                closest_distance = Vector3.Distance(pos, points[i].transform.position);
                ret = points[i];
            }
        }

        return ret;
    }
}
