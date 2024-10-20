using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectionCardMenu : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private Image spriteImage;

    [SerializeField] private TextMeshProUGUI description;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layerMask.value))
            {
                spriteImage.sprite = hit.transform.GetComponent<Card>().cardSprite;
                description.text = hit.transform.GetComponent<Card>().description;
            }
        }
    }
    
    
}
