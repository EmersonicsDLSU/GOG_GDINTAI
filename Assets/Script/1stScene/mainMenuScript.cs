using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuScript : MonoBehaviour
{
    public void playButtonSelected() //moves to the next scene -> Play Mode Scene
    {
        LoaderScript.loadScene(1,3);
    }
    public void quitButtonSelected() //closes the application
    {
        Application.Quit();
    }
}
