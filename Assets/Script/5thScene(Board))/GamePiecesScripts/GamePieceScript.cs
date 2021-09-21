using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePieceScript : MonoBehaviour
{
    [HideInInspector] public int[]  piecePosition = new int[2] { -1, -1 };
    [HideInInspector] public string rankName;
    [HideInInspector]public Sprite rankImageLoad;
    [HideInInspector]public string playerType;  //human | ai1 | ai2
    [HideInInspector] public bool isPlaced = false;
    private Image rankImage;
    [HideInInspector] public int pieceIndex;  //index use for setting the string names(source from GameManager)
    [HideInInspector] public int rank;   //rank of the piece; value of the piece
    [HideInInspector] public bool isDead = false;   //deadState of the piece
    private BoardScript bs;

    [HideInInspector] public string suspectedName = "none";
    [HideInInspector] public int suspectedRankValue = 0;
    [HideInInspector] public bool suspectIsSure = false;
    [HideInInspector] public bool isVisible = false;
    // Start is called before the first frame update
    void Awake()
    {
        rankImage = GetComponent<Image>();
        bs = FindObjectOfType<BoardScript>();
    }

    // Update is called once per frame
    void Start()
    {
        rankName = FindObjectOfType<GameManagerScript>().gamePiecesNames[pieceIndex-1];
        rankImage.sprite = rankImageLoad;
        if(playerType == "human" || playerType == "ai1")
            gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("PlayerPieceSetTag").transform);
        else if (playerType == "ai2")
            gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("EnemyPieceSetTag").transform);
        if (pieceIndex == 2)
            rank = 15;
        else
            rank = pieceIndex == 1 ? 1 : pieceIndex-1;
        /*Debug.Log(rankName);
        Debug.Log("Rank: " + rank);*/
    }

}
