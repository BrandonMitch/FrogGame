using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Heart_Spot : MonoBehaviour
{
    [SerializeField] Image healthFill;
    [SerializeField] Image heartSprite;
    [SerializeField] RectTransform rectTransform;

    public void SetFillColor(Color color)
    {
        healthFill.color = color;
    }
    public void SetFillAmount(float a)
    {
        healthFill.fillAmount = a;
    }
    public void SetSprite(Sprite heartSprite)
    {
        this.heartSprite.sprite = heartSprite;
    }
    public void SetHeartHeight(float heartHeight)
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heartHeight);
    }

    public void SetAll(Color fillColor, float fillAmount, Sprite heartSprite, float heartHeight)
    {
        SetFillColor(fillColor);
        SetFillAmount(fillAmount);
        SetSprite(heartSprite);
        SetHeartHeight(heartHeight);
    }
}
