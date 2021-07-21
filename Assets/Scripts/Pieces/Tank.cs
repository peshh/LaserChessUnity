using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : BaseUnit
{
    public void Activate(int x = 0, int y = 0)
    {
        base.Activate(2, 4, "player", x, y);
        this.name = "Tank";
    }
}
