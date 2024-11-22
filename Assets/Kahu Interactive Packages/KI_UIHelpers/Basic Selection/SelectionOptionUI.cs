using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KahuInteractive.UIHelpers
{

public class SelectionOptionUI : MonoBehaviour
{
    public static event Action onClickedAny; 

    public RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI _text;

    private event Action onClick;

    public void Initialise<T>(BasicSelection.Option<T> option, Action<T> onChosen)
    {
        _text.text = option.displayText;

        option.onChosen = onChosen;

        onClick += option.Chosen;
    }

    public void Clicked()
    {
        onClick?.Invoke();
        onClickedAny?.Invoke();
    }

}


}