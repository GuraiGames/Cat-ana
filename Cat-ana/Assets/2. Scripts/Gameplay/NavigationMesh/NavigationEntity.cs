using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationEntity : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private GameObject map;

    private NavigationMap nav_map = null;

    GameObject target_point = null;

    private List<Vector3> path = new List<Vector3>();

    bool move = false;

	// Use this for initialization
	void Start ()
    {
        map = GameObject.FindGameObjectWithTag("Map");
        nav_map = map.gameObject.GetComponent<NavigationMap>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKey("k"))
        {
            nav_map.target_point = nav_map.GetGrid()[3, 1];
        }


		if(nav_map.target_point != null && !move)
        {
            path = nav_map.GetPath(GetClosestNavPoint(), nav_map.target_point);
            target_point = nav_map.target_point;
            move = true;
        }

        if(move)
        {
            if (Vector3.Distance(gameObject.transform.position, target_point.transform.position) < 0.3f)
            {
                move = false;
                nav_map.target_point = null;
                target_point = null;
            }
            else if (Vector3.Distance(gameObject.transform.position, path[path.Count-1]) < 0.2f)
            {
                path.RemoveAt(path.Count - 1);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, path[path.Count - 1], speed*Time.deltaTime);
            }
        }
	}

    GameObject GetClosestNavPoint()
    {
        GameObject ret = null;

        List<GameObject> points = nav_map.GetPoints();

        float closest_distance = 0;

        if (points.Count > 0)
        {
            closest_distance = Vector3.Distance(gameObject.transform.position, points[0].transform.position);
            ret = points[0];
        }

        for (int i = 0; i<points.Count; i++)
        {
            if(Vector3.Distance(gameObject.transform.position, points[i].transform.position) < closest_distance)
            {
                closest_distance = Vector3.Distance(gameObject.transform.position, points[i].transform.position);
                ret = points[i];
            }
        }

        return ret;
    }

    
}
