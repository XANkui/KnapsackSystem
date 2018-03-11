﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TooltipUI : MonoBehaviour
{
    public Text OutlineText;
    public Text ContentText;

    public void UpdateTooltip(string text)
    {
        OutlineText.text = text;
        ContentText.text = text;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetLocalPosition(Vector2 position)
    {
        transform.localPosition = position;
    }
}
