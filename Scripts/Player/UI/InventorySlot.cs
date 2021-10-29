using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public int ID { get { return _ID; } }
    public bool Free { get { return _free; } }
    public Item CurrentItem { get { return _currentItem; } }

    private Image _itemImage;

    private int _ID;
    private bool _free;
    private Item _currentItem;
    private Item _defaultItem;

    private void Awake()
    {
        _itemImage = this.transform.GetChild(0).GetComponent<Image>();
        _ID = int.Parse(this.transform.name.Replace("Slot", ""));
        _free = true;
        _defaultItem = new Item();
    }

    public void Fill(Item item)
    {
        _currentItem = item;

        _itemImage.sprite = item.ItemSprite;
        _itemImage.color = new Color(1, 1, 1, 1);
        _itemImage.transform.GetChild(0).GetComponent<Text>().text = "x" + item.Amount;
        _free = false;
    }

    public void Emptify()
    {
        _currentItem = _defaultItem;
        _itemImage.sprite = null;
        _itemImage.color = new Color(1, 1, 1, 0);
        _itemImage.transform.GetChild(0).GetComponent<Text>().text = "";
        _free = true;
    }

}
