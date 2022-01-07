using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ItemType { WEAPON, HEAD_ARMOR, CHEST_ARMOR, ARM_ARMOR, LEG_ARMOR, BOOTS, FOOD, WATER, MEDICINE };
public class Item
{
    public int ID { get { return id; } }
    public int Slot { get { return slot; } }
    public bool QuickSlot { get { return quickSlot; } }
    public int Amount { get { return amount; } }
    public int MaxAmount { get { return maxAmount; } }
    public int CurrentItemInstance { get { return curItemInstance; } } // which separate instance of items with this id in the inventory
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

    public Item()
    {
        this.id = -1;
        this.slot = -1;
        this.quickSlot = false;
        this.amount = 0;
        this.maxAmount = 0;
        this.curItemInstance = -1;
        this.sprite = null;
        this.name = "";
        this.itemType = ItemType.WEAPON;
    }
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
            Debug.Log("ChangeAmount was called and this amount is " + this.amount);
        }
        else
        {
            Debug.Log("Amount added is more than you can add!!");
        }
    }

    public void ChangeSlotID(int slotID)
    {
        this.slot = slotID;
    }

    public void ChangeQuickSlot(bool quickSlot)
    {
        this.quickSlot = quickSlot;
    }

    public ItemType GetItemType()
    {
        return this.itemType;
    }
}
public class PlayerInventory : MonoBehaviour
{
    public bool MouseInInventory { get { return _mouseInInventory; } }
    private bool _mouseInInventory;

    public Transform InventorySlotsHolder { get { return _inventorySlotsHolder; } }
    public Transform InventoryQuickSlotsHolder { get { return _inventoryQuickSlotsHolder; } }

    [SerializeField]
    private InventoryContextMenu _contextMenu;

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
        AddItem(0, 1);
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

        _mouseInInventory = CheckIfCursorInsideInventory();
    }

    /// <summary>
    /// Adds item to the inventory using item ID and given amount
    /// </summary>
    /// <param name="itemID">id of the given item</param>
    /// <param name="amount">amount of the given item</param>
    public void AddItem(int itemID, int amount)
    {
        for (int i = 0; i < _inventoryItems.Count; i++)
        {
            if (_inventoryItems[i].ID == itemID) // if there exists an item that has the same id as given
            {
                int tempAmount = Mathf.Clamp(amount, 0, _inventoryItems[i].MaxAmount - _inventoryItems[i].Amount);
                amount -= tempAmount;
                _inventoryItems[i].ChangeAmount(tempAmount);
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
                if (firstFreeSlot == -1) // no empty slots at all
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
                _inventoryItems.Add(item);
                
            }
        }
        UpdateVisualInfo();
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
            //InventorySlot slot = quickSlot ? _inventoryQuickSlotTransforms[slotID].GetComponent<InventorySlot>() : _inventorySlotTransforms[slotID].GetComponent<InventorySlot>();

            if (amount == -1 || _inventoryItems[itemIDInList].Amount - amount == 0)
            {
                //slot.Emptify();
                _inventoryItems.Remove(item);
            }
            else
            {
                item.ChangeAmount(-amount);
                _inventoryItems[itemIDInList] = item;
                //slot.Emptify();
                //slot.Fill(item);
            }
        }
        UpdateVisualInfo();
    }

    /// <summary>
    /// Drops a GameObject of the given Item and removes it from the inventory if needed
    /// </summary>
    /// <param name="item">item to drop</param>
    /// <param name="amount">amount of item to drop, equals to -1 if its required to drop all of it</param>
    public void DropItem(Item item, int amount = -1)
    {
        if (_inventoryItems.Contains(item))
        {
            int itemIDInList = -1;
            for (int i = 0; i < _inventoryItems.Count; i++)
                if (_inventoryItems[i].Equals(item))
                    itemIDInList = i;

            bool quickSlot = item.QuickSlot;
            int slotID = item.Slot;
            int itemID = item.ID;

            InventorySlot slot = quickSlot ? _inventoryQuickSlotTransforms[slotID].GetComponent<InventorySlot>() : _inventorySlotTransforms[slotID].GetComponent<InventorySlot>();

            
            
            if (amount != -1)
            {
                for (int i = 0; i < amount; i++)
                {
                    GameObject collectable = Instantiate(_itemPrefabs[itemID]);
                    collectable.transform.position = transform.position;
                }
                item.ChangeAmount(-amount);
                if (item.Amount <= 0)
                {
                    _inventoryItems.Remove(item);
                    slot.Emptify();
                }
            }
            else
            {
                GameObject collectable = Instantiate(_itemPrefabs[itemID]);
                collectable.transform.position = transform.position;
                slot.Emptify();
                _inventoryItems.Remove(item);
            }
        }
    }

    /// <summary>
    /// Updates visual info of the inventory
    /// </summary>
    private void UpdateVisualInfo()
    {
        foreach (Transform slot in _inventorySlotTransforms)
        {
            slot.GetComponent<InventorySlot>().Emptify();
        }
        foreach (Transform quickSlot in _inventoryQuickSlotTransforms)
        {
            quickSlot.GetComponent<InventorySlot>().Emptify();
        }
        foreach (Item item in _inventoryItems)
        {
            bool quickSlot = item.QuickSlot;
            int slotID = item.Slot;
            InventorySlot slot = quickSlot ? _inventoryQuickSlotTransforms[slotID].GetComponent<InventorySlot>() : _inventorySlotTransforms[slotID].GetComponent<InventorySlot>();
            slot.Fill(item);
        }
    }

    /// <summary>
    /// Swaps two existent items in the inventory
    /// </summary>
    /// <param name="firstItem"></param>
    /// <param name="secondItem"></param>
    public void SwapItems(Item firstItem, Item secondItem)
    {
        if (_inventoryItems.Contains(firstItem) && _inventoryItems.Contains(secondItem))
        {
            int firstInventoryID = -1;
            int secondInventoryID = -1;
            for (int i = 0; i < _inventoryItems.Count; i++)
            {
                if (_inventoryItems[i] == firstItem) firstInventoryID = i;
                else if (_inventoryItems[i] == secondItem) secondInventoryID = i;
            }
            int firstSlotID = _inventoryItems[firstInventoryID].Slot;
            bool firstQuickSlot = _inventoryItems[firstInventoryID].QuickSlot;

            _inventoryItems[firstInventoryID].ChangeSlotID(_inventoryItems[secondInventoryID].Slot);
            _inventoryItems[secondInventoryID].ChangeSlotID(firstSlotID);

            _inventoryItems[firstInventoryID].ChangeQuickSlot(_inventoryItems[secondInventoryID].QuickSlot);
            _inventoryItems[secondInventoryID].ChangeQuickSlot(firstQuickSlot);
            UpdateVisualInfo();
        }
    }

    /// <summary>
    /// Moves an item from one slot to another
    /// </summary>
    /// <param name="beforeSlot">slot before the move</param>
    /// <param name="beforeQuick">slot before was a quick slot</param>
    /// <param name="afterSlot">slot after the move</param>
    /// <param name="afterQuick">slot after was a quick slot</param>
    public void MoveItemToSlot(int beforeSlotID, bool beforeQuick, int afterSlotID, bool afterQuick)
    {
        InventorySlot beforeSlot = beforeQuick ? _inventoryQuickSlotTransforms[beforeSlotID].GetComponent<InventorySlot>() : _inventorySlotTransforms[beforeSlotID].GetComponent<InventorySlot>();
        InventorySlot afterSlot = afterQuick ? _inventoryQuickSlotTransforms[afterSlotID].GetComponent<InventorySlot>() : _inventorySlotTransforms[afterSlotID].GetComponent<InventorySlot>();

        Item passingItem = beforeSlot.CurrentItem;
        passingItem.ChangeQuickSlot(afterQuick);

        beforeSlot.Emptify();
        afterSlot.Fill(passingItem);
        passingItem.ChangeSlotID(afterSlotID);
        UpdateVisualInfo();
    }

    /// <summary>
    /// Calls context menu with given slotID and quickSlot identifier
    /// </summary>
    /// <param name="slotID"></param>
    /// <param name="quickSlot"></param>
    public void CallContextMenu(int slotID, bool quickSlot)
    {
        _contextMenu.Activate(slotID, quickSlot);
    }

    /// <summary>
    /// Returns first available slot ID
    /// </summary>
    /// <param name="quickSlot">whether we search for the first quick slot or regular slot</param>
    /// <returns></returns>
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

    /// <summary>
    /// Returns next instance number property of an item with given itemID
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Returns max amount of items in one slot of the inventory with item ID as input
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    private int GetMaxAmountByItemID(int itemID)
    {
        switch(itemID)
        {
            case 0: // AXE
                return 1;
            case 1: // SWORD
                return 1;
            default:
                return -1;
        }
    }

    /// <summary>
    /// Checkes either the cursor is inside the inventory
    /// </summary>
    /// <returns>ture if it is, false if its not</returns>
    private bool CheckIfCursorInsideInventory()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);


        for (int i = 0; i < results.Count; i++)
            if (results[i].gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;

        return false;
    }
}