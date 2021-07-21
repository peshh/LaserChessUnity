using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController
{
    private GameObject[,] board;

    public PieceController(GameObject[,] board)
    {
        this.board = board;
    }

    public void MovePiece(BaseUnit piece, Vector2 coord)
    {
        Vector3 startPos = new Vector3(piece.GetPos().x, piece.GetPos().y, -1.0f);

        board[(int)startPos.y, (int)startPos.x] = null;
        piece.SetPos(coord);
        board[(int)coord.y, (int)coord.x] = piece.gameObject;

        Game game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        Vector3 endPos = coord;
        endPos.z = -1.0f;
        endPos += (Vector3)game.GetScreen();
        float time = Vector3.Distance(piece.gameObject.transform.position, endPos) / 3.0f;
        game.StartCoroutine(this.LerpPosition(piece.gameObject, endPos, time));
        piece.SetHasMoved(true);
    }

    // Making the movement smooth (instead of the unit just teleporting to the new pos).
    IEnumerator LerpPosition(GameObject unit, Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = unit.transform.position;

        while (time < duration)
        {
            unit.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        unit.transform.position = targetPosition;
    }

    public void AttackPiece(BaseUnit attacker, BaseUnit target)
    {
        attacker.Attack(target);

        if (target.GetHP() == 0)
        {
            Game game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
            game.DestroyObjectAt(target.GetPos());
        }

        attacker.SetHasAttacked(true);
    }

    public List<Vector2> GetPieceMoveSpots(BaseUnit piece)
    {
        string name = piece.name;
        switch (name)
        {
            case "Grunt": return GetMoveSpotsGrunt(piece);
            case "Jumpship": return GetMoveSpotsJumpship(piece);
            case "Tank": return GetMoveSpotsTank(piece);
            case "Drone": return GetMoveSpotsDrone(piece);
            case "Dreadnought": return GetMoveSpotsDreadnought(piece);
            case "CommandUnit": return GetMoveSpotsCU(piece);
            default: return new List<Vector2>();
        }
    }

    public List<Vector2> GetPieceAttackSpots(BaseUnit piece)
    {
        string name = piece.name;
        switch (name)
        {
            case "Grunt": return GetAttackSpotsGrunt(piece);
            case "Jumpship": return GetAttackSpotsJumpship(piece);
            case "Tank": return GetAttackSpotsTank(piece);
            case "Drone": return GetAttackSpotsDrone(piece);
            case "Dreadnought": return GetAttackSpotsDreadnought(piece);
            default: return new List<Vector2>();
        }
    }

    private List<Vector2> GetMoveSpotsGrunt(BaseUnit piece)
    {
        return GetNeighbors(piece.GetPos(), false);
    }

    private List<Vector2> GetMoveSpotsJumpship(BaseUnit piece)
    {
        int posX = (int)piece.GetPos().x;
        int posY = (int)piece.GetPos().y;
        List<Vector2> result = Calculus.FindLPositions(posX, posY);
        for (int i = result.Count - 1; i >= 0; i--)
        {
            Vector2 coord = result[i];
            int coordX = (int)coord.x;
            int coordY = (int)coord.y;
            if (IsOutOfBounds(coordX, 0, this.board.GetLength(1)) || 
                IsOutOfBounds(coordY, 0, this.board.GetLength(0)) 
                || this.board[coordY, coordX] != null)
            {
                result.Remove(coord);
            }
        }

        return result;
    }

    private List<Vector2> GetMoveSpotsTank(BaseUnit piece)
    {
        int posX = (int)piece.GetPos().x;
        int posY = (int)piece.GetPos().y;
        List<Vector2> result = new List<Vector2>();

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // i == 0 and j == 0 at the same time means no increment, therefore no line.
                if (i != 0 || j != 0)
                {
                    List<Vector2> temp = Calculus.FindLine(posX, posY, i, j, 0, this.board.GetLength(0), 3);
                    for (int k = 0; k < temp.Count; k++)
                    {
                        // FindLine collects all the coords in a line in order of moving away from the start point.
                        // The first tile which is taken denies more movement on this line so all after it are deleted.
                        if (this.board[(int)temp[k].y, (int)temp[k].x] != null)
                        {
                            temp.RemoveRange(k, temp.Count - k);
                            break;
                        }
                    }
                    result.AddRange(temp);
                }
            }
        }

        return result;
    }

    private List<Vector2> GetMoveSpotsDrone(BaseUnit piece)
    {
        List<Vector2> result = new List<Vector2>();
        int posX = (int)piece.GetPos().x;
        int posY = (int)piece.GetPos().y;

        if (!IsOutOfBounds(posY - 1, 0, this.board.GetLength(0)) &&
            this.board[posY - 1, posX] == null)
        {
            result.Add(new Vector2(posX, posY - 1));
        }

        return result;
    }

    private List<Vector2> GetMoveSpotsDreadnought(BaseUnit piece)
    {
        return GetNeighbors(piece.GetPos(), true);
    }

    private List<Vector2> GetMoveSpotsCU(BaseUnit piece)
    {
        List<Vector2> result = new List<Vector2>();
        int posX = (int)piece.GetPos().x;
        int posY = (int)piece.GetPos().y;

        if (!IsOutOfBounds(posX - 1, 0, this.board.GetLength(1)) &&
            this.board[posY, posX - 1] == null)
        {
            result.Add(new Vector2(posX - 1, posY));
        }

        result.Add(new Vector2(posX, posY));

        if (!IsOutOfBounds(posX + 1, 0, this.board.GetLength(1)) &&
            this.board[posY, posX + 1] == null)
        {
            result.Add(new Vector2(posX + 1, posY));
        }

        return result;
    }

    private List<Vector2> GetAttackSpotsGrunt(BaseUnit piece)
    {
        int posX = (int)piece.GetPos().x;
        int posY = (int)piece.GetPos().y;
        List<Vector2> result = new List<Vector2>();

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // i == 0 or j == 0 means orthogonal lines and we need only diagonal for this one.
                if (i != 0 && j != 0)
                {
                    result.AddRange(GetAttackSpotsOnLines(posX, posY, i, j, 0, this.board.GetLength(0), "AI"));
                }
            }
        }

        return result;
    }

    private List<Vector2> GetAttackSpotsJumpship(BaseUnit piece)
    {
        return GetNeighbors(piece.GetPos(), false, "AI");
    }

    private List<Vector2> GetAttackSpotsTank(BaseUnit piece)
    {
        int posX = (int)piece.GetPos().x;
        int posY = (int)piece.GetPos().y;
        List<Vector2> result = new List<Vector2>();

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // i != 0 or j != 0 means diagonal lines and we need only orthogonal for this one.
                if (i == 0 || j == 0)
                {
                    result.AddRange(GetAttackSpotsOnLines(posX, posY, i, j, 0, this.board.GetLength(0), "AI"));
                }
            }
        }

        return result;
    }

    private List<Vector2> GetAttackSpotsDrone(BaseUnit piece)
    {
        int posX = (int)piece.GetPos().x;
        int posY = (int)piece.GetPos().y;
        List<Vector2> result = new List<Vector2>();

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // i == 0 or j == 0 means orthogonal lines and we need only diagonal for this one.
                if (i != 0 && j != 0)
                {
                    result.AddRange(GetAttackSpotsOnLines(posX, posY, i, j, 0, this.board.GetLength(0), "player"));
                }
            }
        }

        return result;
    }

    private List<Vector2> GetAttackSpotsDreadnought(BaseUnit piece)
    {
        return GetNeighbors(piece.GetPos(), true, "player");
    }

    private bool IsOutOfBounds(int coord, int min, int max)
    {
        return coord < min || coord >= max;
    }

    // Get neighboring spots.
    // If diagonal is false, returns only orthogonal neighbors.
    // If no team is given, returns free neighbor spots.
    // If a team is given, returns only neighboring spots with units of that team.
    // Teams can be 'player' or 'AI'.
    private List<Vector2> GetNeighbors(Vector2 point, bool diagonal, string team = "")
    {
        int posX = (int)point.x;
        int posY = (int)point.y;
        List<Vector2> result = Calculus.FindNeighbors(posX, posY, diagonal);
        for (int i = result.Count - 1; i >= 0; i--)
        {
            Vector2 coord = result[i];
            int coordX = (int)coord.x;
            int coordY = (int)coord.y;
            if (team == "")
            {
                if (IsOutOfBounds(coordX, 0, this.board.GetLength(1)) ||
                    IsOutOfBounds(coordY, 0, this.board.GetLength(0)) ||
                    this.board[coordY, coordX] != null)
                {
                    result.Remove(coord);
                }
            }
            else
            {
                if (IsOutOfBounds(coordX, 0, this.board.GetLength(1)) ||
                    IsOutOfBounds(coordY, 0, this.board.GetLength(0)) ||
                    this.board[coordY, coordX] == null ||
                    this.board[coordY, coordX].GetComponent<BaseUnit>().GetTeam() != team)
                {
                    result.Remove(coord);
                }
            }

        }

        return result;
    }

    // Finds the first unit on the corresponding line and returns it if it's of the called team.
    private List<Vector2> GetAttackSpotsOnLines(int posX, int posY, int incrementX, int incrementY, int min, int max, string enemyTeam)
    {
        List<Vector2> result = new List<Vector2>();

        List<Vector2> temp = Calculus.FindLine(posX, posY, incrementX, incrementY, min, max);
        for (int k = 0; k < temp.Count; k++)
        {
            if (this.board[(int)temp[k].y, (int)temp[k].x] != null)
            {
                if (this.board[(int)temp[k].y, (int)temp[k].x].GetComponent<BaseUnit>().GetTeam() == enemyTeam)
                {
                    result.Add(temp[k]);
                }
                break;
            }
        }

        return result;
    }

    public void CreateMovePlates(BaseUnit unit)
    {
        List<Vector2> coords = this.GetPieceMoveSpots(unit);
        Game game = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<Game>();
        game.InstantiateMovePlates(unit, coords);
    }

    public void CreateAttackPlates(BaseUnit unit)
    {
        List<Vector2> coords = this.GetPieceAttackSpots(unit);
        Game game = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<Game>();
        game.InstantiateAttackPlates(unit, coords);
    }
}
