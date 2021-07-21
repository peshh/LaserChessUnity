using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : BaseUnit
{
    public void Activate(int x = 0, int y = 0)
    {
        base.Activate(1, 2, "AI", x, y);
        this.name = "Drone";
    }
}
