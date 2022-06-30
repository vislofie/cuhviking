using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryContextMenu : MonoBehaviour
{
    private PlayerInventory _playerInventory;
    public InventorySlot ActiveSlot { get { return _activeSlot; } }
    private InventorySlot _activeSlot;
    private bool _activated;

    private Action<int> _eatDelegate;
    private Action<int> _liquidDelegate;

    private void Awake()
    {
        _playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        Deactivate();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _activated && !RectTransformUtility.RectangleContainsScreenPoint(transform.GetComponent<RectTransform>(), Input.mousePosition, null)) // null coz for some reason it doesnt work with Camera.main on it. no one knows why
        {
            Deactivate();
        }
    }

    public void Activate(int slotID, bool quickSlot)
    {
        _activated = true;

        Transform desiredSlotHolder = quickSlot ? _playerInventory.InventoryQuickSlotsHolder : _playerInventory.InventorySlotsHolder;
        RectTransform desiredSlot = null;
        for (int i = 0; i < desiredSlotHolder.childCount; i++)
        {
            if (desiredSlotHolder.GetChild(i).GetComponent<InventorySlot>().ID == slotID)
            {
                _activeSlot = desiredSlotHolder.GetChild(i).GetComponent<InventorySlot>();
                desiredSlot = _activeSlot.GetComponent<RectTransform>();
            }
        }

        if (desiredSlot == null)
        {
            Debug.Log("YO DUMDUM NO DESIRED SLOT FOUND");
            return;
        }
        if (_activeSlot.Free == false)
        {
            GetComponent<Image>().enabled = true;
            GetComponent<Canvas>().enabled = true;
            GetComponent<GraphicRaycaster>().enabled = true;
            for (int i = 0; i < this.transform.childCount; i++)
                this.transform.GetChild(i).gameObject.SetActive(true);

            RectTransform rt = GetComponent<RectTransform>();

            rt.SetParent(desiredSlot);
            rt.anchoredPosition = new Vector2(0.0f, 0.0f);
            rt.SetParent(desiredSlotHolder.parent);

            ItemType itemType = _activeSlot.CurrentItem.GetItemType();
            switch(itemType)
            {
                case ItemType.FOOD:
                    transform.GetChild(0).GetComponent<Text>().text = "Eat";
                    break;
                case ItemType.LIQUID:
                case ItemType.MEDICINE:
                    transform.GetChild(0).GetComponent<Text>().text = "Use";
                    break;
                default:
                    transform.GetChild(0).GetComponent<Text>().text = "Equip";
                    break;
            }
        }
        else
        {
            return;
        }
        

        
    }

    public void Deactivate()
    {
        _activated = false;
        this.GetComponent<Image>().enabled = false;
        this.GetComponent<Canvas>().enabled = false;
        this.GetComponent<GraphicRaycaster>().enabled = false;
        for (int i = 0; i < this.transform.childCount; i++)
            this.transform.GetChild(i).gameObject.SetActive(false);
    }

    public void UseOREquipAction()
    {
        Debug.Log("USEOREQUIP");
        if (!_activeSlot.Free)
        {
            switch (_activeSlot.CurrentItem.Type)
            {
                case ItemType.WEAPON:
                    break;
                case ItemType.HEAD_ARMOR:
                    break;
                case ItemType.CHEST_ARMOR:
                    break;
                case ItemType.ARM_ARMOR:
                    break;
                case ItemType.LEG_ARMOR:
                    break;
                case ItemType.BOOTS:
                    break;
                case ItemType.FOOD:
                    _eatDelegate.Invoke(_activeSlot.CurrentItem.ID);
                    break;
                case ItemType.LIQUID:
                    _liquidDelegate.Invoke(_activeSlot.CurrentItem.ID);
                    break;
                case ItemType.MEDICINE:
                    break;
                default:
                    Debug.Log("WRONG ITEM TYPE YOU DUM DUM");
                    return;
            }
        }

        _activeSlot.Emptify();
        Deactivate();
    }

    private void EatItemByID(int itemID)
    {
        switch (itemID)
        {
            case 2:
                
                break;
            default:
                Debug.Log("WRONG ITEM ID YOU MORON");
                break;
        }
    }

    public void AboutAction()
    {
        Debug.Log("ABOUT");
        Deactivate();
    }

    public void DropAction()
    {
        Debug.Log("DROP");
        if (!_activeSlot.Free)
        {
            _playerInventory.DropItem(_activeSlot.CurrentItem);
        }
        Deactivate();
    }

    public void SetFoodDelegate(Action<int> action)
    {
        _eatDelegate = action;
    }

    public void SetLiquidDelegate(Action<int> action)
    {
        _liquidDelegate = action;
    }
}
