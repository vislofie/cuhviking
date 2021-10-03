using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    private const int QUICKSLOTS_ID_IN_HIERARCHY = 1;
    private const int SLOTS_ID_IN_HIERARCHY = 2;

    [SerializeField]
    private PlayerBrain _playerBrain;
    private PlayerInventory _playerInventory;
    public PlayerBrain PlayerBrainScript { get { return _playerBrain; } }

    private RectTransform[] _quickSlots;
    private RectTransform[] _slots;


    private void Awake()
    {
        _quickSlots = new RectTransform[this.transform.GetChild(QUICKSLOTS_ID_IN_HIERARCHY).childCount];
        _slots = new RectTransform[this.transform.GetChild(SLOTS_ID_IN_HIERARCHY).childCount];

        _playerInventory = _playerBrain.gameObject.GetComponent<PlayerInventory>();

        for (int i = 0; i < _slots.Length; i++)
        {
            if (i < _quickSlots.Length)
                _quickSlots[i] = this.transform.GetChild(QUICKSLOTS_ID_IN_HIERARCHY).GetChild(i).GetComponent<RectTransform>();
            _slots[i] = this.transform.GetChild(SLOTS_ID_IN_HIERARCHY).GetChild(i).GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// Returns inventory slot transform by id and whether its a quick slot or nah
    /// </summary>
    /// <param name="slotID">id of the needed slot</param>
    /// <param name="quickSlot">is it quickSlot</param>
    public Transform GetSlotTransform(int slotID, bool quickSlot)
    {
        return slotID < _slots.Length ? (quickSlot ? _quickSlots[slotID] : _slots[slotID]) : null;
    }

    /// <summary>
    /// Updates 
    /// </summary>
    /// <param name="previousSlot"></param>
    /// <param name="previousQuick"></param>
    /// <param name="currentSlot"></param>
    /// <param name="newQuick"></param>
    public void UpdateItemInfo(int previousSlot, bool previousQuick, int currentSlot, bool newQuick)
    {
        _playerInventory.UpdateAfterMoving(previousSlot, previousQuick, currentSlot, newQuick);
    }

    /// <summary>
    /// Drops an item from the inventory, either at the player position(fromMousePos false) or at the mouse position(fromMousePos true)
    /// </summary>
    /// <param name="slotID">id of the slot the item was dropped from</param>
    /// <param name="amount">amount of the items dropped</param>
    /// <param name="fromMousePos"></param>
    public void DropItem(int slotID, bool quickSlot, int amount = -1)
    {
        _playerBrain.DropItem(slotID, quickSlot, amount);
        
    }
}
