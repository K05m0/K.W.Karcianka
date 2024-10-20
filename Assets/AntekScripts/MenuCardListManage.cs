using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuCardListManage : MonoBehaviour
{
    public List<Sprite> SpriteList = new List<Sprite>();
    public List<string> DescriptionList = new List<string>();
    private int listNumber = 0;

    private Image image;
    [SerializeField] private TextMeshProUGUI descriptionText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       image = GetComponent<Image>();
       image.sprite = SpriteList[listNumber];
       descriptionText.text = DescriptionList[listNumber];
    }

    // Update is called once per frame


    public void AddOnListNumber()
    {
        listNumber += 1;
        ChangeCard();
    }

    public void SubtruckListNumber()
    {
        listNumber -= 1;
        ChangeCard();
    }

    public void ChangeCard()
    {
        if (listNumber > SpriteList.Count -1)
        {
            listNumber = 0;
        }
        else if (listNumber < 0)
        {
            listNumber = SpriteList.Count -1;
        }

        image.sprite = SpriteList[listNumber];
        descriptionText.text = DescriptionList[listNumber];
    }
}
