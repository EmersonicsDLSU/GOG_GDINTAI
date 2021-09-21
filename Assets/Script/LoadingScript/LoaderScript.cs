using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class LoaderScript
{
    private static Action onLoaderCallBack;

    /*scene - scene index to be loaded
      loadingScene - scene(loading type scene) index to be loaded */
    public static void loadScene(int scene, int loadingScene)
    {
        //set the loaderCallBack Action to call the loaded scene
        onLoaderCallBack = () => { SceneManager.LoadScene(scene); };

        //sets the loading screen while the loaded scene is not yet present
        SceneManager.LoadScene(loadingScene);
    }

    public static void LoaderCallBack()
    {
        if(onLoaderCallBack != null)
        {
            onLoaderCallBack(); //shoots the loaded scene
            onLoaderCallBack = null;  //resets the Action function to null
        }
    }
}
