using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    private int posX, posY;
    public BaseUnit unit;

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

    internal void Activate(BaseUnit unit, string name, int x, int y)
    {
        this.unit = unit;
        this.name = name;
        this.SetPos(x, y);
    }

    void OnMouseUp()
    {
        Game game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

        PlayerController playerController = game.playerController;
        playerController.OnPlateClick(this);
    }
}
