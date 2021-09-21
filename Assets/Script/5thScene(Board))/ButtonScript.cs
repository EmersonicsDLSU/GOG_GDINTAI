using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ButtonScript : MonoBehaviour
{
    private BoardScript bs;
    private GameManagerScript gms;
    public GameObject constructText;
    public GameObject refreshButton;
    void Awake()
    {
        bs = FindObjectOfType<BoardScript>();
        gms = FindObjectOfType<GameManagerScript>();
    }

    public void checkIfAllPlaced()
    {
        int x;
        if(bs.checkIfAllPlaced())
        {
            gms.gameState = "in-game";
            if(gms.gameMode == "humanvsai")
                gms.turn = "human";
            gameObject.SetActive(false);
            constructText.SetActive(false);
            refreshButton.SetActive(false);
            for (x = 0; x < bs.enemyPiecesList.Length; x++)
            {
                bs.enemyPiecesList[x].SetActive(true);
            }
            for (x = 0; x < 21; x++)
            {
                bs.blackGraveyard[x].deadTileObject.SetActive(true);
                bs.whiteGraveyard[x].deadTileObject.SetActive(true);
            }
        }
    }

    public void preConstructionHuman()
    {
        int x, y;
        int posX, posY;
        Random rnd = new Random();

        for (x = 0; x < bs.playerPiecesList.Length; x++)
        {
            bs.playerPiecesList[x].GetComponent<GamePieceScript>().piecePosition[0] = -1;
            bs.playerPiecesList[x].GetComponent<GamePieceScript>().piecePosition[1] = -1;
            bs.playerPiecesList[x].GetComponent<GamePieceScript>().isPlaced = false;
        }

        for (x = 4; x < 8; x++)
        {
            for (y = 0; y < 9; y++)
            {
                bs.occupiedPos[x, y] = false;
                bs.tileObjectAndPos[x + "" + y].GetComponent<TileScript>().occupied = false;
            }
        }

        for (x = 0; x < bs.playerPiecesList.Length; x++)
        {
            posX = rnd.Next(5, 8);
            posY = rnd.Next(0, 9);
            if (bs.occupiedPos[posX, posY] != true)
            {
                bs.tileObjectAndPos[posX + "" + posY].GetComponent<TileScript>().occupied = true;

                bs.playerPiecesList[x].GetComponent<GamePieceScript>().piecePosition[0] = posX;
                bs.playerPiecesList[x].GetComponent<GamePieceScript>().piecePosition[1] = posY;

                bs.occupiedPos[posX, posY] = true;
                bs.playerPiecesList[x].GetComponent<GamePieceScript>().isPlaced = true;
                bs.playerPiecesList[x].GetComponent<RectTransform>().anchoredPosition 
                    = bs.tileList[posX, posY].GetComponent<RectTransform>().anchoredPosition;
                bs.objectAndPos[posX + "" + posY] = bs.playerPiecesList[x];
            }
            else
            {
                --x;
                continue;
            }
        }
    }
}
