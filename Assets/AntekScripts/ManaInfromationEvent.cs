using System;
using UnityEngine;

public class ManaInfromationEvent : MonoBehaviour
{
    public static ManaInfromationEvent current;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        current = this;
    }

    public event Action<int,int> onManaChange;

    public void ManaChange(int mana,int maxMana)
    {
        if (onManaChange != null)
        {
            onManaChange(mana,maxMana);
        }
    }
}
