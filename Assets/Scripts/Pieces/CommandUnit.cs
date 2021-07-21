using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandUnit : BaseUnit
{
    public void Activate(int x = 0, int y = 0)
    {
        base.Activate(0, 5, "AI", x, y);
        this.name = "CommandUnit";
    }
}
