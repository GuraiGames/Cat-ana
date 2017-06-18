using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationMap : MonoBehaviour
{
    [SerializeField]
    public GameObject nav_map_point;

    [SerializeField]
    public GameObject nav_map_points_parent;

    private List<GameObject> points = new List<GameObject>();

    public GameObject target_point = null;

    void Start()
    {
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
    }

    public List<GameObject> GetPoints()
    {
        return points;
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
            // Expand ---------

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
                    Debug.Log("Nice");
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
}
