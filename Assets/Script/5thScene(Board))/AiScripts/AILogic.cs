using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AILogic : MonoBehaviour
{
    public static AILogic instance;


    public enum Player { PlayerOne = 0, PlayerTwo }

    private BoardScript bs;
    private GameManagerScript gms;

    public List<int> suspectedList;
    public List<int> usedSuspectedList;

    void Awake()
    {
        //doesn't cut the background music that starts playing from the mainMenu
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        bs = FindObjectOfType<BoardScript>();
        gms = FindObjectOfType<GameManagerScript>();
    }

    private void Start()
    {
        suspectedList = new List<int>() { 1, 2, 2, 2, 2, 2, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 15 };
        usedSuspectedList = new List<int>();
    }
    /*
        private List<Position> playerOnePosition = new List<Position>();
        private List<Position> playerTwoPosition = new List<Position>();

        private int whoseTurnToMove; //0 - player1, 1 - player2
    */
    /*
     private int pieceID; //piece index - 1-15 flag,spy, .. 5 general
    private int pieceValue; //piece value 1 - 15; 15-spy, 1-flag
    private int row;
    private int column;
    private int playerIndex; //0 - player 1, 1 - player 2
    */
    public BoardState getCurrBoardState(List<GameObject> playerTwoPieceList, List<GameObject> playerOnePieceList)
    {
        BoardState currentState = new BoardState();
        int x = 1;
        //transfer all the datas to be represented as the initial BoardState
        foreach (var pos2 in playerTwoPieceList)
        {
            Position newPos = new Position();
            newPos.PieceID = x++; //flag == 1, spy == 15
            newPos.RealValue = pos2.GetComponent<GamePieceScript>().rank; //flag == 1, spy == 15
            newPos.PieceValue = newPos.RealValue; 
            newPos.Row = pos2.GetComponent<GamePieceScript>().piecePosition[0]; 
            newPos.Column = pos2.GetComponent<GamePieceScript>().piecePosition[1];
            newPos.PlayerIndex = 1;
            newPos.IsVisible = pos2.GetComponent<GamePieceScript>().isVisible;

            currentState.addFromPosList((int)Player.PlayerTwo, newPos);
        }
        int y = 1;
        foreach (var pos1 in playerOnePieceList)
        {
            Position newPos = new Position();
            newPos.PieceID = y++; //flag == 1, spy == 15
            newPos.RealValue = pos1.GetComponent<GamePieceScript>().rank; //flag == 1, spy == 15
            newPos.PieceValue = pos1.GetComponent<GamePieceScript>().suspectedRankValue;
            newPos.Row = pos1.GetComponent<GamePieceScript>().piecePosition[0];
            newPos.Column = pos1.GetComponent<GamePieceScript>().piecePosition[1];
            newPos.PlayerIndex = 0;
            newPos.IsVisible = pos1.GetComponent<GamePieceScript>().isVisible;

            currentState.addFromPosList((int)Player.PlayerOne, newPos);
        }

        currentState.WhoseTurnToMove = (int)Player.PlayerTwo;

        return currentState;
    }

    private int findPieceIndex(Position movingPos, List<GameObject> remainingPlayerTwoPieces)
    {
        for (int i = 0; i < remainingPlayerTwoPieces.Count; i++)
        {
            if(remainingPlayerTwoPieces[i].GetComponent<GamePieceScript>().piecePosition[0] == movingPos.Row && remainingPlayerTwoPieces[i].GetComponent<GamePieceScript>().piecePosition[1] == movingPos.Column)
            {
                return i;
            }
        }
        return 0;
    }

    public void AiMove(GameObject[] playerTwoPieceList, GameObject[] playerOnePieceList)
    {
        //int randomPieceIndex, randomMoveIndex;  //randomMove - 0-Up,1-Down,2-Left,3-Right
        Random rand = new Random();
        List<GameObject> remainingPlayerTwoPieces = new List<GameObject>();
        List<GameObject> remainingPlayerOnePieces = new List<GameObject>();

        //gets the remaining pieces for each side
        foreach (var item in playerTwoPieceList)
        {
            if (item.GetComponent<GamePieceScript>().isPlaced != false)
                remainingPlayerTwoPieces.Add(item);
        }
        foreach (var item in playerOnePieceList)
        {
            if (item.GetComponent<GamePieceScript>().isPlaced != false)
                remainingPlayerOnePieces.Add(item);
        }

        Debug.Log("GOBJECTS: " + remainingPlayerTwoPieces.Count);
        //Creates an Adjacency graph
        //AdjacencyGraph A = new AdjacencyGraph();

        /*for (int x = 0; x < remainingPlayerTwoPieces.Count; x++)
        {
            A.addVertex(remainingPlayerTwoPieces[x], returnPossibleMoves(remainingPlayerTwoPieces[x].GetComponent<GamePieceScript>().piecePosition, bs.occupiedPos));
        }*/

        Debug.Log("GOBJECTS: " + remainingPlayerTwoPieces.Count);

        //start MONTE CARLO SEARCH
        BoardState currentBoardState = getCurrBoardState(remainingPlayerTwoPieces, remainingPlayerOnePieces);
        Debug.Log("playerOne: " + currentBoardState.getPositionSize(0));
        Debug.Log("playerTwo: " + currentBoardState.getPositionSize(1));
        BoardState resultBoardState = currentBoardState.addEvalScoreAndGetBest();

        int pieceIndex = findPieceIndex(resultBoardState.sourcePosition, remainingPlayerTwoPieces);
        int moveIndex = resultBoardState.moveUnit;

        /*
        //this is all for the random movement == START
        //Creates an Adjacency graph
        AdjacencyGraph A = new AdjacencyGraph();
        for (int x = 0; x < remainingPlayerTwoPieces.Count; x++)
        {
            A.addVertex(remainingPlayerTwoPieces[x], returnPossibleMoves(remainingPlayerTwoPieces[x].GetComponent<GamePieceScript>().piecePosition, bs.occupiedPos));
        }

        //removes the vertex with no connections
        for (int x = 0; x < remainingPlayerTwoPieces.Count; x++)
        {
            if (A.possibleMoves(remainingPlayerTwoPieces[x]) == null || A.possibleMoves(remainingPlayerTwoPieces[x]).Count == 0)
            {
                remainingPlayerTwoPieces.Remove(remainingPlayerTwoPieces[x--]);
                continue;
            }
            //Debug.Log("Name: " + remainingPieces[x].GetComponent<GamePieceScript>().rankName + " Count = " + A.possibleMoves(remainingPieces[x]).Count);
        }

         //random picking of object and moves
        randomPieceIndex = rand.Next(0, remainingPlayerTwoPieces.Count);
        randomMoveIndex = rand.Next(0, A.possibleMoves(remainingPlayerTwoPieces[randomPieceIndex]).Count);

        int pieceIndex = randomPieceIndex;
        int moveIndex = randomMoveIndex;

        
        move(remainingPlayerTwoPieces[pieceIndex], A.A[remainingPlayerTwoPieces[pieceIndex]][moveIndex]);   //1st param is object, 2nd param is the possible moves
        gms.setTurn("human");
        bs.turnOnInteractable();

        //Debug.Log("Number of Vertex: " + remainingPieces.Count);
        //Debug.Log("Vertex Name: " + remainingPieces[pieceIndex].GetComponent<GamePieceScript>().rankName);
        //this is all for the random movement == END
        */

        /*//removes the vertex with no connections for playerTwo
        for (int x = 0; x < remainingPlayerTwoPieces.Count; x++)
        {
            if (A.possibleMoves(remainingPlayerTwoPieces[x]).Count == 0)
            {
                /*remainingPlayerTwoPieces.Remove(remainingPlayerTwoPieces[x--]);
                continue;
                remainingPlayerTwoPieces.RemoveAt(x);
                x = -1;
            }
            //Debug.Log("Name: " + remainingPieces[x].GetComponent<GamePieceScript>().rankName + " Count = " + A.possibleMoves(remainingPieces[x]).Count);
        }*/

        move(remainingPlayerTwoPieces[pieceIndex], moveIndex);   //1st param is object, 2nd param is the possible moves
        gms.setTurn("human");
        bs.turnOnInteractable();

        /*Debug.Log("ARRAY OF PIECE Positions");
        foreach (var item in pieceList)
        {
            Debug.Log("Piece: " + item.GetComponent<GamePieceScript>().rankName + " Position: " + item.GetComponent<GamePieceScript>().piecePosition[0] + ":" + item.GetComponent<GamePieceScript>().piecePosition[1]);
        }
        Debug.Log("ARRAY OF Occupied Positions");
        for (int x = 0; x < bs.occupiedPos.GetLength(0); x++)
        {
            for (int y = 0; y < bs.occupiedPos.GetLength(1); y++)
            {
                Debug.Log(" Occupied Position: " + x + ":" + y + " = " + bs.occupiedPos[x, y]);
            }
        }*/
    }

    //this needs the GameObject piece and it's move index (0-up, 1-down, 2-left, 3-right)
    private void move(GameObject aiPiece, int moveIndex)
    {
        Debug.Log("Ai's Piece: " + aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ":" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1] + " Move: " + moveIndex);
        int posX = aiPiece.GetComponent<GamePieceScript>().piecePosition[0];
        int posY = aiPiece.GetComponent<GamePieceScript>().piecePosition[1];
        int aiPieceRank = bs.objectAndPos[posX + "" + posY].GetComponent<GamePieceScript>().rank;
        string aiPieceType = bs.objectAndPos[posX + "" + posY].GetComponent<GamePieceScript>().playerType;
        int opposingPieceRank = 0;
        string opposingPieceType = "none";
        string newPos = "none";
        int newPosX = posX, newPosY = posY;
        GameObject opposingPiece = null;

        //0-up, 1-down, 2-left, 3-right
        switch (moveIndex)
        {
            case 0:
                {
                    opposingPiece = bs.objectAndPos[(posX - 1) + "" + posY];
                    newPos = (posX - 1) + "" + posY;
                    newPosX = posX - 1;
                    if (opposingPiece == null)
                        break;
                    opposingPieceRank = bs.objectAndPos[(posX - 1) + "" + posY].GetComponent<GamePieceScript>().rank;
                    opposingPieceType = bs.objectAndPos[(posX - 1) + "" + posY].GetComponent<GamePieceScript>().playerType;
                    break;
                }
            case 1:
                {
                    opposingPiece = bs.objectAndPos[(posX + 1) + "" + posY];
                    newPos = (posX + 1) + "" + posY;
                    newPosX = posX + 1;
                    if (opposingPiece == null)
                        break;
                    opposingPieceRank = bs.objectAndPos[(posX + 1) + "" + posY].GetComponent<GamePieceScript>().rank;
                    opposingPieceType = bs.objectAndPos[(posX + 1) + "" + posY].GetComponent<GamePieceScript>().playerType;
                    break;
                }
            case 2:
                {
                    opposingPiece = bs.objectAndPos[posX + "" + (posY - 1)];
                    newPos = posX + "" + (posY - 1);
                    newPosY = posY - 1;
                    if (opposingPiece == null)
                        break;
                    opposingPieceRank = bs.objectAndPos[posX + "" + (posY - 1)].GetComponent<GamePieceScript>().rank;
                    opposingPieceType = bs.objectAndPos[posX + "" + (posY - 1)].GetComponent<GamePieceScript>().playerType;
                    break;
                }
            case 3:
                {
                    opposingPiece = bs.objectAndPos[posX + "" + (posY + 1)];
                    newPos = posX + "" + (posY + 1);
                    newPosY = posY + 1;
                    if (opposingPiece == null)
                        break;
                    opposingPieceRank = bs.objectAndPos[posX + "" + (posY + 1)].GetComponent<GamePieceScript>().rank;
                    opposingPieceType = bs.objectAndPos[posX + "" + (posY + 1)].GetComponent<GamePieceScript>().playerType;
                    break;
                }
        }


        if (opposingPiece != null)
        {
            //when a private attacks a spy
            if (aiPieceRank == 2 && opposingPieceRank == 15 &&
                aiPieceType != opposingPieceType)
            {
                Debug.Log("Ai Private eats Spy");

                bs.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                bs.objectAndPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = aiPiece.gameObject;
                //eat this object
                opposingPiece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
                                                                                  //set the occupiedPosition of the board
                bs.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0],
                    aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                bs.occupiedPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0],
                    opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = true;
                bs.tileObjectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
                bs.tileObjectAndPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = true;
                //sets the piece's isDead to true
                opposingPiece.GetComponent<GamePieceScript>().isDead = true;
                //to set suspectedValue
                opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue = 15;
                instance.suspectedList.Remove(opposingPieceRank);
                instance.usedSuspectedList.Add(opposingPieceRank);
                aiPiece.GetComponent<GamePieceScript>().isVisible = true;
                //replace the new piece to the tile
                aiPiece.GetComponent<RectTransform>().anchoredPosition = opposingPiece.GetComponent<RectTransform>().anchoredPosition;
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = opposingPiece.GetComponent<GamePieceScript>().piecePosition[0];
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = opposingPiece.GetComponent<GamePieceScript>().piecePosition[1];
                aiPiece.GetComponent<GamePieceScript>().isPlaced = true;  //new piece is placed
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
                //opposingPiece.SetActive(false); //removes the eaten piece
                bs.transferDeadPiece(opposingPiece.gameObject);
                /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rankName);
                Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rankName);*/
            }
            //when a spy attacks a private
            else if (aiPieceRank == 15 && opposingPieceRank == 2 &&
                aiPieceType != opposingPieceType)
            {
                Debug.Log("Ai spy eats private");
                bs.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                //set the occupiedPosition of the board
                bs.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0], aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                bs.tileObjectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
                //sets the piece's isDead to true
                aiPiece.GetComponent<GamePieceScript>().isDead = true;
                //to set suspectedValue
                opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue = 2;
                instance.suspectedList.Remove(aiPieceRank);
                instance.usedSuspectedList.Add(aiPieceRank);
                //eat this object
                aiPiece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
                //replace the new piece to the tile
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
                //aiPiece.SetActive(false); //removes the eaten piece
                bs.transferDeadPiece(aiPiece.gameObject);
                /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rankName);
                Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rankName);*/
            }
            //when the attacking piece is stronger
            else if (aiPieceRank > opposingPieceRank &&
                aiPieceType != opposingPieceType)
            {
                Debug.Log("Ai strong eats weak");
                bs.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                bs.objectAndPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = aiPiece.gameObject;
                //set the occupiedPosition of the board
                bs.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0], aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                bs.occupiedPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0], opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = true;
                bs.tileObjectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
                bs.tileObjectAndPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = true;
                //sets the piece's isDead to true
                opposingPiece.GetComponent<GamePieceScript>().isDead = true;
                //to set suspectedValue
                opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue = instance.suspectAgain(opposingPiece.gameObject, aiPiece.GetComponent<GamePieceScript>().rank, false);
                instance.suspectedList.Remove(opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue);
                instance.usedSuspectedList.Add(opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue);
                if (aiPiece.GetComponent<GamePieceScript>().rank == 15 && opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue == 14)
                {
                    aiPiece.GetComponent<GamePieceScript>().isVisible = true;
                }
                //eat this object
                opposingPiece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
                //replace the new piece to the tile
                aiPiece.GetComponent<RectTransform>().anchoredPosition = opposingPiece.GetComponent<RectTransform>().anchoredPosition;
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = opposingPiece.GetComponent<GamePieceScript>().piecePosition[0];
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = opposingPiece.GetComponent<GamePieceScript>().piecePosition[1];
                aiPiece.GetComponent<GamePieceScript>().isPlaced = true;  //new piece is placed
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
                //
                if(aiPieceRank == 15 && opposingPieceRank == 14)
                {
                    foreach (var item in bs.enemyPiecesList)
                    {
                        if(item.GetComponent<GamePieceScript>().rank == 2)
                        {
                            item.GetComponent<GamePieceScript>().suspectedRankValue = 2;
                        }
                    }
                }
                //opposingPiece.SetActive(false); //removes the eaten piece
                bs.transferDeadPiece(opposingPiece.gameObject);
                if (opposingPiece.GetComponent<GamePieceScript>().rankName == "Flag")
                {
                    bs.GameCondition(aiPiece.GetComponent<GamePieceScript>().playerType); //if both are flags
                }
                /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rank);
                Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rank);*/
            }
            //when the attacking piece is weaker
            else if (aiPieceRank < opposingPieceRank &&
                aiPieceType != opposingPieceType)
            {
                Debug.Log("Ai weak eats strong");
                bs.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                //set the occupiedPosition of the board
                bs.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0], aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                bs.tileObjectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
                //sets the piece's isDead to true
                aiPiece.GetComponent<GamePieceScript>().isDead = true;
                //to set suspectedValue
                opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue = instance.suspectAgain(opposingPiece.gameObject, aiPiece.GetComponent<GamePieceScript>().rank, true);
                instance.suspectedList.Remove(aiPieceRank);
                instance.usedSuspectedList.Add(aiPieceRank);
                //eat this object
                aiPiece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
                //replace the new piece to the tile
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
                //aiPiece.SetActive(false); //removes the eaten piece
                bs.transferDeadPiece(aiPiece.gameObject);
                if (aiPiece.GetComponent<GamePieceScript>().rankName == "Flag")
                {
                    bs.GameCondition(opposingPiece.GetComponent<GamePieceScript>().playerType); //if both are flags
                }
                /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rank);
                Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rank);*/
            }
            //when both piece have the same rank
            else if (aiPieceRank == opposingPieceRank &&
                aiPieceType != opposingPieceType)
            {
                Debug.Log("Ai same eats same");
                bs.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                bs.objectAndPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                //set the occupiedPosition of the board
                bs.occupiedPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0], aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                bs.occupiedPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0], opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]] = false;
                bs.tileObjectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
                bs.tileObjectAndPos[opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + opposingPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = false;
                //sets the piece's isDead to true
                aiPiece.GetComponent<GamePieceScript>().isDead = true;
                opposingPiece.GetComponent<GamePieceScript>().isDead = true;
                //to set suspectedValue
                opposingPiece.GetComponent<GamePieceScript>().suspectedRankValue = opposingPieceRank;
                opposingPiece.GetComponent<GamePieceScript>().suspectIsSure = true;
                if(!suspectedList.Contains(opposingPieceRank))
                {
                    foreach (var item in bs.playerPiecesList)
                    {
                        if(item.GetComponent<GamePieceScript>().suspectedRankValue == opposingPieceRank && !item.GetComponent<GamePieceScript>().suspectIsSure)
                        {
                            if(instance.suspectedList.Count == 2)
                            {
                                item.GetComponent<GamePieceScript>().suspectedRankValue = suspectedList[1];
                                item.GetComponent<GamePieceScript>().suspectIsSure = true;
                            }
                            else
                            {
                                item.GetComponent<GamePieceScript>().suspectedRankValue = 0;
                            }
                        }
                    }
                }
                else
                {
                    instance.suspectedList.Remove(opposingPieceRank);
                    instance.usedSuspectedList.Add(opposingPieceRank);
                }
                //eat this object
                aiPiece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
                aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
                //aiPiece.gameObject.SetActive(false); //removes the eaten piece
                opposingPiece.GetComponent<GamePieceScript>().isPlaced = false;   //not include in the board
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[0] = -1; //removes from the board
                opposingPiece.GetComponent<GamePieceScript>().piecePosition[1] = -1; //removes from the board
                //opposingPiece.SetActive(false); //removes the eaten piece
                bs.transferDeadPiece(aiPiece.gameObject);
                bs.transferDeadPiece(opposingPiece.gameObject);
                if (aiPiece.GetComponent<GamePieceScript>().rankName == "Flag")
                {
                    bs.GameCondition(aiPiece.GetComponent<GamePieceScript>().playerType); //if both are flags
                }
                /*Debug.Log("Top: " + piece.GetComponent<GamePieceScript>().rankName);
                Debug.Log("Bottom: " + GetComponent<GamePieceScript>().rankName);*/
            }
        }
        else
        {
            Debug.Log("Ai normal move");
            /*
            bs.objectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
            */
            bs.objectAndPos[posX + "" + posY] = null;
            bs.objectAndPos[newPos] = aiPiece.gameObject;

            aiPiece.GetComponent<RectTransform>().anchoredPosition = bs.tileObjectAndPos[newPosX + "" + newPosY].GetComponent<RectTransform>().anchoredPosition; //anchoredPosition is the pivot/origin of the object
            bs.occupiedPos[posX, posY] = false;
            bs.occupiedPos[newPosX, newPosY] = true;
            aiPiece.GetComponent<GamePieceScript>().isPlaced = true;
            aiPiece.GetComponent<GamePieceScript>().piecePosition[0] = newPosX;
            aiPiece.GetComponent<GamePieceScript>().piecePosition[1] = newPosY;
            bs.tileObjectAndPos[aiPiece.GetComponent<GamePieceScript>().piecePosition[0] + "" + aiPiece.GetComponent<GamePieceScript>().piecePosition[1]].GetComponent<TileScript>().occupied = true;
            if (aiPiece.GetComponent<GamePieceScript>().rankName == "Flag" && aiPiece.GetComponent<GamePieceScript>().playerType == "ai2" && 
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] == 7)
            {
                bs.GameCondition(aiPiece.GetComponent<GamePieceScript>().playerType);
            }
            else if (aiPiece.GetComponent<GamePieceScript>().rankName == "Flag" && aiPiece.GetComponent<GamePieceScript>().playerType == "ai1" &&
                aiPiece.GetComponent<GamePieceScript>().piecePosition[0] == 0)
            {
                bs.GameCondition(aiPiece.GetComponent<GamePieceScript>().playerType);
            }
        }
        //change side and turn off interactable for all the player's pieces
        gms.setTurn("human");

    }

    public int getSuspectedValue(int aiPieceRank, bool isEaten)
    {
        List<int> possible = new List<int>();
        int suspectedValue = 0;
        if(!isEaten)
        {
            for(int i = 2; i < aiPieceRank; i++)
            {
                if(instance.suspectedList.Contains(i))
                {
                    possible.Add(i);
                }
            }
            if (possible.Count == 0)
                return 0;
            suspectedValue = possible[possible.Count < 2 ? 0 : possible.Count / 2 -1];
        }
        else
        {
            for (int i = aiPieceRank + 1; i < (aiPieceRank == 2 ? 15 : 16); i++)
            {
                if (instance.suspectedList.Contains(i))
                {
                    possible.Add(i);
                }
            }
            if (possible.Count == 0)
                return 0;
            suspectedValue = possible[possible.Count < 2 ? 0 : possible.Count / 2 - 1];
        }
        return suspectedValue;
    }


    public int suspectAgain(GameObject piece, int aiPieceRank, bool isEaten)
    {
        int currentValue = instance.getSuspectedValue(aiPieceRank, isEaten);
        if (piece.GetComponent<GamePieceScript>().suspectedRankValue != 0 && currentValue > piece.GetComponent<GamePieceScript>().suspectedRankValue)
        {
            instance.suspectedList.Add(piece.GetComponent<GamePieceScript>().suspectedRankValue);
            instance.usedSuspectedList.Remove(piece.GetComponent<GamePieceScript>().suspectedRankValue);
        }
        else if(piece.GetComponent<GamePieceScript>().suspectedRankValue != 0 && currentValue <= piece.GetComponent<GamePieceScript>().suspectedRankValue)
        {
            currentValue = piece.GetComponent<GamePieceScript>().suspectedRankValue;
        }
        
        return currentValue;
    }

    private List<int> returnPossibleMoves(int[] piecePos, bool[,] occupiedPos)
    {
        string typeUp = "none", typeDown = "none", typeLeft = "none", typeRight = "none";
        GameObject [] sample = new GameObject[4] { null,null,null,null};
        List<int> li = new List<int>();

        //going up
        if (piecePos[0] - 1 >= 0)
        {
            sample[0] = bs.objectAndPos[(piecePos[0] - 1) + "" + piecePos[1]];
        }
        //going Down
        if (piecePos[0] + 1 < occupiedPos.GetLength(0))
        {
            sample[1] = bs.objectAndPos[(piecePos[0] + 1) + "" + piecePos[1]];
        }
        //going Left
        if (piecePos[1] - 1 >= 0)
        {
            sample[2] = bs.objectAndPos[piecePos[0] + "" + (piecePos[1] - 1)];
        }
        //going Right
        if (piecePos[1] + 1 < occupiedPos.GetLength(1))
        {
            sample[3] = bs.objectAndPos[piecePos[0] + "" + (piecePos[1] + 1)];
        }
            //checks if the tilePosition to traverse has a piece; determines the type of the piece
        if(sample[0] != null)
            typeUp = sample[0].GetComponent<GamePieceScript>().playerType;
        if (sample[1] != null)
            typeDown = sample[1].GetComponent<GamePieceScript>().playerType;
        if (sample[2] != null)
            typeLeft = sample[2].GetComponent<GamePieceScript>().playerType;
        if (sample[3] != null)
            typeRight = sample[3].GetComponent<GamePieceScript>().playerType;

        
        //going Up
        if ((true && piecePos[0] - 1 >= 0) && (typeUp == "human" || occupiedPos[piecePos[0] - 1, piecePos[1]] != true))
            li.Add(0);
        //going Down
        if ((true && piecePos[0] + 1 < occupiedPos.GetLength(0)) && (typeDown == "human" || occupiedPos[piecePos[0] + 1, piecePos[1]] != true))
            li.Add(1);
        //going Left
        if ((true && piecePos[1] - 1 >= 0) && (typeLeft == "human" || occupiedPos[piecePos[0], piecePos[1] - 1] != true))
            li.Add(2);
        //going Right
        if ((true && piecePos[1] + 1 < occupiedPos.GetLength(1)) && (typeRight == "human" || occupiedPos[piecePos[0], piecePos[1] + 1] != true))
            li.Add(3);

        return li;
    }

    public void randomPreConstructionAI(int dFieldStartX, int dFieldMaxX, GameObject[] pieceList)
    {
        int x;
        int posX, posY;
        Random rnd = new Random();
        for (x = 0; x < pieceList.Length; x++)
        {
            posX = rnd.Next(0, bs.tileList.GetLength(0)/2-1);
            posY = rnd.Next(0, bs.tileList.GetLength(1));
            if (bs.occupiedPos[posX,posY] != true)
            {
                setPiece(pieceList, posX, posY, x);
            }
            else
            {
                --x;
                continue;
            }
        }
    }

    //{ 1, 2, 2, 3, 3, 3, 3, 3, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

    //below is the ordering of the pieceList
    /*{ "1Flag", "2Spy", "3Private", "4Sergeant", "52nd Lieutenant", "61st Lieutenant", 
        "7Captain", "8Major", "9Lieutenant Colonel", "10Colonel", "11One-Star General", 
        "12Two-Star General", "13Three-Star General", "14Four-Star General", "15Five-Star General"} 1 - 15 piece index */

    public void bestPreConstructionAI(GameObject[] pieceList)
    {
        Random rand = new Random();
        int x = rand.Next(0, 3);

        switch(x)
        {
            case 0:
                setPiece(pieceList, 0, 1, 0);
                setPiece(pieceList, 1, 1, 1);
                setPiece(pieceList, 1, 7, 2);
                setPiece(pieceList, 1, 2, 3);
                setPiece(pieceList, 1, 4, 4);
                setPiece(pieceList, 1, 6, 5);
                setPiece(pieceList, 2, 0, 6);
                setPiece(pieceList, 2, 5, 7);
                setPiece(pieceList, 2, 8, 8);
                setPiece(pieceList, 0, 2, 9);
                setPiece(pieceList, 1, 0, 10);
                setPiece(pieceList, 1, 8, 11);
                setPiece(pieceList, 0, 3, 12);
                setPiece(pieceList, 1, 3, 13);
                setPiece(pieceList, 1, 5, 14);
                setPiece(pieceList, 2, 3, 15);
                setPiece(pieceList, 2, 6, 16);
                setPiece(pieceList, 2, 2, 17);
                setPiece(pieceList, 2, 1, 18);
                setPiece(pieceList, 2, 7, 19);
                setPiece(pieceList, 2, 4, 20);
                break;
            case 1:
                setPiece(pieceList, 0, 4, 0);
                setPiece(pieceList, 0, 0, 1);
                setPiece(pieceList, 0, 8, 2);
                setPiece(pieceList, 1, 2, 3);
                setPiece(pieceList, 1, 3, 4);
                setPiece(pieceList, 1, 5, 5);
                setPiece(pieceList, 1, 7, 6);
                setPiece(pieceList, 1, 4, 7);
                setPiece(pieceList, 1, 6, 8);
                setPiece(pieceList, 2, 6, 9);
                setPiece(pieceList, 2, 3, 10);
                setPiece(pieceList, 2, 7, 11);
                setPiece(pieceList, 2, 1, 12);
                setPiece(pieceList, 2, 8, 13);
                setPiece(pieceList, 0, 2, 14);
                setPiece(pieceList, 2, 0, 15);
                setPiece(pieceList, 0, 6, 16);
                setPiece(pieceList, 2, 4, 17);
                setPiece(pieceList, 2, 5, 18);
                setPiece(pieceList, 1, 1, 19);
                setPiece(pieceList, 2, 2, 20);
                break;
            case 2:
                setPiece(pieceList, 0, 4, 0);
                setPiece(pieceList, 0, 8, 1);
                setPiece(pieceList, 1, 2, 2);
                setPiece(pieceList, 1, 3, 3);
                setPiece(pieceList, 1, 4, 4);
                setPiece(pieceList, 1, 5, 5);
                setPiece(pieceList, 1, 6, 6);
                setPiece(pieceList, 1, 0, 7);
                setPiece(pieceList, 1, 8, 8);
                setPiece(pieceList, 2, 0, 9);
                setPiece(pieceList, 2, 3, 10);
                setPiece(pieceList, 2, 1, 11);
                setPiece(pieceList, 2, 8, 12);
                setPiece(pieceList, 2, 7, 13);
                setPiece(pieceList, 2, 6, 14);
                setPiece(pieceList, 0, 0, 15);
                setPiece(pieceList, 2, 5, 16);
                setPiece(pieceList, 1, 1, 17);
                setPiece(pieceList, 1, 7, 18);
                setPiece(pieceList, 2, 4, 19);
                setPiece(pieceList, 2, 2, 20);
                break;
        }
    }

    private void setPiece(GameObject[] pieceList, int posX, int posY, int index)
    {
        bs.tileObjectAndPos[posX + "" + posY].GetComponent<TileScript>().occupied = true;

        pieceList[index].GetComponent<GamePieceScript>().piecePosition[0] = posX;
        pieceList[index].GetComponent<GamePieceScript>().piecePosition[1] = posY;
        bs.occupiedPos[posX, posY] = true;
        pieceList[index].GetComponent<GamePieceScript>().isPlaced = true;
        pieceList[index].GetComponent<RectTransform>().anchoredPosition =
            bs.tileList[posX, posY].GetComponent<RectTransform>().anchoredPosition;
        bs.objectAndPos[posX + "" + posY] = pieceList[index];
    }

}
