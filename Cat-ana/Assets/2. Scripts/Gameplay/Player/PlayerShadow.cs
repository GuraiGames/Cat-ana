﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : MonoBehaviour 
{
    private GameObject _player = null;
    private NavigationEntity navigation_entity = null;

    List<GameObject> prev_pos = new List<GameObject>();

    private int stealth_distance = 2;

	// Private functions
	private void Start () 
	{
        navigation_entity = gameObject.GetComponent<NavigationEntity>();
    }

    public void SetInitialPlayerShadowInfo(GameObject player)
    {
        _player = player;
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
        if (prev_pos.Count >= 2)
        {
            prev_pos.RemoveAt(0);
        }
    }

    public void Desappear()
    {
        prev_pos.Clear();
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public void Appear()
    {
        prev_pos.Clear();

        gameObject.transform.position = _player.transform.position;

        gameObject.GetComponent<MeshRenderer>().enabled = true;
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
}