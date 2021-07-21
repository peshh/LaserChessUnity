using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUnit : MonoBehaviour
{
    private int attackPower;
    private int hitPoints;
    private int posX, posY;
    private string team;
    private bool hasAttacked, hasMoved;
    protected PlayerController playerController;
    public GameObject health;
    public bool showHealth;

    public virtual void Activate(int attackPower, int hitPoints, string team, int x = 0, int y = 0)
    {
        this.SetPos(x, y);
        this.attackPower = attackPower;
        this.hitPoints = hitPoints;
        this.team = team;
    }

    void OnMouseUp()
    {
        Game game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

        if (this.team == "player")
        {
            PlayerController playerController = game.playerController;
            playerController.OnUnitClick(this);
        }
    }

    public int GetHP()
    {
        return this.hitPoints;
    }

    public Vector2 GetPos()
    {
        return new Vector2(posX, posY);
    }

    public void SetPos(int x, int y)
    {
        this.posX = x;
        this.posY = y;
    }

    public void SetPos(Vector2 coord)
    {
        this.posX = (int)coord.x;
        this.posY = (int)coord.y;
    }

    public virtual void Attack(BaseUnit piece)
    {
        piece.TakeDamage(this.attackPower);
    }

    public virtual void TakeDamage(int damage)
    {
        if (damage >= this.hitPoints)
        {
            this.hitPoints = 0;
            Destroy(this.gameObject);
        }
        else
        {
            this.hitPoints -= damage;
        }

        if (this.showHealth)
        {
            this.health.GetComponentInChildren<TMPro.TextMeshProUGUI>()
                .text = this.hitPoints.ToString();
        }
        else
        {
            this.health.GetComponentInChildren<TMPro.TextMeshProUGUI>()
                .text = string.Empty;
        }
    }

    public string GetTeam()
    {
        return this.team;
    }

    public bool GetHasAttacked()
    {
        return this.hasAttacked;
    }

    public void SetHasAttacked(bool value)
    {
        this.hasAttacked = value;
    }

    public bool GetHasMoved()
    {
        return this.hasMoved;
    }

    public void SetHasMoved(bool value)
    {
        this.hasMoved = value;
    }
}
