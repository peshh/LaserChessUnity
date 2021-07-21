using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController
{
    private GameObject[,] board;
    private PieceController pieceController;

    public PlayerController(GameObject[,] board, PieceController pieceController)
    {
        this.board = board;
        this.pieceController = pieceController;
    }

    // On clicking a unit: destroys all current plates and creates new ones for this unit.
    // If the unit has moved this turn only attack plates are created. If it has attacked, no plates are created.
    public void OnUnitClick(BaseUnit unit)
    {
        Game game = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<Game>();
        if (game.isPlayerTurn)
        {
            game.DestroyPlates();

            if (!unit.GetHasAttacked())
            {
                this.pieceController.CreateAttackPlates(unit);
                if (!unit.GetHasMoved())
                {
                    this.pieceController.CreateMovePlates(unit);
                }
            }
        }
    }

    // On clicking a plate:
    // Move plate - moves the plate's reference unit on this spot.
    // Attack plate - reference unit attacks this spot.
    // All plates are destroyed after the action is done.
    public void OnPlateClick(Plate plate)
    {
        string type = plate.gameObject.GetComponent<SpriteRenderer>().name;

        if (type == "MovePlate")
        {
            this.pieceController.MovePiece(plate.unit, plate.GetPos());
        }
        else
        {
            BaseUnit target = this.board[(int)plate.GetPos().y, (int)plate.GetPos().x]
                .GetComponent<BaseUnit>();

            this.pieceController.AttackPiece(plate.unit, target);
        }

        Game game = GameObject.FindGameObjectWithTag("GameController")
            .GetComponent<Game>();
        game.DestroyPlates();
    }
}
