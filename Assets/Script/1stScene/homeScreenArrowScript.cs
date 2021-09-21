using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class homeScreenArrowScript : MonoBehaviour
{
    public GameObject lastPage;
    public GameObject[] pageList;
    public Image arrowImage;
    public Button arrowButton;

    string htmlValue = "#636363";
    Color newCol;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lastPage.active==true)
        {
            if (ColorUtility.TryParseHtmlString(htmlValue, out newCol))
            {
                arrowImage.color = newCol;
            }
            arrowButton.interactable = false;
        }
        else
        {
            arrowImage.color = Color.white;
            arrowButton.interactable = true;
        }
    }

    public void nextPage(string direction)
    {
        int index = 0;
        foreach (var item in pageList)
        {
            if(item.active==true)
            {
                index = Array.IndexOf(pageList, item);
                item.SetActive(false);
            }
        }
        if(direction=="left")
            pageList[index - 1].SetActive(true);
        else if(direction == "right")
            pageList[index + 1].SetActive(true);

    }
}
