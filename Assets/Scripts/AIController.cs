using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController
{
    private GameObject[,] board;
    private PieceController pieceController;
    public AIController(GameObject[,] board, PieceController pieceController)
    {
        this.board = board;
        this.pieceController = pieceController;
    }

    public void DroneMove(Drone drone)
    {
        List<Vector2> coords = this.pieceController.GetPieceMoveSpots(drone);
        if (coords.Count > 0)
        {
            // The drone has only one movement option henceforth chosing the first (and only) in the list.
            this.pieceController.MovePiece(drone, coords[0]);
        }
    }

    public List<Vector2> DroneAttack(Drone drone)
    {
        List<Vector2> coords = this.pieceController.GetPieceAttackSpots(drone);
        List<Vector2> result = new List<Vector2>();
        if (coords.Count > 0)
        {
            BaseUnit target = this.GetLowestHPTarget(coords);
            this.pieceController.AttackPiece(drone, target);
            result.Add(target.GetPos());
        }
        return result;
    }

    public void DreadnoughtMove(Dreadnought dreadnought)
    {
        List<Vector2> coords = this.pieceController.GetPieceMoveSpots(dreadnought);
        if (coords.Count > 0)
        {
            Vector2 closestEnemyPos = this.GetClosestEnemyPos(dreadnought.GetPos());
            int moveIndex = 0;
            float closest = this.board.Length;
            for (int i = 0; i < coords.Count; i++)
            {
                float check = Calculus
                    .FindLengthPythagorean(coords[i].x - closestEnemyPos.x,
                                           coords[i].y - closestEnemyPos.y);
                if (check < closest)
                {
                    closest = check;
                    moveIndex = i;
                }
            }
            this.pieceController.MovePiece(dreadnought, coords[moveIndex]);
        }
    }

    public List<Vector2> DreadnoughtAttack(Dreadnought dreadnought)
    {
        List<Vector2> coords = this.pieceController.GetPieceAttackSpots(dreadnought);
        for (int i = 0; i < coords.Count; i++)
        {
            BaseUnit target = this.board[(int)coords[i].y, (int)coords[i].x].GetComponent<BaseUnit>();
            this.pieceController.AttackPiece(dreadnought, target);
        }
        return coords;
    }

    public void CommandUnitMove(CommandUnit commandUnit)
    {
        List<Vector2> coords = this.pieceController.GetPieceMoveSpots(commandUnit);
        if (coords.Count > 0)
        {
            this.pieceController.MovePiece(commandUnit, GetSafestSpot(coords));
        }
    }

    private BaseUnit GetLowestHPTarget(List<Vector2> coords)
    {
        int targetIndex = 0;
        int targetHP = this.board[(int)coords[0].y, (int)coords[0].x].GetComponent<BaseUnit>().GetHP();
        for (int i = 1; i < coords.Count; i++)
        {
            BaseUnit target = board[(int)coords[i].y, (int)coords[i].x].GetComponent<BaseUnit>();
            if (target.GetHP() < targetHP)
            {
                targetHP = target.GetHP();
                targetIndex = i;
            }
        }

        return board[(int)coords[targetIndex].y, (int)coords[targetIndex].x].GetComponent<BaseUnit>();
    }

    // For each enemy unit on the board finds the shortest distance (using the Pythagorean theorem).
    private Vector2 GetClosestEnemyPos(Vector2 pos)
    {
        float closest = this.board.Length;
        Vector2 result = new Vector2(pos.x, pos.y);
        for (int x = 0; x < this.board.GetLength(1); x++)
        {
            for (int y = 0; y < this.board.GetLength(0); y++)
            {
                if (this.board[y, x] != null &&
                    this.board[y, x].GetComponent<BaseUnit>().GetTeam() == "player")
                {
                    float check = Calculus.FindLengthPythagorean(x - pos.x, y - pos.y);
                    if (check < closest)
                    {
                        closest = check;
                        result = new Vector2(x, y);
                    }
                }
            }
        }

        return result;
    }

    // For each of the input coordinates finds the spot where the minimum number of enemies and 
    // a maximum number of friendlies have line of sight.
    private Vector2 GetSafestSpot(List<Vector2> coords)
    {
        int[] enemyCount = new int[3];
        int[] teamCount = new int[3];
        int lowestEnemyIndex = 0;
        for (int i = 0; i < coords.Count; i++)
        {
            enemyCount[i] = this.CountClosestUnitsOnLines(coords[i], "player");
            teamCount[i] = this.CountClosestUnitsOnLines(coords[i], "AI");

            if (i != 0)
            {
                if (enemyCount[i] < enemyCount[lowestEnemyIndex])
                {
                    lowestEnemyIndex = i;
                }
                else if (enemyCount[i] == enemyCount[lowestEnemyIndex])
                {
                    if (teamCount[i] > teamCount[lowestEnemyIndex])
                    {
                        lowestEnemyIndex = i;
                    }
                }
            }

        }

        return coords[lowestEnemyIndex];
    }

    private int CountClosestUnitsOnLines(Vector2 pos, string team)
    {
        int result = 0;
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x != 0 || y != 0)
                {
                    List<Vector2> coords = Calculus.FindLine((int)pos.x, (int)pos.y, x, y, 0, this.board.GetLength(0));
                    for (int i = 0; i < coords.Count; i++)
                    {
                        if (this.board[(int)coords[i].y, (int)coords[i].x] != null &&
                            this.board[(int)coords[i].y, (int)coords[i].x].GetComponent<BaseUnit>().GetTeam() == team)
                        {
                            result++;
                            break;
                        }
                    }
                }
            }
        }

        return result;
    }
}
