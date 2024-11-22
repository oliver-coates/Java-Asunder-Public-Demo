using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KahuInteractive.UIHelpers
{


public class ContextualMenuButton : MonoBehaviour
{
    public static event Action onClicked;

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _text;

    private ContextualMenu.Option _option;

    public void Setup(ContextualMenu.Option option, Vector2 position)
    {
        _rectTransform.anchoredPosition = position;
        _text.text = option.text;
          
        _option = option;
    }

    public void Clicked()
    {
        _option.OnClick();

        onClicked?.Invoke();
    }

}

}