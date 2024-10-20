using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GridCell : MonoBehaviour
{
    public Vector2Int coordinates;  // Koordynaty (x, y) komórki
    public bool isOccupied = false; // Czy komórka jest zajęta czy nie
    public bool isTargeted = false; //Czy jest nad nia myszka
    public Card CardInCell = null;


    private SpriteRenderer spriteRenderer;

    // Metoda opcjonalna do ustawienia koordynatów podczas tworzenia gridu

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetCoordinates(int x, int y)
    {
        coordinates = new Vector2Int(x, y);
    }

    public void Update()
    {
        
        if (isTargeted == true && isOccupied == false)
        {
            if (GetComponent<ParticleSystem>().isPlaying != true)
            {
                GetComponent<ParticleSystem>().Play();
            }
        }
    }
}
