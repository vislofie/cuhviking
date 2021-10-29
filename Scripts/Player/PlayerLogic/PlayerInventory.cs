using System.Collections.Generic;
using UnityEngine;

public enum ItemType { WEAPON, HEAD_ARMOR, CHEST_ARMOR, ARM_ARMOR, LEG_ARMOR, BOOTS, FOOD, WATER, MEDICINE };
public struct Item
{
    public int ID { get { return id; } }
    public int Slot { get { return slot; } }
    public bool QuickSlot { get { return quickSlot; } }
    public int Amount { get { return amount; } }
    public int MaxAmount { get { return maxAmount; } }
    public int CurrentItemInstance { get { return CurrentItemInstance; } } // which separate instance of items with this id in the inventory
    public Sprite ItemSprite { get { return sprite; } }
    public string Name { get { return name; } }
    public ItemType Type { get { return itemType; } }

    private int id;
    private int slot;
    private bool quickSlot;
    private int amount;
    private int maxAmount;
    private int curItemInstance;
    private Sprite sprite;
    private string name;
    private ItemType itemType;

    public Item(int id, int slot, bool quickSlot, int curItemInstance, Sprite sprite, string name, ItemType itemType, int amount = 1, int maxAmount = 64)
    {
        this.id = id;
        this.slot = slot;
        this.quickSlot = quickSlot;
        this.amount = amount;
        this.maxAmount = maxAmount;
        this.curItemInstance = curItemInstance;
        this.sprite = sprite;
        this.name = name;
        this.itemType = itemType;
    }

    public void ChangeAmount(int amount)
    {
        if (this.amount + amount <= this.maxAmount)
        {
            this.amount += amount;
        }
        else
        {
            Debug.Log("Amount added is more than you can add!!");
        }
    }
}
public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private Transform _inventorySlotsHolder;
    [SerializeField]
    private Transform _inventoryQuickSlotsHolder;
    private Transform[] _inventorySlotTransforms;
    private Transform[] _inventoryQuickSlotTransforms;

    private List<Item> _inventoryItems = new List<Item>();

    private Dictionary<int, GameObject> _itemPrefabs = new Dictionary<int, GameObject>();
    private Sprite[] _sprites;

    private void Awake()
    {
        GameObject[] itemPrefabsArr = Resources.LoadAll<GameObject>("Prefabs/Collectables");
        foreach (GameObject item in itemPrefabsArr)
        {
            string name = item.name;
            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] == '_')
                {
                    _itemPrefabs.Add(int.Parse(name.Substring(0, i)), item);
                    break;
                }
            }
        }

        _sprites = Resources.LoadAll<Sprite>("Sprites/Items");

        _inventorySlotTransforms = new Transform[_inventorySlotsHolder.childCount];
        _inventoryQuickSlotTransforms = new Transform[_inventoryQuickSlotsHolder.childCount];

        for (int i = 0; i < _inventorySlotTransforms.Length; i++)
            _inventorySlotTransforms[i] = _inventorySlotsHolder.GetChild(i);
        for (int i = 0; i < _inventoryQuickSlotTransforms.Length; i++)
            _inventoryQuickSlotTransforms[i] = _inventoryQuickSlotsHolder.GetChild(i);
    }

    private void Start()
    {
        AddItem(0, 26);
        RemoveItem(_inventorySlotTransforms[2].GetComponent<InventorySlot>().CurrentItem);
        RemoveItem(_inventorySlotTransforms[3].GetComponent<InventorySlot>().CurrentItem, 2);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("ITEMS\n");
            for (int i = 0; i < _inventoryItems.Count; i++)
            {
                Debug.Log("ITEM AT SLOT ID " + _inventoryItems[i].Slot + " WITH A NAME " + _inventoryItems[i].Name + " IS " + (_inventoryItems[i].QuickSlot ? "A QUICK" : "NOT A QUICK") + "SLOT HAS ID " + _inventoryItems[i].ID + "; AMOUNT = " + _inventoryItems[i].Amount);
            }
        }
    }

    public void AddItem(int itemID, int amount)
    {
        foreach (Item item in _inventoryItems)
        {
            if (item.ID == itemID) // если среди вещей в инвентаре есть тот же айдишник, что и данный в аргументах
            {
                int tempAmount = Mathf.Clamp(amount, 0, item.MaxAmount - item.Amount);
                amount -= tempAmount;
                item.ChangeAmount(tempAmount);
            }
        }

        while (amount > 0)
        {
            int firstFreeSlot = GetFirstFreeSlotID(false);
            bool quickSlot = false;
            if (firstFreeSlot == -1)
            {
                firstFreeSlot = GetFirstFreeSlotID(true);
                Debug.Log(firstFreeSlot);
                quickSlot = true;
                if (firstFreeSlot == -1) // свободных слотов ну вообще нет
                {
                    Debug.Log("Inventory is full!!!!!");
                    break;
                }
            }

            if (firstFreeSlot != -1)
            {
                int curInstance = GetNextInstanceNumberOfSameItem(itemID);
                int amountToAdd = Mathf.Clamp(amount, 1, GetMaxAmountByItemID(itemID));
                amount -= amountToAdd;
                Item item = new Item(itemID, firstFreeSlot, quickSlot, curInstance, _sprites[itemID], _sprites[itemID].name.Remove(0, itemID.ToString().Length + 1),
                                        _itemPrefabs[itemID].GetComponent<Collectable>().Type, amountToAdd, GetMaxAmountByItemID(itemID));

                InventorySlot desiredSlot = quickSlot ? _inventoryQuickSlotTransforms[firstFreeSlot].GetComponent<InventorySlot>() : _inventorySlotTransforms[firstFreeSlot].GetComponent<InventorySlot>();
                if (desiredSlot.Free)
                {
                    Debug.Log("DesiredSlot Free");
                    desiredSlot.Fill(item);
                }
                _inventoryItems.Add(item);
            }
        }
    }

    /// <summary>
    /// Removes item from the inventory
    /// </summary>
    /// <param name="itemID">id of the item to remove</param>
    /// <param name="amount">amount to remove. -1 means all of it</param>
    public void RemoveItem(int itemID, int amount = -1)
    {

    }

    /// <summary>
    /// Removes item from the ivnentory
    /// </summary>
    /// <param name="item">item to remove</param>
    /// <param name="amount">amount to remove. -1 means all of it</param>
    public void RemoveItem(Item item, int amount = -1)
    {
        if (_inventoryItems.Contains(item))
        {
            int itemIDInList = -1;
            for (int i = 0; i < _inventoryItems.Count; i++)
                if (_inventoryItems[i].Equals(item))
                    itemIDInList = i;

            bool quickSlot = item.QuickSlot;
            int slotID = item.Slot;
            InventorySlot slot = quickSlot ? _inventoryQuickSlotTransforms[slotID].GetComponent<InventorySlot>() : _inventorySlotTransforms[slotID].GetComponent<InventorySlot>();

            if (amount == -1 || _inventoryItems[itemIDInList].Amount - amount == 0)
            {
                slot.Emptify();
                _inventoryItems.Remove(item);
            }
            else
            {
                item.ChangeAmount(-amount);
                _inventoryItems[itemIDInList] = item;
                slot.Emptify();
                slot.Fill(item);
            }
        }  
    }

    private int GetFirstFreeSlotID(bool quickSlot)
    {
        int firstFreeSlot = -1;

        if (quickSlot)
        {
            for (int i = 0; i < _inventoryQuickSlotTransforms.Length; i++)
            {
                if (_inventoryQuickSlotTransforms[i].GetComponent<InventorySlot>().Free)
                {
                    firstFreeSlot = i;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < _inventorySlotTransforms.Length; i++)
            {
                if (_inventorySlotTransforms[i].GetComponent<InventorySlot>().Free)
                {
                    firstFreeSlot = i;
                    break;
                }
            }
        }


        return firstFreeSlot;
    }

    private int GetNextInstanceNumberOfSameItem(int itemID)
    {
        int curInstance = 0;
        foreach (Item item in _inventoryItems)
        {
            if (item.ID == itemID)
            {
                curInstance++;
            }
        }

        return curInstance;
    }

    private int GetMaxAmountByItemID(int itemID)
    {
        switch(itemID)
        {
            case 0: // AXE
                return 3;
            default:
                return -1;
        }
    }
}