using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private MatchManager match_manager = null;
    private NavigationEntity navigation_entity = null;
    private PlayerShadow _shadow = null;

    // Components
    private GameObject player_marker = null;
    private GameObject player_model = null;

    private string _network_id = "";
    private bool _is_client = false;
    private bool _stealth = false;
    private bool _visible = false;
    private bool _attack = false;
    private int _life = 0;
    private int _num_cards = 0;

    private Vector2 target_pos = new Vector2(0, 0);

    private List<Card> cards = new List<Card>();

	void Start ()
    {
        match_manager = GameObject.FindGameObjectWithTag("MatchManager").GetComponent<MatchManager>();
        navigation_entity = gameObject.GetComponent<NavigationEntity>();
	}

    public void SetInitialPlayerInfo(Vector3 pos, string network_id, bool is_client, GameObject shadow, int life = 0)
    {
        player_model = gameObject.transform.GetChild(0).gameObject;
        player_marker = gameObject.transform.GetChild(1).gameObject;
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
        player_model.GetComponent<MeshRenderer>().enabled = false;
        player_marker.GetComponent<MeshRenderer>().enabled = false;
        _visible = false;
    }

    public void Appear()
    {
        player_model.GetComponent<MeshRenderer>().enabled = true;
        player_marker.GetComponent<MeshRenderer>().enabled = true;
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
                {
                    _shadow.AdvanceTurn();
                    _attack = false;
                }
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
            player_marker.GetComponent<Renderer>().material.color = new Color(0.3f, 0, 0);
        else
            player_marker.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
    }

    public void GainStealth()
    {
        SetStealth(true);
        _visible = false;
        if (IsClient())
        {
            player_marker.GetComponent<Renderer>().material.color = new Color(0, 0.3f, 0);
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

                    break;
                }
            }
        }

        LoseStealth();
    }

    public void TakeCard(string name)
    {
        if (cards.Count >= 3)
            return;

        Card card = new Card(name, this);

        cards.Add(card);

        _num_cards = cards.Count;
    }

    public void UseCard(string name, Player target = null)
    {
        DoCardEffects(name, target);

        _num_cards--;
    }

    private void DoCardEffects(string name, Player target)
    {
        if (_num_cards <= 0)
            return;

        switch (name)
        {
            case "Raven":
                if(target != null)
                    target.LoseStealth();
                break;

            case "Stealth":
                GainStealth();
                break;
        }
    }

    public void SetNumCards(int set)
    {
        if (!IsClient())
            _num_cards = set;
        else
            _num_cards = cards.Count;
    }

    public int GetCardsCount()
    {
        return _num_cards;
    }

    public string GetCardNameByIndex(int index)
    {
        return cards[index].GetName();
    }

    public void RemoveCardAtIndex(int index)
    {
        cards.RemoveAt(index);
        _num_cards = cards.Count;
    }

    public class Card
    {
        public Card(string name, Player owner)
        {
            _name = name;
            _owner = owner;
        }
        public string GetName()
        {
            return _name;
        }

        public Player GetOwner()
        {
            return _owner;
        }

        public void SetTarget(Player target)
        {
            _target = target;
        }

        public Player GetTarget()
        {
            return _target;
        }

        private string _name;
        private Player _owner;
        private Player _target;
    }

    public void SetMarkerColor(Color color)
    {
        player_marker.GetComponent<Renderer>().material.color = color;
    }

    private void OnMouseDown()
    {
        if (!IsClient())
        {
            match_manager.target = this;
            match_manager.curr_action = MatchManager.action.card_target_selected;
        }
    }
}
