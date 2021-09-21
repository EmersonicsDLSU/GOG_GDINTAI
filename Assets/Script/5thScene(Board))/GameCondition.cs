using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCondition : MonoBehaviour
{
    private GameManagerScript gms;
    private AudioManagerScript ams;
    [SerializeField] private GameObject winButton;
    [SerializeField] private GameObject loseButton;
    [SerializeField] private GameObject whitewinButton;

    private void Awake()
    {
        gms = FindObjectOfType<GameManagerScript>();
        ams = FindObjectOfType<AudioManagerScript>();
        checkWinCondition();
    }

    private void Start()
    {
        if (gms.winner == "human")
            ams.playSound("WinSoundEffect");
        else if (gms.winner == "ai2")
            ams.playSound("LoseSoundEffect");
    }
    public void winbutton()
    {
        ams.stopSound("WinSoundEffect");
        LoaderScript.loadScene(0, 3);
    }
    public void whitewinbutton()
    {
        LoaderScript.loadScene(0, 3);
    }


    public void losebutton()
    {
        ams.stopSound("LoseSoundEffect");
        SceneManager.LoadScene(0);
    }

    public void checkWinCondition()
    {
        if (gms.winner == "ai2")
        {
            loseButton.SetActive(true);
            gms.scores[1]++;
        }
        else if (gms.winner == "human")
        {
            winButton.SetActive(true);
            gms.scores[0]++;
        }
        else if(gms.winner == "ai1")
        {
            whitewinButton.SetActive(true);
        }
        
    }
}
 