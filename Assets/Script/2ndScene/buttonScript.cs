using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void nextScene()
    {
        LoaderScript.loadScene(2,3);
    }

    public void previousScene()
    {
        LoaderScript.loadScene(0,3);
    }

    public void setGameMode(string gameMode)
    {
        FindObjectOfType<GameManagerScript>().gameMode = gameMode;
    }
}
