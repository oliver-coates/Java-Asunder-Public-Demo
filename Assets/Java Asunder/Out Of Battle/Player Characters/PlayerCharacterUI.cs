using System;
using System.Collections;
using System.Collections.Generic;
using KahuInteractive.HassleFreeSaveLoad;
using KahuInteractive.UIHelpers;
using Ships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterUI : MonoBehaviour
{

    private static event Action onPlayerCharacterChanged;

    [SerializeField] private ContextualMenuLocation _contextualMenu;


    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _taskText;
    [SerializeField] private Image _portraitImage;
    [SerializeField] private Image _completionBarImage;

    private PlayerCharacter _playerCharacter;
    private ICharacterInteractable _taskCurrentlyBeingRolledFor;

    #region Unity Methods
    private void Start()
    {
        SetupContextualMenu();
        onPlayerCharacterChanged += RefreshUI;
    }

    private void OnDestroy()
    {
        onPlayerCharacterChanged -= RefreshUI;

        if (_playerCharacter != null)
        {
            _playerCharacter.OnStateChanged -= RefreshUI;
        }
    }

    private void Update()
    {
        if (_playerCharacter == null)
        {
            return;
        }

        UpdateCompletionBar();
    }
    #endregion

    public void AssignToPlayerCharacter(PlayerCharacter playerCharacter)
    {
        _playerCharacter = playerCharacter;
        _playerCharacter.OnStateChanged += RefreshUI;

        RefreshUI();
        UpdateCompletionBar();
    }

    #region Button callbacks:
    private void Rename()
    {
        BasicInputManager.RequestInput(new BasicInputManager.InputRequest("Rename Character", Rename));
    }

    private void Rename(string newName)
    {
        _playerCharacter.characterName = newName;

        onPlayerCharacterChanged?.Invoke();
    }

    private void Remove()
    {
        SessionMaster.RemovePlayerCharacter(_playerCharacter);
    }

    private void SetDisabled()
    {
        _playerCharacter.isDisabled = !_playerCharacter.isDisabled;        

        onPlayerCharacterChanged?.Invoke();
    }

    private void SetMove()
    {
        if (GameMaster.BattleUnderway == false)
        {
            Debug.Log($"Cannot set tasks when battle is not underway");
            return;
        }

        // Get all component slots in the ship:
        ComponentSlot[] slots = SessionMaster.PlayerShip.instance.componentSlots;

        // Set up the category 
        BasicSelection.Category<ComponentSlot> movementDestinations = new BasicSelection.Category<ComponentSlot>();
        movementDestinations.categoryName = "Move Locations";
        
        // Collate all ship component slots into selectable options
        List<BasicSelection.Option<ComponentSlot>> options = new List<BasicSelection.Option<ComponentSlot>>();
        for (int slotIndex = 0; slotIndex < slots.Length; slotIndex++)
        {
            ComponentSlot slot = slots[slotIndex];

            if (slot.componentType.isLocationCharacterCanMoveTo)
            {
                options.Add(new BasicSelection.Option<ComponentSlot>(slot.slotName, slot));                
            }
        }
        movementDestinations.options = options.ToArray();

        // Request the slection, SetMoveTo will be called once an option is decided.
        BasicSelection.RequestSelection<ComponentSlot>(movementDestinations, SetMoveTo);    
    }

    private void SetMoveTo(ComponentSlot destinationSlot)
    {
        _playerCharacter.StartMoveTo(destinationSlot);
    }

    private void ChooseSetTask()
    {
        if (GameMaster.BattleUnderway == false)
        {
            Debug.Log($"Cannot set tasks when battle is not underway");
            return;
        }

        ComponentEffectiveness[] possibleComponentBuffs;
        if (_playerCharacter.location.componentInstance == null)
        {
            // This component slot has no component
            possibleComponentBuffs = new ComponentEffectiveness[0];
        }
        else
        {
            possibleComponentBuffs = _playerCharacter.location.componentInstance.GetComponentEffectivenesses();
        }


        // Assemble categories from possible player tasks:
        List<BasicSelection.Category<ICharacterInteractable>> categories = new List<BasicSelection.Category<ICharacterInteractable>>();

        #region Components to buff:
        if (GetComponentsToBuffCategory(possibleComponentBuffs, out BasicSelection.Category<ICharacterInteractable> componentsToBuffCategory))
        {
            categories.Add(componentsToBuffCategory);
        }
        #endregion

        #region Damage control:
        if (GetDamageControlTasksCategory(_playerCharacter.location.shipSection, out BasicSelection.Category<ICharacterInteractable> damageControlCategory))
        {
            categories.Add(damageControlCategory);
        }
        #endregion

        if (categories.Count == 0)
        {
            // There are no possible tasks for the player to do here.
            return;
        }

        BasicSelection.RequestSelection(categories.ToArray(), RollForSetTask);
    }

    private bool GetComponentsToBuffCategory(ComponentEffectiveness[] possibleComponentBuffs, out BasicSelection.Category<ICharacterInteractable> componentsToBuffCategory)
    {
        componentsToBuffCategory = new BasicSelection.Category<ICharacterInteractable>();

        if (possibleComponentBuffs.Length == 0)
        {
            return false;
        }

        // Set up the category for component buffs:
        componentsToBuffCategory.categoryName = "Components to Buff";

        // Collate all componentEffectiveness into selectable options
        BasicSelection.Option<ICharacterInteractable>[] options = new BasicSelection.Option<ICharacterInteractable>[possibleComponentBuffs.Length];
        for (int buffIndex = 0; buffIndex < possibleComponentBuffs.Length; buffIndex++)
        {
            ComponentEffectiveness component = possibleComponentBuffs[buffIndex];
            options[buffIndex] = new BasicSelection.Option<ICharacterInteractable>(component.name, component);
        }
        componentsToBuffCategory.options = options;

        return true;
    }

    private bool GetDamageControlTasksCategory(ShipSection section, out BasicSelection.Category<ICharacterInteractable> damageControlCategory)
    {
        damageControlCategory = new BasicSelection.Category<ICharacterInteractable>();

        int numOfDamages = section.state.damages.Count; 

        if (numOfDamages == 0)
        {
            // No damage to be repaired.
            return false;
        }

        damageControlCategory.categoryName = "Damage Control";

        BasicSelection.Option<ICharacterInteractable>[] options = new BasicSelection.Option<ICharacterInteractable>[numOfDamages];

        int i = 0;
        foreach (DamageInstance damageToRepair in section.state.damages)
        {
            string damageName = $"{damageToRepair.GetTaskName()} ({damageToRepair.intensity:0}%)";

            options[i] = new BasicSelection.Option<ICharacterInteractable>(damageName, damageToRepair);

            i++;            
        }

        damageControlCategory.options = options;

        return true;
    }

    private void RollForSetTask(ICharacterInteractable taskToRollFor)
    {
        BasicInputManager.InputRequest inputRequest = new BasicInputManager.InputRequest("Roll", SetTask); 
        BasicInputManager.RequestInput(inputRequest);

        if (_taskCurrentlyBeingRolledFor != null)
        {
            Debug.LogError("Component is already being rolled for. Aborting/.");
            return;
        }
        else
        {
            _taskCurrentlyBeingRolledFor = taskToRollFor;
        }
    }

    private void SetTask(string rollAsString)
    {  
        if (int.TryParse(rollAsString, out int roll))
        {
            _taskCurrentlyBeingRolledFor.ApplyRoll(roll, _playerCharacter);
            
            string description = _taskCurrentlyBeingRolledFor.GetTaskDescription();
            _playerCharacter.StartTask(description, PlayerCharacter.TASK_TIME);
            
            _taskCurrentlyBeingRolledFor = null;
        }
        else
        {
            Debug.LogError($"Could not parse input {rollAsString} to int");
        }
    }

    private void SetIdle()
    {
        Debug.Log($"Idle");
    }


    #endregion

    #region UI

    private void SetupContextualMenu()
    {
        // Set up the contextual menu:
        ContextualMenu.Option[] options = new ContextualMenu.Option[6];

        options[0] = new ContextualMenu.Option("Remove", Remove);
        options[1] = new ContextualMenu.Option("Rename", Rename);
        
        options[2] = new ContextualMenu.Option("Disable/Enable", SetDisabled);
        options[3] = new ContextualMenu.Option("Move", SetMove);
        options[4] = new ContextualMenu.Option("Do task", ChooseSetTask);
        options[5] = new ContextualMenu.Option("Set idle", SetIdle);

        _contextualMenu.Initialise(options);
    }

    private void RefreshUI()
    {
        _nameText.text = _playerCharacter.characterName;
        // _portraitImage.sprite = _playerCharacter.image;

        if (_playerCharacter.isDisabled)
        {
            _canvasGroup.alpha = 0.25f;
            _taskText.text = "";
        }
        else
        {
            _canvasGroup.alpha = 1f;
            _taskText.text = _playerCharacter.currentTaskString;
        }
    }
    
    private void UpdateCompletionBar()
    {
        // Set fill amount - preventing divide by 0
        if (_playerCharacter.timeToCompleteTask == 0)
        {
            _completionBarImage.fillAmount = 0;
        }
        else
        {
            _completionBarImage.fillAmount = (_playerCharacter.taskTimer / _playerCharacter.timeToCompleteTask);
        }
    }


    #endregion
}

