using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField]
    private PlayerBrain _playerBrain;
    private PlayerInventory _playerInventory;
    public PlayerBrain PlayerBrainScript { get { return _playerBrain; } }

    private RectTransform[] _quickSlots;
    private RectTransform[] _slots;


    private void Awake()
    {
        _quickSlots = new RectTransform[this.transform.GetChild(0).childCount];
        _slots = new RectTransform[this.transform.GetChild(1).childCount];

        _playerInventory = _playerBrain.gameObject.GetComponent<PlayerInventory>();

        for (int i = 0; i < _slots.Length; i++)
        {
            if (i < _quickSlots.Length)
                _quickSlots[i] = this.transform.GetChild(0).GetChild(i).GetComponent<RectTransform>();
            _slots[i] = this.transform.GetChild(1).GetChild(i).GetComponent<RectTransform>();
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

    public void UpdateItemInfo(int previousSlot, bool previousQuick, int currentSlot, bool newQuick)
    {
        _playerInventory.UpdateAfterMoving(previousSlot, previousQuick, currentSlot, newQuick);
    }
}
