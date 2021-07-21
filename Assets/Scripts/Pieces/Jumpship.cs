using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpship : BaseUnit
{
    public void Activate(int x = 0, int y = 0)
    {
        base.Activate(2, 2, "player", x, y);
        this.name = "Jumpship";
    }
}
