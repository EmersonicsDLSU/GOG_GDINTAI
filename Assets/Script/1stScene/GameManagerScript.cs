using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [HideInInspector]public string gameMode; //humanvsai or aivsai
    [HideInInspector]public float time;
    [HideInInspector]public string turn = "player";    //player, ai1, ai2
    [HideInInspector]public string gameState = "pre-game";  //pre-game, in-game, post-game
    [HideInInspector]public string winner = "none";  //human, ai1, ai2
    [HideInInspector]public int []scores = new int[2];  //human/ai1 - ai2 
    private bool Lose = false;
    private static GameManagerScript instance;
    public Sprite[] gamePiecesImagesB = new Sprite[22];
    public Sprite[] gamePiecesImagesW = new Sprite[22];
    public string[] gamePiecesNames;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(instance);
    }
    // Start is called before the first frame update
    void Start()
    {
        gamePiecesNames = new string[15]{ "Flag", "Spy", "Private", "Sergeant", "2nd Lieutenant", "1st Lieutenant", 
        "Captain", "Major", "Lieutenant Colonel", "Colonel", "One-Star General", 
        "Two-Star General", "Three-Star General", "Four-Star General", "Five-Star General"};
    }

    // Update is called once per frame
    void Update()
    {
        configureTime();
        //Debug.Log(scores[0] + ":" + scores[1]);
        //Debug.Log(gameState);
    }

    public void chosenGameMode(string choice)
    {
        this.gameMode = choice;
    }

    public void configureTime(bool stop = false)
    {
        if(stop==false)
        {
            time += Time.deltaTime;
        }
    }

    public void setTurn(string side)
    {
        turn = side;
        if(turn == "human")
        {
            GameObject.FindGameObjectWithTag("EnemyPieceSetTag").transform.SetSiblingIndex(6);
        }
        else if (turn == "ai1")
        {
            GameObject.FindGameObjectWithTag("PlayerPieceSetTag").transform.SetSiblingIndex(6);
        }
        else if (turn == "ai2")
        {
            GameObject.FindGameObjectWithTag("PlayerPieceSetTag").transform.SetSiblingIndex(6);
        }
    }
}
