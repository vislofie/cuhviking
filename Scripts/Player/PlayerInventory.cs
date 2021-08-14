using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public bool IsMainInventoryActive { get { return _inventoryHolder.gameObject.activeInHierarchy; } }

    public struct Item
    {
        public int id;
        public string name;
        public Sprite img;
        public int amount;

        public Item(int id, string name, Sprite img, int amount = 1)
        {
            this.id = id;
            this.amount = amount;
            this.name = "";
            this.img = img;
        }
    }

    [SerializeField]
    private Transform _inventoryHolder;
    [SerializeField]
    private Transform _quickSlotsHolder;

    
    private List<Item> _items = new List<Item>(); // items that player holds
    private List<Transform> _inventorySlots = new List<Transform>();
    private Dictionary<int, int> _slotsToItemID = new Dictionary<int, int>(); // key is the slot, value is the item id that is inside of this slot. if value is -1 then the slot is free
    private Sprite[] _sprites;
    

    private void Awake()
    {
        _sprites = Resources.LoadAll<Sprite>("Sprites/Items");


        for (int i = 0; i < _inventoryHolder.childCount; i++)
        {
            _inventorySlots.Add(_inventoryHolder.GetChild(i));
            _slotsToItemID.Add(i, -1);
        }
    }

    private void Start()
    {
        UpdateInventory();
    }

    /// <summary>
    /// updates visual info of your inventory
    /// </summary>
    public void UpdateInventory()
    {

        foreach (Item item in _items)
        {
            if (!_slotsToItemID.ContainsValue(item.id))
            {
                for (int i = 0; i < _slotsToItemID.Count; i++)
                {
                    if (_slotsToItemID[i] == -1)
                    {
                        _slotsToItemID[i] = item.id;
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < _inventorySlots.Count; i++)
        {
            Image itemImg = _inventorySlots[i].GetChild(0).GetComponent<Image>();
            Text itemAmount = itemImg.transform.GetChild(0).GetComponent<Text>();

            if (_slotsToItemID[i] != -1)
            {
                Item curItem = _items[_slotsToItemID[i]];
                itemImg.sprite = curItem.img;
                itemImg.color = new Color(1, 1, 1, 1);

                itemAmount.text = 'x' + curItem.amount.ToString();
            }
            else
            {
                itemImg.sprite = null;
                itemImg.color = new Color(1, 1, 1, 0);

                itemAmount.text = "";
            }
        }
    }

    /// <summary>
    /// Adds an item to the inventory with given amount
    /// </summary>
    /// <param name="itemId">id of the item</param>
    /// <param name="amount">amount of the item</param>
    public void AddItem(int itemId, int amount = 1)
    {
        Item newItem;

        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].id == itemId)
            {
                newItem = _items[i];
                newItem.amount += amount;
                _items[i] = newItem;
                break;
            }
        }

        newItem = new Item(itemId, _sprites[itemId].name.Remove(0, itemId.ToString().Length + 1), _sprites[itemId], amount);
        _items.Add(newItem);

        if (IsMainInventoryActive) UpdateInventory();
    }


    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    /// <param name="itemId">id of the item</param>
    /// <param name="amount">amount of the item</param>
    public void RemoveItem(int itemId, int amount = 1)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            Item curItem = _items[i];
            curItem.amount -= amount;

            if (curItem.amount <= 0)
            {
                _items.RemoveAt(i);

                for (int j = 0; j < _slotsToItemID.Count; j++)
                    if (_slotsToItemID[j] == curItem.id)
                        _slotsToItemID[j] = -1;

                break;
            }
            _items[i] = curItem;
        }

        if (IsMainInventoryActive) UpdateInventory();
    }

    public void ActivateMainInventory()
    {
        UpdateInventory();
        _inventoryHolder.gameObject.SetActive(true);
    }

    public void DisableMainInventory()
    {
        _inventoryHolder.gameObject.SetActive(false);
    }


}
