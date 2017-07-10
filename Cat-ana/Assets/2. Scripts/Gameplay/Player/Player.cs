using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private MatchManager match_manager = null;
    private NavigationEntity navigation_entity = null;
    private PlayerShadow _shadow = null;

    private string _network_id = "";
    private bool _is_client = false;
    private bool _stealth = false;
    private bool _visible = false;
    private bool _attack = false;
    private int _life = 0;
    private Vector2 target_pos = new Vector2(0, 0);

	void Start ()
    {
        match_manager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
        navigation_entity = gameObject.GetComponent<NavigationEntity>();
	}

    public void SetInitialPlayerInfo(Vector3 pos, string network_id, bool is_client, GameObject shadow, int life = 0)
    {
        gameObject.transform.position = pos;
        _network_id = network_id;
        _is_client = is_client;
        _shadow = shadow.GetComponent<PlayerShadow>();
        _life = life;

        gameObject.GetComponent<PlayerShadow>().enabled = false;
    }

    public string GetNetworkId() { return _network_id; }

    public bool IsClient() { return _is_client; }

    public NavigationEntity GetNavigationEntity() { return navigation_entity; }

    public void Desappear()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        _visible = false;
    }

    public void Appear()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        _visible = true;
    }

    public bool GetStealth()
    {
        return _stealth;
    }

    public void SetStealth(bool set)
    {
        _stealth = set;
    }

    public bool GetVisible()
    {
        return _visible;
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

    public void MoveTo(int grid_x, int grid_y)
    {
        // Get current point
        NavigationPoint curr_nav_point = navigation_entity.GetNavMap().GetClosestNavPoint(gameObject.transform.position).GetComponent<NavigationPoint>();

        // Ask for path and check if the position is different to the actual
        if(navigation_entity.MoveTo(grid_x, grid_y))
        {
            // Tell the tile that we are leaving
            //curr_nav_point.PlayerLeavesTile(gameObject);
        }

        // Get path
        //List<Vector3> path = navigation_entity.GetCurrentPath();

        //// Avoid overlap
        //if (path.Count > 0)
        //{
        //    // Get last point
        //    GameObject final_point = navigation_entity.GetNavMap().GetClosestNavPoint(path[0]);

        //    NavigationPoint final_point_script = final_point.GetComponent<NavigationPoint>();

        //    // Change the last point to the correct tile position depending of how much players there are on it
        //    Vector3 new_pos = final_point_script.AskForPlayerPosition(this.gameObject);

        //    path[0] = new_pos;
        //}
        //else
        //{
        //    curr_nav_point.AskForPlayerPosition(this.gameObject);
        //}
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
    }

    public void SetLife(int set)
    {
        if(set >= 0)
            _life = set;
    }

    public int GetLife()
    {
        return _life;
    }

    public bool IsDead()
    {
        return (_life == 0);
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
        _visible = true;

        Appear();
        GetPlayerShadow().Desappear();

        if(IsClient())
            gameObject.GetComponent<Renderer>().material.color = new Color(0.3f, 0, 0);
        else
            gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
    }

    public void GainStealth()
    {
        SetStealth(true);
        _visible = false;

        if (IsClient())
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(0, 0.3f, 0);
        }
        else
        {
            Desappear();
        }

        GetPlayerShadow().Appear();
    }

    public void Attack()
    {
        GameObject pos = navigation_entity.GetPos();
        List<GameObject> players = match_manager.GetPlayers();

        for(int i = 0; i < players.Count; i++)
        {
            Player player_to_attack = players[i].GetComponent<Player>();

            if(player_to_attack._network_id != _network_id)
            {
                if(player_to_attack.GetNavigationEntity().GetPos() == GetNavigationEntity().GetPos())
                {
                    if(player_to_attack.IsClient())
                    {
                        match_manager.SetLifeText(player_to_attack.GetLife());
                    }

                    if(player_to_attack.IsDead())
                    {
                        match_manager.KillPlayer(players[i]);
                    }

                    break;
                }
            }
        }

        LoseStealth();
    }
}
