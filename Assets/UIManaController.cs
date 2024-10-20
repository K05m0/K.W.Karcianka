using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManaController : MonoBehaviour
{ 
    private TextMeshProUGUI manaText;
    [SerializeField] private List<GameObject> coinList = new List<GameObject>();

    private int maxCoins;
    private int currentCoins;
    
    void Start()
    {
        ManaInfromationEvent.current.onManaChange += onManaChange;
        manaText = GetComponent<TextMeshProUGUI>();
    }
   

    private void onManaChange(int mana,int maxMana)
    {
        if (maxCoins != maxMana)
        {
            currentCoins = maxMana;
            maxCoins = maxMana;
            for (int i = 0; i < maxMana; i++)
            {
                coinList[i].gameObject.SetActive(true);
            } 
        }
        
        if (mana < 0)
        {
            manaText.text = currentCoins+mana + "/" + maxMana;
            currentCoins = currentCoins + mana;
        }
        else if(mana == maxMana)
        {
            manaText.text = maxMana + "/" + maxMana;
        }

        if (mana < 0)
        {
            mana = mana * -1;
            for (int i = 0; i < mana; i++)
            {
                if (coinList[i].gameObject.activeSelf == false)
                {
                    int j = 0;
                    while (coinList[i].gameObject.activeSelf == false)
                    {
                        j += i + 1;
                        if (coinList[j].gameObject.activeSelf == true)
                        {
                            coinList[j].gameObject.SetActive(false);
                            break;
                        }
                    }
                }
                else
                {
                    coinList[i].gameObject.SetActive(false);
                }
            }
        }
       
    }
    
}
