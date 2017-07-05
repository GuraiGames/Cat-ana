using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private MatchManager match_manager = null;
    private NavigationEntity navigation_entity = null;
    private PlayerShadow _shadow = null;

    private string _network_id = "";
    private bool _is_player = false;
    private bool _stealth = false;
    private bool _attack = false;
    private int _life = 0;
    private Vector2 target_pos = new Vector2(0, 0);

	void Start ()
    {
        match_manager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
        navigation_entity = gameObject.GetComponent<NavigationEntity>();
	}

    public void SetInitialPlayerInfo(Vector3 pos, string network_id, bool is_player, GameObject shadow, int life = 0)
    {
        gameObject.transform.position = pos;
        _network_id = network_id;
        _is_player = is_player;
        _shadow = shadow.GetComponent<PlayerShadow>();
        _life = life;

        gameObject.GetComponent<PlayerShadow>().enabled = false;
    }

    public string GetNetworkId() { return _network_id; }

    public bool IsPlayer() { return _is_player; }

    public NavigationEntity GetNavigationEntity() { return navigation_entity; }

    public void Desappear()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public void Appear()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    public bool OnStealth()
    {
        return _stealth;
    }

    public void SetStealth(bool set)
    {
        _stealth = set;
    }

    public PlayerShadow GetPlayerShadow()
    {
        return _shadow.GetComponent<PlayerShadow>();
    }

    public void AdvanceTurn(MatchManager.TurnInfo turn_info)
    {
        switch(turn_info.turn)
        {
            case MatchManager.turn_type.strategy:
                _shadow.AdvanceTurn();
                _attack = false;
                break;

            case MatchManager.turn_type.action:
                break;
        }
    }

    public void SetAttack(bool set)
    {
        _attack = set;
    }

    public bool GetAttack()
    {
        return _attack;
    }

    public void DealDamage(int hit_points)
    {
        _life -= hit_points;

        if (_life < 0)
            _life = 0;

        Debug.Log("Damage dealt");
    }

    public int GetLife()
    {
        return _life;
    }

    public void SetTargetPos(Vector2 pos)
    {
        target_pos = pos;
    }

    public Vector2 GetTargetPos()
    {
        return target_pos;
    }

    public bool IsMoving()
    {
        return navigation_entity.IsMoving();
    }

    public void LoseStealth()
    {
        SetStealth(false);
        Appear();
        GetPlayerShadow().Desappear();

        if(IsPlayer())
            gameObject.GetComponent<Renderer>().material.color = new Color(0.3f, 0, 0);
        else
            gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
    }

    public void GetStealth()
    {
        SetStealth(true);
        Desappear();
        GetPlayerShadow().Appear();
    }

    public void Attack()
    {
        GameObject pos = navigation_entity.GetPos();
        List<GameObject> players = match_manager.GetPlayers();

        for(int i = 0; i < players.Count; i++)
        {
            Player curr_player = players[i].GetComponent<Player>();

            if(curr_player._network_id != _network_id)
            {
                if(curr_player.GetNavigationEntity().GetPos() == GetNavigationEntity().GetPos())
                {
                    curr_player.DealDamage(1);
                }
            }
        }

        LoseStealth();
    }
}
