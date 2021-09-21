using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; //this is needed for the event functions
using Math = System.Math;

public class TileSlot : MonoBehaviour, IDropHandler
{
    [HideInInspector] public GameObject piece;
    private BoardScript bs;
    private TileScript ts;
    private GameManagerScript gms;
    private AILogic aiScript;

    void Awake()
    {
        bs = FindObjectOfType<BoardScript>();
        ts = GetComponent<TileScript>();
        gms = FindObjectOfType<GameManagerScript>();
        aiScript = FindObjectOfType<AILogic>();
    }

    public void OnDrop(PointerEventData eventData) //this is for when an object is placed/drop on the slot
    {
        Debug.Log("OnDrop");
        if (eventData.pointerDrag != null)//pointerDrag is the object that is being drag; checks if the object is passing through the slot
        {
            piece = eventData.pointerDrag.gameObject;
            int distanceX = Math.Abs(piece.GetComponent<GamePieceScript>().piecePosition[0] - gameObject.GetComponent<TileScript>().tilePosition[0]);
            int distanceY = Math.Abs(piece.GetComponent<GamePieceScript>().piecePosition[1] - gameObject.GetComponent<TileScript>().tilePosition[1]);
            Debug.Log("Hello: " + distanceX + ":" + distanceY);
            if ((gms.gameState == "pre-game") ||
                (distanceX <= 1 && distanceY <= 1) && !(distanceX == 1 && distanceY == 1) &&
                (piece.GetComponent<GamePieceScript>().piecePosition[0] != gameObject.GetComponent<TileScript>().tilePosition[0] ||
                piece.GetComponent<GamePieceScript>().piecePosition[1] != gameObject.GetComponent<TileScript>().tilePosition[1]))
            {
                //Debug.Log("OnDrop 1stCondition");
                bs.objectAndPos[piece.GetComponent<GamePieceScript>().piecePosition[0] + ""
                    + piece.GetComponent<GamePieceScript>().piecePosition[1]] = null;
                bs.objectAndPos[GetComponent<TileScript>().tilePosition[0] + ""
                    + GetComponent<TileScript>().tilePosition[1]] = piece.gameObject;

                piece.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition; //anchoredPosition is the pivot/origin of the object
                Debug.Log(ts.tilePosition[0]);
                bs.occupiedPos[ts.tilePosition[0], ts.tilePosition[1]] = true;
                GetComponent<TileScript>().occupied = true;
                piece.GetComponent<GamePieceScript>().isPlaced = true;
                piece.GetComponent<GamePieceScript>().piecePosition[0] = gameObject.GetComponent<TileScript>().tilePosition[0];
                piece.GetComponent<GamePieceScript>().piecePosition[1] = gameObject.GetComponent<TileScript>().tilePosition[1];
                //Debug.Log("turns to tilePosition: " + piece.GetComponent<GamePieceScript>().piecePosition[0] + ":" + piece.GetComponent<GamePieceScript>().piecePosition[1]);
                piece.GetComponent<DragDrop>().previousIndexPosition[0] = gameObject.GetComponent<TileScript>().tilePosition[0];
                piece.GetComponent<DragDrop>().previousIndexPosition[1] = gameObject.GetComponent<TileScript>().tilePosition[1];

                if (piece.GetComponent<GamePieceScript>().rankName == "Flag" && piece.GetComponent<GamePieceScript>().playerType == "human" &&
                    piece.GetComponent<GamePieceScript>().piecePosition[0] == 0)
                {
                    bs.GameCondition(piece.GetComponent<GamePieceScript>().playerType); 
                }


                if (gms.gameState == "in-game")
                {
                    Debug.Log("OnDrop AiMove Trigger Tile");
                    gms.setTurn("ai1");
                    bs.turnOffInteractable();
                    //ai Turn
                    aiScript.AiMove(bs.enemyPiecesList, bs.playerPiecesList);
                }
            }
            else
            {
                Debug.Log("OnDrop 2ndCondition");
                //Debug.Log("Piece Position: " + piece.GetComponent<GamePieceScript>().piecePosition[0] + ":" + piece.GetComponent<GamePieceScript>().piecePosition[1]);
                //Debug.Log("Tile Position: " + gameObject.GetComponent<TileScript>().tilePosition[0] + ":" + gameObject.GetComponent<TileScript>().tilePosition[1]);
                piece.GetComponent<RectTransform>().position = piece.GetComponent<DragDrop>().previousPosition;
            }
        }

    }

}

