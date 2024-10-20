using DG.Tweening;
using TMPro;
using UnityEngine;

public class Text_Fade_DoTween : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FadeOut();
    }

    // Update is called once per frame
    void Update()
    {
    }


    void FadeOut()
    {
        GetComponent<TextMeshProUGUI>().DOFade(0, 5f);
    }
}
