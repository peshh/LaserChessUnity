using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    private float screenX = -3.5f;
    private float screenY = -3.5f;
    private GameObject[,] board;
    private List<GameObject> player = new List<GameObject>();
    private List<GameObject> ai = new List<GameObject>();
    private bool gameOver, playerWin;
    private string[][] levels = new string[][]
    {
        new string[] 
        {
            "Grunt,2,1", "Grunt,3,1", "Grunt,4,1", "Grunt,5,1",
            "Jumpship,0,0", "Jumpship,1,0", "Jumpship,6,0", "Jumpship,7,0",
            "Tank,3,0", "Tank,4,0",
            "Drone,0,6", "Drone,1,6", "Drone,2,6", "Drone,5,6", "Drone,6,6", "Drone,7,6",
            "Dreadnought,3,6", "Dreadnought,4,6",
            "CommandUnit,3,7", "CommandUnit,4,7",
        },
        new string[]
        {
            "Grunt,0,1", "Grunt,1,1", "Grunt,6,1", "Grunt,7,1",
            "Jumpship,2,0", "Jumpship,3,0", "Jumpship,4,0", "Jumpship,5,0",
            "Tank,0,0", "Tank,7,0",
            "Drone,0,4", "Drone,1,4", "Drone,2,4", "Drone,3,5", "Drone,4,5", "Drone,5,4", "Drone,6,4", "Drone,7,4",
            "Dreadnought,3,4", "Dreadnought,4,4",
            "CommandUnit,2,7", "CommandUnit,5,7",
        },
        new string[]
        {
            "Grunt,0,1", "Grunt,1,1", "Grunt,2,1", "Grunt,3,1", "Grunt,4,1", "Grunt,5,1", "Grunt,6,1", "Grunt,7,1",
            "Jumpship,2,0", "Jumpship,5,0",
            "Tank,0,0", "Tank,7,0",
            "Drone,0,4", "Drone,1,4", "Drone,2,5", "Drone,3,5", "Drone,4,5", "Drone,5,5", "Drone,6,4", "Drone,7,4",
            "Dreadnought,2,4", "Dreadnought,3,4", "Dreadnought,4,4", "Dreadnought,5,4",
            "CommandUnit,0,7",
        },
    };

    public GameObject grunt, tank, jumpship;
    public GameObject drone, dreadnought, commandUnit;
    public GameObject movePlate, attackPlate;
    public GameObject restartLevelButton, nextLevelButton;
    public GameObject winScreen, loseScreen;
    public PieceController pieceController;
    public PlayerController playerController;
    public AIController aiController;
    public bool isPlayerTurn, showHealth, showDamage;
    public int level = 0;

    // Start is called before the first frame update
    void Start()
    {
        this.ResetLevel();

        this.isPlayerTurn = true;
    }

    public Vector2 GetScreen()
    {
        return new Vector2(this.screenX, this.screenY);
    }

    void CreateUnit(string type, int posX, int posY)
    {
        if (type == "Grunt")
        {
            GameObject gO = Instantiate(this.grunt, new Vector3(posX + screenX, posY + screenY, -1.0f), Quaternion.identity);
            Grunt grunt = gO.GetComponent<Grunt>();
            grunt.Activate(posX, posY);
            this.board[posY, posX] = gO;
            this.player.Add(gO);
        }
        else if (type == "Tank")
        {
            GameObject gO = Instantiate(this.tank, new Vector3(posX + screenX, posY + screenY, -1.0f), Quaternion.identity);
            Tank tank = gO.GetComponent<Tank>();
            tank.Activate(posX, posY);
            this.board[posY, posX] = gO;
            this.player.Add(gO);
        }
        else if (type == "Jumpship")
        {
            GameObject gO = Instantiate(this.jumpship, new Vector3(posX + screenX, posY + screenY, -1.0f), Quaternion.identity);
            Jumpship jumpship = gO.GetComponent<Jumpship>();
            jumpship.Activate(posX, posY);
            this.board[posY, posX] = gO;
            this.player.Add(gO);
        }
        else if (type == "Drone")
        {
            GameObject gO = Instantiate(this.drone, new Vector3(posX + screenX, posY + screenY, -1.0f), Quaternion.identity);
            Drone drone = gO.GetComponent<Drone>();
            drone.Activate(posX, posY);
            this.board[posY, posX] = gO;
            this.ai.Add(gO);
        }
        else if (type == "Dreadnought")
        {
            GameObject gO = Instantiate(this.dreadnought, new Vector3(posX + screenX, posY + screenY, -1.0f), Quaternion.identity);
            Dreadnought dreadnought = gO.GetComponent<Dreadnought>();
            dreadnought.Activate(posX, posY);
            this.board[posY, posX] = gO;
            this.ai.Add(gO);
        }
        else if (type == "CommandUnit")
        {
            GameObject gO = Instantiate(this.commandUnit, new Vector3(posX + screenX, posY + screenY, -1.0f), Quaternion.identity);
            CommandUnit commandUnit = gO.GetComponent<CommandUnit>();
            commandUnit.Activate(posX, posY);
            this.board[posY, posX] = gO;
            this.ai.Add(gO);
        }
    }

    public void DestroyPlates()
    {
        GameObject[] plates = GameObject.FindGameObjectsWithTag("Plate");
        for (int i = 0; i < plates.Length; i++)
        {
            Destroy(plates[i]);
        }
    }

    public void DestroyObjectAt(Vector2 pos)
    {
        GameObject target = this.board[(int)pos.y, (int)pos.x];
        this.board[(int)pos.y, (int)pos.x] = null;
        if (target.GetComponent<BaseUnit>().GetTeam() == "AI")
        {
            CommandUnit cu;
            bool hasCommandUnit = false;
            for (int i = this.ai.Count - 1; i >= 0; i--)
            {
                if (this.ai[i] == target)
                {
                    this.ai.RemoveAt(i);
                }
                else if (this.ai[i].TryGetComponent<CommandUnit>(out cu))
                {
                    hasCommandUnit = true;
                }
            }

            if (!hasCommandUnit)
            {
                this.gameOver = true;
                this.playerWin = true;
            }
        }
        else
        {
            for (int i = this.player.Count - 1; i >= 0; i--)
            {
                if (this.player[i] == target)
                {
                    this.player.RemoveAt(i);
                    break;
                }
            }

            if (this.player.Count == 0)
            {
                this.gameOver = true;
                this.playerWin = false;
            }
        }
        Destroy(target);
    }

    public void InstantiateAttackPlates(BaseUnit unit, List<Vector2> coords)
    {
        if (!this.gameOver)
        {
            foreach (Vector2 coord in coords)
            {
                GameObject gO = Instantiate(this.attackPlate, new Vector3(coord.x + this.screenX, coord.y + this.screenY, -1.5f), Quaternion.identity);
                Plate plate = gO.GetComponent<Plate>();
                plate.Activate(unit, "AttackPlate", (int)coord.x, (int)coord.y);
            }
        }
    }

    public void InstantiateMovePlates(BaseUnit unit, List<Vector2> coords)
    {
        if (!this.gameOver)
        {
            foreach (Vector2 coord in coords)
            {
                GameObject gO = Instantiate(this.movePlate, new Vector3(coord.x + this.screenX, coord.y + this.screenY, -1.0f), Quaternion.identity);
                Plate plate = gO.GetComponent<Plate>();
                plate.Activate(unit, "MovePlate", (int)coord.x, (int)coord.y);
            }
        }
    }

    public void EndTurn()
    {
        // Remove HasAttacked and HasMoved flags from the player units.
        foreach (GameObject gO in this.player)
        {
            BaseUnit unit = gO.GetComponent<BaseUnit>();
            unit.SetHasAttacked(false);
            unit.SetHasMoved(false);
        }

        StartCoroutine(this.MakeAITurn());

        foreach (GameObject gO in this.ai)
        {
            BaseUnit unit = gO.GetComponent<BaseUnit>();
            unit.SetHasAttacked(false);
            unit.SetHasMoved(false);
        }
    }

    public void ToggleHealth()
    {
        this.showHealth = !this.showHealth;

        this.ShowHealth(this.showHealth);
    }

    private void ShowHealth(bool show)
    {
        foreach (GameObject obj in this.player)
        {
            obj.GetComponent<BaseUnit>().showHealth = show;
            obj.GetComponent<BaseUnit>().TakeDamage(0);
        }
        foreach (GameObject obj in this.ai)
        {
            obj.GetComponent<BaseUnit>().showHealth = show;
            obj.GetComponent<BaseUnit>().TakeDamage(0);
        }
    }

    private IEnumerator MakeAITurn()
    {
        this.isPlayerTurn = false;

        this.DestroyPlates();

        foreach (GameObject obj in this.ai)
        {
            Drone drone;
            bool isDrone = obj.TryGetComponent<Drone>(out drone);
            if (isDrone)
            {
                this.aiController.DroneMove(drone);
                if (drone.GetPos().y == 0)
                {
                    this.gameOver = true;
                    this.playerWin = false;
                }
                yield return new WaitForSeconds(0.5f);

                List<Vector2> pos = this.aiController.DroneAttack(drone);
                if (pos.Count > 0)
                {
                    this.InstantiateAttackPlates(drone, pos);
                    yield return new WaitForSeconds(0.5f);
                    this.DestroyPlates();
                }
            }
        }

        foreach (GameObject obj in this.ai)
        {
            Dreadnought dreadnought;
            bool isDreadnought = obj.TryGetComponent<Dreadnought>(out dreadnought);
            if (isDreadnought)
            {
                this.aiController.DreadnoughtMove(dreadnought);
                yield return new WaitForSeconds(0.5f);

                List<Vector2> pos = this.aiController.DreadnoughtAttack(dreadnought);
                if (pos.Count > 0)
                {
                    this.InstantiateAttackPlates(dreadnought, pos);
                    yield return new WaitForSeconds(0.5f);
                    this.DestroyPlates();
                }
            }
        }

        foreach (GameObject obj in this.ai)
        {
            CommandUnit cu;
            bool isCU = obj.TryGetComponent<CommandUnit>(out cu);
            if (isCU)
            {
                this.aiController.CommandUnitMove(cu);
            }
        }

        this.isPlayerTurn = true;
    }

    public void NextLevel()
    {
        this.level++;
        if (this.level >= this.levels.Length)
        {
            this.level = 0;
        }

        this.ResetLevel();
    }

    public void ResetLevel()
    {
        this.winScreen.SetActive(false);
        this.loseScreen.SetActive(false);

        this.board = new GameObject[8, 8];
        this.pieceController = new PieceController(this.board);
        this.playerController = new PlayerController(this.board, this.pieceController);
        this.aiController = new AIController(this.board, this.pieceController);
        this.gameOver = false;
        this.playerWin = false;

        foreach (GameObject gameObject in this.ai)
        {
            Destroy(gameObject);
        }
        this.ai.Clear();

        foreach (GameObject gameObject in this.player)
        {
            Destroy(gameObject);
        }
        this.player.Clear();

        for (int i = 0; i < this.levels[this.level].Length; i++)
        {
            string[] split = this.levels[this.level][i].Split(',');
            string name = split[0];
            int x = int.Parse(split[1]);
            int y = int.Parse(split[2]);
            this.CreateUnit(name, x, y);
        }

        this.ShowHealth(this.showHealth);

        this.restartLevelButton.SetActive(true);
        this.nextLevelButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameOver)
        {
            if (this.playerWin)
            {
                this.restartLevelButton.SetActive(false);
                this.nextLevelButton.SetActive(true);
                this.winScreen.SetActive(true);
            }
            else
            {
                this.restartLevelButton.SetActive(false);
                this.nextLevelButton.SetActive(true);
                this.loseScreen.SetActive(true);
            }
        }
    }
}
