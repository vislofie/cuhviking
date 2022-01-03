using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryContextMenu : MonoBehaviour
{
    private PlayerInventory _playerInventory;
    private bool _activated;

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
            if (desiredSlotHolder.GetChild(i).GetComponent<InventorySlot>().ID == slotID) desiredSlot = desiredSlotHolder.GetChild(i).GetComponent<RectTransform>();

        if (desiredSlot == null)
        {
            Debug.Log("YO DUMDUM NO DESIRED SLOT FOUND");
            return;
        }

        InventorySlot slot = desiredSlot.GetComponent<InventorySlot>();
        if (slot.Free == false)
        {
            this.GetComponent<Image>().enabled = true;
            this.GetComponent<Canvas>().enabled = true;
            this.GetComponent<GraphicRaycaster>().enabled = true;
            for (int i = 0; i < this.transform.childCount; i++)
                this.transform.GetChild(i).gameObject.SetActive(true);

            this.GetComponent<RectTransform>().SetParent(desiredSlot);
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
            this.GetComponent<RectTransform>().SetParent(desiredSlotHolder.parent);

            ItemType itemType = slot.CurrentItem.GetItemType();
            switch(itemType)
            {
                case ItemType.FOOD:
                case ItemType.WATER:
                case ItemType.MEDICINE:
                    this.transform.GetChild(0).GetComponent<Text>().text = "Use";
                    break;
                default:
                    this.transform.GetChild(0).GetComponent<Text>().text = "Equip";
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
    }

    public void AboutAction()
    {
        Debug.Log("ABOUT");
    }

    public void DropAction()
    {
        Debug.Log("DROP");
    }
}
