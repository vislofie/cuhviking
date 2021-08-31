using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ItemSlot : MonoBehaviour, IDropHandler
{
    
    public int SlotNumber { get; private set; }
    public bool QuickSlot { get { return _quickSlot; } }
    [SerializeField]
    private bool _quickSlot;

    private UIInventory _UIInventory;

    private void Awake()
    {
        SlotNumber = int.Parse(this.name.Remove(0, 4));
        _UIInventory = this.transform.parent.parent.GetComponent<UIInventory>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject otherDropObj = eventData.pointerDrag;
        GameObject ownDropObj = this.transform.GetChild(0).gameObject;
        if (otherDropObj != null && otherDropObj.GetComponent<ItemDragDrop>().IsEmpty == false)
        {
            
            ItemDragDrop otherDropItem = otherDropObj.GetComponent<ItemDragDrop>();
            ItemDragDrop ownDropItem = ownDropObj.GetComponent<ItemDragDrop>();

            int previousSlot = otherDropItem.CurrentSlotID;
            int newSlot = ownDropItem.CurrentSlotID;
 
            ownDropObj.transform.SetParent(_UIInventory.GetSlotTransform(otherDropItem.CurrentSlotID, otherDropItem.QuickSlot));
            ownDropItem.UpdateSlotIDAndPos();

            otherDropObj.transform.SetParent(this.transform);
            otherDropItem.UpdateSlotIDAndPos();

            _UIInventory.UpdateItemInfo(previousSlot, newSlot);
        }
    }

}
