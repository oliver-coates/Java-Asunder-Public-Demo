using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using KahuInteractive.UIHelpers;

namespace KahuInteractive.UIHelpers
{

public class ContextualMenuLocation : MonoBehaviour, IPointerClickHandler
{
    public static event Action<ContextualMenu.Option[]> onMenuOpened;

    private bool _initialised = false;
    private ContextualMenu.Option[] _options;


    public void Initialise(ContextualMenu.Option[] options)
    {
        _initialised = true;
        _options = options;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_initialised == false)
        {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            onMenuOpened.Invoke(_options);
        }
    }
    

}



}