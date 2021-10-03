using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemAction { USE, EQUIP, ABOUT, DROP };
public class ItemContextMenu : MonoBehaviour
{
    [SerializeField]
    private UIInventory _inventoryManager;
    [SerializeField]
    private PlayerBrain _playerBrain;

    public PlayerBrain PlayerBrainScript { get { return _playerBrain; } }
    public int ActiveSlotID { get { return _activeSlotID; } }
    public bool ActiveSlotQuick { get { return _activeSlotQuick; } }
    public bool Activated { get { return _activated; } }


    private int _activeSlotID;
    private bool _activeSlotQuick;
    private bool _activated;

    private Item _currentItem;
    private ItemAction[] _currentSetOfActions = new ItemAction[3];

    private void Awake()
    {
        _playerBrain = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBrain>();
    }

    private void Start()
    {
        _activeSlotID = -1;
        _activeSlotQuick = false;
        _activated = false;
    }

    /// <summary>
    /// Activates context menu at the current slot
    /// </summary>
    /// <param name="slotID">ID of the slot where context menu gonna be activated</param>
    /// <param name="quickSlot">tells whether current slot is a quick slot or not</param>
    public void Activate(int slotID, bool quickSlot)
    {
        _currentItem = _playerBrain.GetItemFromSlotID(slotID, quickSlot);

        if (_currentItem.itemType == ItemType.WEAPON || _currentItem.itemType == ItemType.HEAD_ARMOR || _currentItem.itemType == ItemType.CHEST_ARMOR ||
            _currentItem.itemType == ItemType.ARM_ARMOR || _currentItem.itemType == ItemType.LEG_ARMOR || _currentItem.itemType == ItemType.BOOTS)
        {
            _currentSetOfActions[0] = ItemAction.EQUIP;
        }
        else
        {
            _currentSetOfActions[0] = ItemAction.USE;
        }
        _currentSetOfActions[1] = ItemAction.ABOUT;
        _currentSetOfActions[2] = ItemAction.DROP;


        EnableVisuals();
        _activeSlotID = slotID;
        _activeSlotQuick = quickSlot;
        GetComponent<RectTransform>().position = _inventoryManager.GetSlotTransform(slotID, quickSlot).position;
        _activated = true;
    }

    /// <summary>
    /// Deactivates context menu
    /// </summary>
    public void Deactivate()
    {
        DisableVisuals();
        _activated = false;
    }

    /// <summary>
    /// Makes an action according to what action player chose on the context menu
    /// </summary>
    /// <param name="action">type of the action</param>
    public void MakeAnAction(ItemAction action)
    {
        if (_activated)
        {
            switch (action)
            {
                case ItemAction.USE:
                    break;
                case ItemAction.EQUIP:
                    break;
                case ItemAction.ABOUT:
                    break;
                case ItemAction.DROP:
                    _inventoryManager.DropItem(_activeSlotID, _activeSlotQuick);
                    Deactivate();
                    break;
                default:
                    break;
            }
        }
        
    }

    /// <summary>
    /// Forbids player to attack and interact
    /// </summary>
    public void ForbidPlayerInteraction()
    {
        _inventoryManager.PlayerBrainScript.ForbidAttackAndInteractionMovement();
    }

    /// <summary>
    /// Allows player to attack and interact
    /// </summary>
    public void AllowPlayerInteraction()
    {
        _inventoryManager.PlayerBrainScript.AllowAttackAndInteractionMovement();
    }

    /// <summary>
    /// Disables visual representation of the context menu
    /// </summary>
    private void DisableVisuals()
    {
        this.GetComponent<Image>().enabled = false;
        foreach (Text children in transform.GetComponentsInChildren<Text>())
        {
            children.enabled = false;
        }
    }

    /// <summary>
    /// Enables visual representation of the context menu
    /// </summary>
    private void EnableVisuals()
    {
        this.GetComponent<Image>().enabled = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Text textComponent = child.GetComponent<Text>();
            textComponent.enabled = true;
            switch(_currentSetOfActions[i])
            {
                case ItemAction.EQUIP:
                    textComponent.text = "Equip";
                    break;
                case ItemAction.USE:
                    textComponent.text = "Use";
                    break;
                case ItemAction.ABOUT:
                    textComponent.text = "About";
                    break;
                case ItemAction.DROP:
                    textComponent.text = "Drop";
                    break;
                default:
                    textComponent.text = "Bruh";
                    break;
            }
            child.GetComponent<ItemActionEvent>().SetAction(_currentSetOfActions[i]);
        }
    }
}
