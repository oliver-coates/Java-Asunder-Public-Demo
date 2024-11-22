using System;
using System.Collections;
using System.Collections.Generic;
using KahuInteractive.HassleFreeAudio;
using Ships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




public class DisasterUI : MonoBehaviour
{
    public struct Event
    {
        [SerializeField] private string _header;
        public string header
        {
            get
            {
                return _header;
            }	
        }

        [SerializeField] private Sprite _sprite;
        public Sprite sprite
        {
            get
            {
                return _sprite;
            }	
        }

        [SerializeField] private string _description;
        public string description
        {
            get
            {
                return _description;
            }	
        }
    
        [SerializeField] private ClipSet _clipSet;
        public ClipSet clipSet
        {
            get
            {
                return _clipSet;
            }	
        }

        public Event(string header, Sprite sprite, string description)
        {
            _header = header;
            _sprite = sprite;
            _description = description;
            _clipSet = null;
        }
    
        public Event(string header, Sprite sprite, string description, ClipSet clipSet)
        {
            _header = header;
            _sprite = sprite;
            _description = description;
            _clipSet = clipSet;
        }
    }    
    
    public static event Action OnStartShowing;
    public static event Action OnFinishedShowing; 

    private static DisasterUI _Instance;

    private Queue<Event> _toDisplayQueue;

    [Header("UI References:")]
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private TextMeshProUGUI _header;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _description;

    [Header("State:")]
    [SerializeField] private bool _showEventsNextFrame;
    [SerializeField] private bool _isShowingEvent;
    [SerializeField] private float _fadeTimer;
    [SerializeField] private bool _fadingIn;

    [Header("Ship Sinking Event")]
    [SerializeField] private Sprite _shipSinkingSprite;
    [SerializeField] private ClipSet _shipSinkingAudio;

    #region Initialisation & Destruction
    private void Awake()
    {
        _Instance = this;
        _toDisplayQueue = new Queue<Event>();

        _isShowingEvent = false;
        _fadeTimer = 0f;
        _fadingIn = false;
        _showEventsNextFrame = false;

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    
        ShipInstance.OnShipStartedSinking += NotifyPlayersOfSinkingShip;
    }
    
    private void OnDestroy()
    {
        ShipInstance.OnShipStartedSinking -= NotifyPlayersOfSinkingShip;
    }
    #endregion

    #region Event queue
    public static void QueueDisasterEvent(Event disasterEvent)
    {
        _Instance._toDisplayQueue.Enqueue(disasterEvent);

        if (_Instance._isShowingEvent == false)
        {
            // By tripping this boolean,
            // the following frame will show the disaster
            _Instance._showEventsNextFrame = true;            
        }
    }
    
    private void NextDisasterEvent()
    {
        // Peek to see if there is an event in queue
        if (_toDisplayQueue.TryPeek(out Event nextEventToShow))
        {
            if (_isShowingEvent == false)
            {
                // We are showing the first event
                OnStartShowing?.Invoke();
            }
     
            ShowDisasterEvent(nextEventToShow);
        }
        else
        {
            if (_isShowingEvent == true)
            {
                // Done showing events
                OnFinishedShowing?.Invoke();

                _fadingIn = false;
                _isShowingEvent = false;
                _showEventsNextFrame = false;
            }
            
        }
    }

    private void ShowDisasterEvent(Event disasterEvent)
    {
        // Update the UI        
        _header.text = disasterEvent.header;
        _image.sprite = disasterEvent.sprite;
        _description.text = disasterEvent.description;

        // Start fading in, if not already
        _fadingIn = true;
        _isShowingEvent = true;
        _showEventsNextFrame = false;

        // Pop off the current event at the top of the queue (it is the one we are showing)
        _toDisplayQueue.Dequeue();

        if (disasterEvent.clipSet != null)
        {
            AudioEngine.PlaySound(disasterEvent.clipSet);
        }
    }
    #endregion

    private void Update()
    {
        if (_fadingIn)
        {
            _fadeTimer += Time.deltaTime * 1.5f;
        }
        else
        {
            _fadeTimer -= Time.deltaTime * 2.5f; // Magic numbers woo!!!
        }
    
        _fadeTimer = Mathf.Clamp(_fadeTimer, 0, 1);

        _canvasGroup.alpha = _fadeTimer;

        if (_isShowingEvent && Input.GetKeyDown(KeyCode.E))
        {
            NextDisasterEvent();
        }

        if (_showEventsNextFrame)
        {
            _Instance.NextDisasterEvent();
        }
    }

    private void NotifyPlayersOfSinkingShip(ShipInstance shipInstance)
    {   
        // NOTE that all sinking ships are shown to the players,
        // It isn't just the player ship sinking that triggers this notification.
        
        string shipFullName = shipInstance.shipData.GetFullName();
        string shipShortName = shipInstance.shipData.shipName;

        string header = $"{shipShortName} is sinking!"; 
        string description = $"The {shipFullName} has recieved too much damage and has started to sink.";

        // Notify the players that a sinking has started
        Event sinkingEvent = new Event(header, _shipSinkingSprite, description);
        QueueDisasterEvent(sinkingEvent);
    
        AudioEngine.PlaySound(_shipSinkingAudio);
    }   
}
