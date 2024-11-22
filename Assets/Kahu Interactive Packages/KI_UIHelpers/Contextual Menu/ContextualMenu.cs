using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KahuInteractive.UIHelpers
{

public class ContextualMenu : MonoBehaviour
{
    [SerializeField] private GameObject _buttonPrefab;

    [SerializeField] private GameObject[] _buttonObjects;

    [Header("UI references:")]
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;



    private void Awake()
    {
        ContextualMenuLocation.onMenuOpened += OpenMenu;
        ContextualMenuButton.onClicked += CloseMenu;

        _buttonObjects = new GameObject[0];
        CloseMenu();
    }

    private void OnDestroy()
    {
        ContextualMenuLocation.onMenuOpened -= OpenMenu;
        ContextualMenuButton.onClicked -= CloseMenu;
    }

    public void OpenMenu(Option[] options)
    {
        ClearButtonObjects();
        _buttonObjects = new GameObject[options.Length];

        // Show:
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;

        int uiDirection = 0;
        // Changed anchors depending on which side of the screen we are at
        if (Input.mousePosition.y < (Screen.height/2f))
        {
            _rectTransform.anchorMin = new Vector2(0, 0);
            _rectTransform.anchorMax = new Vector2(0, 0);
            _rectTransform.pivot = new Vector2(0, 0);
            uiDirection = 1;
        }
        else
        {
            _rectTransform.anchorMin = new Vector2(0, 1);
            _rectTransform.anchorMax = new Vector2(0, 1);
            _rectTransform.pivot = new Vector2(0, 1);
            uiDirection = -1;
        }

        // Move to mouse position:
        transform.position = Input.mousePosition;

        int buttonIndex = 0;
        foreach (Option option in options)
        {
            GameObject newButton = Instantiate(_buttonPrefab, transform);
            _buttonObjects[buttonIndex] = newButton;
            
            Vector2 pos = new Vector2(0, uiDirection * buttonIndex * 34f);
            newButton.GetComponent<ContextualMenuButton>().Setup(option, pos);
            
            buttonIndex++;
        }
    }


    public void CloseMenu()
    {
        ClearButtonObjects();

        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0f;
    }

    private void ClearButtonObjects()
    {
        if (_buttonObjects.Length != 0)
        {
            foreach (GameObject buttonObject in _buttonObjects)
            {
                Destroy(buttonObject);
            }
        }
    }

    public struct Option
    {
        public string text;
        public event Action onClick;

        public Option(string text, Action action)
        {
            this.text = text;
            onClick = null;

            onClick += action;
        }

        public void OnClick()
        {
            onClick.Invoke();
        }

    }
}

}
