using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileScript : MonoBehaviour
{
    [HideInInspector] public int [] tilePosition = new int[2] {-1, -1 };
    public bool occupied = false;
    private Image sr;
    private Color tempColor;
    // Start is called before the first frame update
    void Awake()
    {
        sr = GetComponent<Image>();
        tempColor = sr.color;
        tempColor.a = 0.0f;
    }
    private void Start()
    {
        gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("TileSetTag").transform);
        sr.color = tempColor;
    }

    private void Update()
    {
        
    }

}
