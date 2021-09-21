using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemAction { USE, ABOUT, DROP };
public class ItemContextMenu : MonoBehaviour
{
    [SerializeField]
    private UIInventory _inventoryManager;
    [SerializeField]
    private PlayerBrain _playerBrain;

    public int ActiveSlotID { get { return _activeSlotID; } }
    public bool ActiveSlotQuick { get { return _activeSlotQuick; } }
    public bool Activated { get { return _activated; } }

    private int _activeSlotID;
    private bool _activeSlotQuick;
    private bool _activated;

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
        foreach (Text children in transform.GetComponentsInChildren<Text>())
        {
            children.enabled = true;
        }
    }
}
