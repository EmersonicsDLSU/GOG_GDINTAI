using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButton : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;
    private GameManagerScript gms;
    private void Awake()
    {
        gms = FindObjectOfType<GameManagerScript>();
    }

    public void YesButton()
    {
        Application.Quit();
    }
    //
    public void NoButton()
    {
        PauseMenu.SetActive(false);
    }
    public void SurrenderButton()
    {
        gms.winner = "ai2";
        LoaderScript.loadScene(4, 3);
        
    }

    public void On_clickPauseButton()
    {
        PauseMenu.SetActive(true); 
    }
}
