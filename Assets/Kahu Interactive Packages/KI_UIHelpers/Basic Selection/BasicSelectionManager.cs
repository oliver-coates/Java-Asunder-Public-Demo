using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


namespace KahuInteractive.UIHelpers
{


public class BasicSelection : MonoBehaviour
{

    private static BasicSelection _Instance;

    [SerializeField] private GameObject _categoryUI;
    private List<SelectionCategoryUI> _selectionCategories;

    [Header("UI References:")]
    [SerializeField] private CanvasGroup _canvasGroup;

    private void Awake()
    {
        SelectionOptionUI.onClickedAny += Hide;

        _selectionCategories = new List<SelectionCategoryUI>();
        _Instance = this;
        Hide();
    }

    private void OnDestroy()
    {
        SelectionOptionUI.onClickedAny -= Hide;
    }

    private void Show()
    {       
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

    }

    private void Hide()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        foreach (SelectionCategoryUI categoryUI in _selectionCategories)
        {
            Destroy(categoryUI.gameObject);
        }
        _selectionCategories = new List<SelectionCategoryUI>();
    }

    /// <summary>
    /// Deletes all old UI elements from the screen
    /// </summary>
    private static void ClearElements()
    {
        foreach (SelectionCategoryUI categoryUI in _Instance._selectionCategories)
        {
            categoryUI.Remove();
        }
    }

    public static void RequestSelection<T>(Category<T>[] categories, Action<T> onChosen)
    {    
        ClearElements();

        int categoryIndex = 0;
        foreach (Category<T> category in categories)
        {
            if (category.options.Length == 0)
            {
                continue;
            }
            
            DisplayCategory(category, onChosen, categoryIndex);
            categoryIndex++;
        }

        _Instance.Show();
    }

    public static void RequestSelection<T>(Category<T> category, Action<T> onChosen)
    {
        ClearElements();
        
        DisplayCategory(category, onChosen, 0);

        _Instance.Show();
    }

    private static void DisplayCategory<T>(Category<T> category, Action<T> onChosen, int categoryIndex)
    {
        // Create Category divider
        SelectionCategoryUI categoryUI = Instantiate(_Instance._categoryUI, _Instance.transform).GetComponent<SelectionCategoryUI>();
        _Instance._selectionCategories.Add(categoryUI);

        // Position it
        Vector2 pos = new Vector2(100 + (350 * categoryIndex), 0);
        categoryUI.rectTransform.anchoredPosition = pos;

        // Draw the category options
        categoryUI.RepresentCategory(category, onChosen);
    }

    public struct Category<T>
{
    public string categoryName;
    public Option<T>[] options;

    public Category(string name, Option<T>[] options)
    {
        categoryName = name;
        this.options = options;
    }
}

    public struct Option<T>
    {
        public T value;
        public string displayText;

        public Action<T> onChosen;

        public Option(string displayText, T value)
        {
            this.displayText = displayText;
            this.value = value;
            onChosen = null;
        }

        public void Chosen()
        {
            onChosen?.Invoke(value);
        }
    }


}

}