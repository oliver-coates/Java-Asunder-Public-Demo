using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


namespace KahuInteractive.UIHelpers
{


public class BasicInputManager : MonoBehaviour
{

    private static BasicInputManager _Instance;

    [Header("UI References:")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _displayText;
    [SerializeField] private TMP_InputField _inputField;
    
    private static InputRequest _inputRequest;

    private void Awake()
    {
        _Instance = this;
        Hide();
    }

    private void Show()
    {
        _inputField.text = "";
       
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        // Force selection of this input field:
        EventSystem.current.SetSelectedGameObject(_inputField.gameObject, null);
        _inputField.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    private void Hide()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public static void RequestInput(InputRequest inRequest)
    {    
        _inputRequest = inRequest;

        _Instance._displayText.text = inRequest.displayText;
        _Instance.Show();

    }

    public void SubmitInput()
    {
        _inputRequest.onFulfilled.Invoke(_inputField.text);
        Hide();
    }

    public struct InputRequest
    {
        public Action<string> onFulfilled;   
        public string displayText;

        public InputRequest(string displayText, Action<string> onFulfilled)
        {
            this.displayText = displayText;
            this.onFulfilled = onFulfilled;
        }
    }


}

}