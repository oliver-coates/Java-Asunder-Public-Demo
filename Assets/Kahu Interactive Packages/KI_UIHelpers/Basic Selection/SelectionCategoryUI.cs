using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KahuInteractive.UIHelpers
{

public class SelectionCategoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _optionButtonPrefab;
    private List<SelectionOptionUI> _optionButtons;

    public RectTransform rectTransform;
    [SerializeField] private RectTransform _contentZone;
    [SerializeField] private TextMeshProUGUI _header;

    private void Awake()
    {
        _optionButtons = new List<SelectionOptionUI>();
    }

    public void RepresentCategory<T>(BasicSelection.Category<T> category, Action<T> onChosen)
    {
        _header.text = category.categoryName;
    
        int optionIndex = 0;
        foreach (BasicSelection.Option<T> option in category.options)
        {
            SelectionOptionUI optionUI = Instantiate(_optionButtonPrefab, _contentZone.transform).GetComponent<SelectionOptionUI>();

            optionUI.Initialise(option, onChosen);
    
            Vector2 pos = new Vector2(0, -10 - (optionIndex * 50));
            optionUI.rectTransform.anchoredPosition = pos;

            _optionButtons.Add(optionUI);
            optionIndex++;
        }
    }

    public void Remove()
    {
        foreach (SelectionOptionUI selectionOption in _optionButtons)
        {
            Destroy(selectionOption.gameObject);
        }

        Destroy(gameObject);
    }

}


}