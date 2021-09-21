using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    public class DeadTiles
    {
        public bool occupied = false;
        public GameObject piece;
        public float[] position = new float[2];
        public GameObject deadTileObject;

        public DeadTiles(float[]position)
        {
            this.position = position;
        }
    };

    private AILogic aiLog;

    public GameObject tileObject;   //a prefab of a tile
    public GameObject pieceObject;   //a prefab of a piece
    [HideInInspector] public GameObject[,] tileList = new GameObject[8,9]; //the tiles represented in an array
    [HideInInspector] public GameObject[] playerPiecesList = new GameObject[21]; //player's pieces gameobject
    [HideInInspector] public GameObject[] enemyPiecesList = new GameObject[21]; //player's pieces gameobject
    [HideInInspector] public int[] gamePiecesIndex = new int[21]; /*the number of pieces in each side; 
    sorted in increasing order(weakest(flag) to strongest rank(5 star))*/
    [HideInInspector] public bool[,] occupiedPos = new bool[8, 9]; //the state of the tiles if its occupied; 0 or 1 if occupied
    [HideInInspector] public Dictionary<string, GameObject> objectAndPos = new Dictionary<string, GameObject>();
    [HideInInspector] public Dictionary<string, GameObject> tileObjectAndPos = new Dictionary<string, GameObject>();

    float posX = -414.0f + 960.0f;  //position x of the tiles;2nd param is the offset of the default pos
    float posY = 408.0f + 540.5f;  //position y of the tiles;2nd param is the offset of the default pos
    float gapLength = 105.0f; //for tiles

    float posXPieces = -864.0f + 960.0f;  //position x of the pieces;2nd param is the offset of the default pos
    float posYPieces = 200.0f + 540.5f;  //position y of the pieces;2nd param is the offset of the default pos

    Vector3 pos; //pos for tiles
    Vector3 posPieces; //pos for pieces

    private GameManagerScript gms;

    public List<DeadTiles> blackGraveyard = new List<DeadTiles>();
    public List<DeadTiles> whiteGraveyard = new List<DeadTiles>();

    // Start is called before the first frame update

    void Awake()
    {
        aiLog = FindObjectOfType<AILogic>();
        gms = FindObjectOfType<GameManagerScript>();
    }

    private void Start()
    {
        int x, y;
        for (x = 0; x < tileList.GetLength(0); x++) //thank you
        {
            for (y = 0; y < tileList.GetLength(1); y++)
            {
                objectAndPos.Add(x + "" + y, null);
            }
        }
        pos = new Vector3(posX, posY, 0);
        setDeadTiles();
        instantiateTiles();
        gamePiecesIndex = new int[21] { 1, 2, 2, 3, 3, 3, 3, 3, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }; //15-FiveStar Gen to 1-Flag
        posPieces = new Vector3(posXPieces, posYPieces, 0);
        instantiatePieces();
    }

    private void Update()
    {
        //Debug.Log(playerPiecesList.Length + ":" + enemyPiecesList.Length);
    }

    void setDeadTiles()
    {
        //for blackGraveyard
        float deadPosXPieces = -880.0f + 960.0f;  //position x of the pieces;2nd param is the offset of the default pos
        float deadPposYPieces = 200.0f + 540.5f;  //position y of the pieces;2nd param is the offset of the default pos
        for (int x = 0; x < 21; x++)
        {
            DeadTiles sample = new DeadTiles( new float[2] { deadPosXPieces, deadPposYPieces } );
            sample.deadTileObject = Instantiate(tileObject, pos, Quaternion.identity);
            sample.deadTileObject.transform.position = new Vector3(deadPosXPieces, deadPposYPieces, 0);
            deadPosXPieces += 100.0f;
            if (x % 4 == 0)
            {
                deadPosXPieces = -880.0f + 960.0f;
                deadPposYPieces -= 100.0f;
            }
            blackGraveyard.Add(sample);
            blackGraveyard[x].deadTileObject.SetActive(false);
        }
        //for whiteGraveyard
        deadPosXPieces = 550.0f + 960.0f;  //position x of the pieces;2nd param is the offset of the default pos
        deadPposYPieces = 200.0f + 540.5f;  //position y of the pieces;2nd param is the offset of the default pos
        for (int x = 0; x < 21; x++)
        {
            DeadTiles sample = new DeadTiles(new float[2] { deadPosXPieces, deadPposYPieces });
            sample.deadTileObject = Instantiate(tileObject, pos, Quaternion.identity);
            sample.deadTileObject.transform.position = new Vector3(deadPosXPieces, deadPposYPieces, 0);
            deadPosXPieces += 100.0f;
            if (x % 4 == 0)
            {
                deadPosXPieces = 550.0f + 960.0f;
                deadPposYPieces -= 100.0f;
            }
            whiteGraveyard.Add(sample);
            whiteGraveyard[x].deadTileObject.SetActive(false);
        }
    }

    public void transferDeadPiece(GameObject deadPiece)
    {
        if (deadPiece.GetComponent<GamePieceScript>().playerType == "human")
        {
            for (int x = 0; x < whiteGraveyard.Count; x++)
            {
                if(whiteGraveyard[x].occupied != true)
                {
                    whiteGraveyard[x].occupied = true;
                    whiteGraveyard[x].piece = deadPiece;
                    deadPiece.GetComponent<RectTransform>().anchoredPosition = whiteGraveyard[x].deadTileObject.GetComponent<RectTransform>().anchoredPosition;
                    break;
                }
            }
        }
        else if (deadPiece.GetComponent<GamePieceScript>().playerType == "ai1")
        {
            for (int x = 0; x < whiteGraveyard.Count; x++)
            {
                if (whiteGraveyard[x].occupied != true)
                {
                    whiteGraveyard[x].occupied = true;
                    whiteGraveyard[x].piece = deadPiece;
                    deadPiece.GetComponent<RectTransform>().anchoredPosition = whiteGraveyard[x].deadTileObject.GetComponent<RectTransform>().anchoredPosition;
                    break;
                }
            }
        }
        else if (deadPiece.GetComponent<GamePieceScript>().playerType == "ai2")
        {
            for (int x = 0; x < blackGraveyard.Count; x++)
            {
                if (blackGraveyard[x].occupied != true)
                {
                    blackGraveyard[x].occupied = true;
                    blackGraveyard[x].piece = deadPiece;
                    deadPiece.GetComponent<RectTransform>().anchoredPosition = blackGraveyard[x].deadTileObject.GetComponent<RectTransform>().anchoredPosition;
                    break;
                }
            }
        }
    }

    //instantiate the tiles in their corresponding positions
    public void instantiateTiles()
    {
        int x, y;
        for (x = 0; x < tileList.GetLength(0) / 2; x++)
        {
            for (y = 0; y < tileList.GetLength(1); y++)
            {
                tileList[x, y] = Instantiate(tileObject, pos, Quaternion.identity);
                tileList[x, y].GetComponent<TileScript>().tilePosition[0] = x;
                tileList[x, y].GetComponent<TileScript>().tilePosition[1] = y;
                //Debug.LogError("TilePosition X: " + tileList[x,y].GetComponent<TileScript>().tilePosition[0]);
                tileList[x, y].transform.position = pos;
                if (y >= 2)
                    pos.x += gapLength - 1.0f;
                else
                    pos.x += gapLength;
            }
            pos.y -= gapLength;
            pos.x = posX;
        }

        posX = -415.0f + 960.0f;  //2nd param is the offset of the default pos
        posY = -66.0f + 540.0f;  //2nd param is the offset of the default pos
        pos = new Vector3(posX, posY, 0);

        for (; x < tileList.GetLength(0); x++)
        {
            for (y = 0; y < tileList.GetLength(1); y++)
            {
                tileList[x, y] = Instantiate(tileObject, pos, Quaternion.identity);
                tileList[x, y].GetComponent<TileScript>().tilePosition[0] = x;
                tileList[x, y].GetComponent<TileScript>().tilePosition[1] = y;
                tileList[x, y].transform.position = pos;
                if (y >= 2)
                    pos.x += gapLength - 1.0f;
                else
                    pos.x += gapLength;
            }
            pos.y -= gapLength;
            pos.x = posX;
        }

        for (x = 0; x < tileList.GetLength(0); x++)
        {
            for (y = 0; y < tileList.GetLength(1); y++)
            {
                tileObjectAndPos.Add(x + "" + y, tileList[x, y]);
            }
        }
    }

    public void instantiatePieces()
    {
        int x;
        for (x = 0; x < playerPiecesList.Length; x++)
        {
            playerPiecesList[x] = Instantiate(pieceObject, posPieces, Quaternion.identity);
            playerPiecesList[x].transform.position = posPieces;
            //humanvsai or aivsai
            if(gms.gameMode == "humanvsai")
            {
                playerPiecesList[x].GetComponent<GamePieceScript>().rankImageLoad =
                    GameObject.FindObjectOfType<GameManagerScript>().gamePiecesImagesW[x];
                playerPiecesList[x].GetComponent<GamePieceScript>().playerType = "human";
            }
            else if (gms.gameMode == "aivsai")
            {
                playerPiecesList[x].GetComponent<GamePieceScript>().rankImageLoad = gms.gamePiecesImagesW[gms.gamePiecesImagesW.Length-1];
                playerPiecesList[x].GetComponent<GamePieceScript>().playerType = "ai1";
            }
            playerPiecesList[x].GetComponent<GamePieceScript>().pieceIndex = gamePiecesIndex[x];

            posPieces.x += 80.0f;
            if (x % 4 == 0)
            {
                posPieces.x = posXPieces;
                posPieces.y -= 100.0f;
            }
        }

        posXPieces = 614.0f + 960.0f;
        posPieces = new Vector3(posXPieces, posYPieces, 0);

        for (x = 0; x < enemyPiecesList.Length; x++)
        {
            enemyPiecesList[x] = Instantiate(pieceObject, posPieces, Quaternion.identity);
            enemyPiecesList[x].transform.position = posPieces;
            //back Image; change: gms.gamePiecesImagesB[gms.gamePiecesImagesB.Length - 1] : gms.gamePiecesImagesB[x]
            enemyPiecesList[x].GetComponent<GamePieceScript>().rankImageLoad = gms.gamePiecesImagesB[gms.gamePiecesImagesB.Length - 1];
            enemyPiecesList[x].GetComponent<GamePieceScript>().playerType = "ai2";
            enemyPiecesList[x].GetComponent<GamePieceScript>().pieceIndex = gamePiecesIndex[x];
            posPieces.x += 80.0f;
            if (x % 4 == 0)
            {
                posPieces.x = posXPieces;
                posPieces.y -= 100.0f;
            }
        }

        for (x = 0; x < enemyPiecesList.Length; x++)
        {
            enemyPiecesList[x].SetActive(false);
        }
        //PreConstruction for AI; should be best board layout
        //aiLog.randomPreConstructionAI(0, 3, enemyPiecesList); //random
        aiLog.bestPreConstructionAI(enemyPiecesList);
    }

    //check if all the pieces of the plaer is placed on the right tiles; all pieces are placed on the board
    public bool checkIfAllPlaced()
    {
        int x, y;
        for (x = 0; x < playerPiecesList.Length; x++)
        {
            if (playerPiecesList[x].GetComponent<GamePieceScript>().isPlaced == false)
            {
                return false;
            }
        }
        for (x = 0; x < occupiedPos.GetLength(1); x++)
        {
            if (occupiedPos[4,x] == true)
            {
                return false;
            }
        }
        return true;
    }

    public void turnOffInteractable()
    {
        int x;
        for(x=0;x< playerPiecesList.Length;x++)
        {
            playerPiecesList[x].GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
    public void turnOnInteractable()
    {
        int x;
        for (x = 0; x < playerPiecesList.Length; x++)
        {
            playerPiecesList[x].GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    public void GameCondition(string winner)
    {
        gms.winner = winner; //Updates the winner on the recent match
        gms.gameState = "pre-game";
        LoaderScript.loadScene(4, 3);
    }

}
