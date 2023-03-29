using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    [SerializeField] 
    private TextAsset _stageTextAsset;
    
    [SerializeField]
    private GameObject _cellPrefab;

    private CellStatus[,] _cellStatusArray;
    private Cell[,] _cellArray;
    private GamePhaseEnum _gamePhase = GamePhaseEnum.SETUP;
    bool IsGameOver()
    {
        for(int k=0; k<this._cellStatusArray.GetLength(0) - 2; k++)
        {
            for(int x=0; x<this._cellStatusArray.GetLength(1) - 2; x++)
            {
                if(this._cellArray[k+1, x+1].IsTarget())
                {
                    return false;
                }
            }
        }
        return true; 
    }
    void LoadCellStatus() 
    {
        string[] lines = this._stageTextAsset.text.Split("\n");
        string[] first_words = lines[0].Split(",");

        //for(int i =0; i < lines.GetLength(0); i++) 
        //{
        //    string[] words = lines[i].Split(",");
        //    for(int j = 0; j<words.GetLength(0); j++)
        //    {
        //        Debug.Log(words[j]);
        //    }
        //}

        this._cellStatusArray = new CellStatus[lines.Length + 2, first_words.Length +2];

        for(int j=0; j<this._cellStatusArray.GetLength(1); j++)
        {
            this._cellStatusArray[0, j] = new CellStatus(
                false,
                true,
                false
            );
        }

        for(int i=0; i<this._cellStatusArray.GetLength(0) -2; i++)
        {
            this._cellStatusArray[i +1, 0] = new CellStatus(
                false,
                true,
                false
            );
            string[] words = lines[i].Split(",");
            for(int j=0; j<this._cellStatusArray.GetLength(1) - 2; j++)
            {
                switch(words[j].Trim())
                {
                    case "0":
                        this._cellStatusArray[i + 1, j + 1] = new CellStatus(
                            false,
                            false,
                            false
                        );
                        break;

                    case "1":
                        this._cellStatusArray[i + 1, j + 1] = new CellStatus(
                            true,
                            false,
                            false
                        );
                        break;

                    case "2":
                        this._cellStatusArray[i + 1, j + 1] = new CellStatus(
                            false,
                            true,
                            false
                        );
                        break;
                    
                    case "3":
                        this._cellStatusArray[i + 1, j + 1] = new CellStatus(
                            true,
                            true,
                            false
                        );
                        break;
                    
                    case "4":
                        this._cellStatusArray[i + 1, j + 1] = new CellStatus(
                            false,
                            true,
                            true
                        );
                        break;
                }
            }

            this._cellStatusArray[i + 1, this._cellStatusArray.GetLength(1)-  1] = new CellStatus(
                false,
                true,
                false
            );
        }

        for(int i=0; i<this._cellStatusArray.GetLength(0); i++)
        {
            this._cellStatusArray[this._cellStatusArray.GetLength(0) - 1, i] = new CellStatus(
                false,
                true,
                false
            );
        }
    
    }

    void CreateCell()
    {
        this._cellArray = new Cell[this._cellStatusArray.GetLength(0), this._cellStatusArray.GetLength(1)];

        for(int i=0; i<this._cellStatusArray.GetLength(0); i++)
        {
            for(int j=0; j<this._cellStatusArray.GetLength(1); j++)
            {
                GameObject cell = Instantiate(this._cellPrefab);
                cell.name = string.Format("({0}, {1})", i, j);
                cell.transform.localPosition = new Vector3(j, i, 0);
                this._cellArray[i, j] = cell.GetComponent<Cell>();
                this._cellArray[i, j].SetStatus(
                    this._cellStatusArray[i, j].isAlive,
                    this._cellStatusArray[i, j].isFixed,
                    this._cellStatusArray[i, j].isTarget
                );
            }
        }
    }

    public void ResetCell()
    {
        this._gamePhase = GamePhaseEnum.SETUP;

        for(int i=0; i<this._cellStatusArray.GetLength(0); i++)
        {
            for(int j=0; j<this._cellStatusArray.GetLength(1); j++)
            {
                this._cellArray[i, j].SetStatus(
                    this._cellStatusArray[i, j].isAlive,
                    this._cellStatusArray[i, j].isFixed,
                    this._cellStatusArray[i, j].isTarget
                );
            }
        }
    }
    public void ProgressCell()
    {
        if(!IsGameOver())
        {
            if(this._gamePhase == GamePhaseEnum.OVER)
            {
                return;
            }
            this._gamePhase = GamePhaseEnum.PLAY;

            CellStatus[,] nextCellStatusArray = new CellStatus[this._cellStatusArray.GetLength(0), this._cellStatusArray.GetLength(1)];
            
            int adjAliveCellCount;
            for(int i=0; i<this._cellStatusArray.GetLength(0) - 2; i++)
            {
                for(int j=0; j<this._cellStatusArray.GetLength(1) - 2; j++)
                {
                    adjAliveCellCount = (this._cellArray[i, j].IsAlive() ? 1 : 0)
                    + (this._cellArray[i + 1, j].IsAlive() ? 1 : 0)
                    + (this._cellArray[i + 2, j].IsAlive() ? 1 : 0)
                    + (this._cellArray[i, j + 1].IsAlive() ? 1 : 0)
                    + (this._cellArray[i + 2, j + 1].IsAlive() ? 1 : 0)
                    + (this._cellArray[i, j + 2].IsAlive() ? 1 : 0)
                    + (this._cellArray[i + 1, j + 2].IsAlive() ? 1 : 0)
                    + (this._cellArray[i + 2, j + 2].IsAlive() ? 1 : 0);

                    if(this._cellArray[i + 1, j + 1].IsAlive())
                    {
                        if(adjAliveCellCount < 2)
                        {
                            nextCellStatusArray[i + 1, j + 1] = new CellStatus(
                                false,
                                this._cellArray[i + 1, j + 1].IsFixed(),
                                this._cellArray[i + 1, j + 1].IsTarget()
                            );
                        }
                        else if(adjAliveCellCount >= 2 && adjAliveCellCount <= 3)
                        {
                            nextCellStatusArray[i + 1, j + 1] = new CellStatus(
                                true,
                                this._cellArray[i + 1, j + 1].IsFixed(),
                                false
                            );
                        }
                        else
                        {
                            nextCellStatusArray[i + 1, j + 1] = new CellStatus(
                                false,
                                this._cellArray[i + 1, j + 1].IsFixed(),
                                this._cellArray[i + 1, j + 1].IsTarget()
                            );
                        }
                    }
                    else
                    {
                        if(adjAliveCellCount == 3)
                        {
                            nextCellStatusArray[i + 1, j + 1] = new CellStatus(
                                true,
                                this._cellArray[i + 1, j + 1].IsFixed(),
                                false
                            );
                        }
                        else
                        {
                            nextCellStatusArray[i + 1, j + 1] = new CellStatus(
                                false,
                                this._cellArray[i + 1, j + 1].IsFixed(),
                                this._cellArray[i + 1, j + 1].IsTarget()
                            );
                        }
                    }

                }
            }

            for(int i=0; i<this._cellStatusArray.GetLength(0) - 2; i++)
            {
                for(int j=0; j<this._cellStatusArray.GetLength(1) - 2; j++)
                {
                    this._cellArray[i + 1, j + 1].SetStatus(
                        nextCellStatusArray[i + 1, j + 1].isAlive,
                        nextCellStatusArray[i + 1, j + 1].isFixed,
                        nextCellStatusArray[i + 1, j + 1].isTarget
                    );
                }
            }
        }
    }
    
    void Start()
    {
        this.LoadCellStatus();
        this.CreateCell();
    }

    void Update()
    {
        if(this._gamePhase == GamePhaseEnum.SETUP)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray selectionRay = Camera.main.ScreenPointToRay(
                    Input.mousePosition
                );
                RaycastHit selection;
                if(Physics.Raycast(selectionRay, out selection))
                {
                    Cell selectedCell = selection.transform.gameObject.GetComponent<Cell>();
                    if(!selectedCell.IsFixed())
                    {
                        selectedCell.ToggleStatus();
                    }
                }
            }
        }
    }
}
