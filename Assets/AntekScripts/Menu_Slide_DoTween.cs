using DG.Tweening;
using UnityEngine;

public class Menu_Slide_DoTween : MonoBehaviour
{
    [SerializeField] private GameObject PointToMove;

    public Ease ease;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SlideObject()
    {
        this.transform.DOMove(PointToMove.transform.position, 8f).SetEase(ease);
    }
    
    
}
