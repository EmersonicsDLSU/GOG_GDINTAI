using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallBackScript : MonoBehaviour
{
    private bool isFirstUpdate = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.isFirstUpdate)
        {
            this.isFirstUpdate = false;
            LoaderScript.LoaderCallBack();
        }
    }
}
