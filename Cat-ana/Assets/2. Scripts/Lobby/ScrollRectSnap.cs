using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectSnap : MonoBehaviour {

    public RectTransform panel;
    public GameObject[] main_menu_go;
    public RectTransform center;

    private float[] distance;   // All buttons' distance to the center
    private bool dragging = false; // True when dragging
    private int distance_go = 0; // will hold the distance between the go
    private int min_num = 0;

	// Use this for initialization
	void Start () {
        int go_lenght = main_menu_go.Length;
        distance = new float[go_lenght];

        distance_go = (int)Mathf.Abs(main_menu_go[1].GetComponent<RectTransform>().anchoredPosition.x - main_menu_go[0].GetComponent<RectTransform>().anchoredPosition.x);
        print(distance_go);
    }
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < main_menu_go.Length; i++)
        {
            distance[i] = Mathf.Abs(center.transform.position.x - main_menu_go[i].transform.position.x);
        }

        float min_distance = Mathf.Min(distance); // Gets the min distance

        for (int a = 0; a < main_menu_go.Length; a++)
        {
            if (min_distance == distance[a])
            {
                min_num = a;
            }
        }

        if (!dragging)
        {
            LerpToGO(min_num * -distance_go);
        }
	}

    void LerpToGO(int position)
    {
        float new_x = Mathf.Lerp(panel.anchoredPosition.x, position, Time.deltaTime * 10f);
        Vector2 new_position = new Vector2(new_x, panel.anchoredPosition.y);

        panel.anchoredPosition = new_position;
    }

    public void ToggleDrag(bool toggle)
    {
        dragging = toggle;
    }
    
}
