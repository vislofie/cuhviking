using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int ID { get { return _ID; } }
    public bool QuickSlot { get { return _quickSlot; } }
    public bool Free { get { return _free; } }
    public Item CurrentItem { get { return _currentItem; } }

    private Image _itemImage;

    [SerializeField]
    private bool _quickSlot;
    private int _ID;
    private bool _free;
    private Item _currentItem;
    private Item _defaultItem;

    private PlayerInventory _playerInventory;

    private void Awake()
    {
        _itemImage = this.transform.GetChild(0).GetComponent<Image>();
        _ID = int.Parse(this.transform.name.Replace("Slot", ""));
        _free = true;
        _defaultItem = new Item();

        _playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
    }

    /// <summary>
    /// Fills current slot with a given item
    /// </summary>
    /// <param name="item">item to fill slot with</param>
    public void Fill(Item item)
    {
        _currentItem = item;

        _itemImage.sprite = item.ItemSprite;
        _itemImage.color = new Color(1, 1, 1, 1);
        _itemImage.transform.GetChild(0).GetComponent<Text>().text = "x" + item.Amount;
        _free = false;
    }

    /// <summary>
    /// Makes this slot empty
    /// </summary>
    public void Emptify()
    {
        _currentItem = _defaultItem;
        _itemImage.sprite = null;
        _itemImage.color = new Color(1, 1, 1, 0);
        _itemImage.transform.GetChild(0).GetComponent<Text>().text = "";
        _free = true;
    }

    /// <summary>
    /// Swaps items with some other inventory slot
    /// </summary>
    /// <param name="inventorySlot"></param>
    public void Swap(InventorySlot inventorySlot)
    {
        Item otherItem = inventorySlot.CurrentItem;
        inventorySlot.Fill(this.CurrentItem);
        this.Fill(otherItem);
    }

    /// <summary>
    /// Event that's called when a visual of an item drops on a visual of this slot
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            if (eventData.pointerDrag.GetComponent<InventoryItemVisual>().InteractionAllowed)
            {
                eventData.pointerDrag.GetComponent<RectTransform>().offsetMax = new Vector2(-3, -3);
                eventData.pointerDrag.GetComponent<RectTransform>().offsetMin = new Vector2(3, 3);
                InventorySlot slot = eventData.pointerDrag.GetComponentInParent<InventorySlot>();
                if (_currentItem.CurrentItemInstance != -1)
                {
                    _playerInventory.SwapItems(_currentItem, slot.CurrentItem);
                }
                else
                {
                    _playerInventory.MoveItemToSlot(slot.ID, slot.QuickSlot, _ID, _quickSlot);
                }
            }
        }
    }
}
