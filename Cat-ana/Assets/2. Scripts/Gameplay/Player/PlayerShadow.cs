using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : MonoBehaviour 
{
    private Player _player = null;
    private NavigationEntity navigation_entity = null;
    private MatchManager match_manager = null;

    // Components
    private GameObject player_marker = null;
    private GameObject player_model = null;

    List<GameObject> prev_pos = new List<GameObject>();

    private int stealth_distance = 2;

	// Private functions
	private void Start () 
	{
        navigation_entity = gameObject.GetComponent<NavigationEntity>();
        match_manager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
    }

    public void SetInitialPlayerShadowInfo(GameObject player)
    {
        player_model = gameObject.transform.GetChild(0).gameObject;
        player_marker = gameObject.transform.GetChild(1).gameObject;
        _player = player.GetComponent<Player>();
        gameObject.GetComponent<Player>().enabled = false;
        gameObject.transform.position = player.transform.position;
    }

    public void AddPosition(GameObject pos)
    {
        prev_pos.Add(pos);
    }

    public Vector2 GetNextPos()
    {
        Vector2 ret = navigation_entity.GetGridPos();

        if (prev_pos.Count >= stealth_distance)
        {
            ret = navigation_entity.GetNavMap().WorldPointToGrid(prev_pos[0]);
        }

        return ret;
    }

    public void AdvanceTurn()
    {
        if (prev_pos.Count >= stealth_distance)
        {
            prev_pos.RemoveAt(0);
        }
    }

    public void Desappear()
    {
        prev_pos.Clear();
        player_model.GetComponent<MeshRenderer>().enabled = false;
        player_marker.GetComponent<MeshRenderer>().enabled = false;
    }

    public void Appear()
    {
        prev_pos.Clear();

        gameObject.transform.position = _player.transform.position;

        player_model.GetComponent<MeshRenderer>().enabled = true;
        player_marker.GetComponent<MeshRenderer>().enabled = true;
    }

    public NavigationEntity GetNavigationEntity()
    {
        return navigation_entity;
    }

    public List<GameObject> GetPreviousPositions() { return prev_pos; }

    public void SetStealthDistance(int set)
    {
        stealth_distance = set;
    }

    private void OnMouseDown()
    {
        if (!_player.IsClient())
        {
            match_manager.target = _player;
            match_manager.curr_action = MatchManager.action.card_target_selected;
        }
    }

    public void SetMarkerColor(Color color)
    {
        player_marker.GetComponent<Renderer>().material.color = color;
    }
}
