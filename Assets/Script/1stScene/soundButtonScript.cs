using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //
    public void OnSound()
    {
        FindObjectOfType<AudioManagerScript>().sounds[0].source.volume = 1;
    }
    public void OffSound()
    {
        FindObjectOfType<AudioManagerScript>().sounds[0].source.volume = 0;
    }
}
