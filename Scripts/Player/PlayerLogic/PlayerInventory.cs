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
    private List<Transform> _quickSlots = new List<Transform>();
    private Dictionary<int, int> _slotsToItemID = new Dictionary<int, int>(); // key is the slot, value is the item id that is inside of this slot. if value is -1 then the slot is free
    private Dictionary<int, int> _quickSlotsToItemID = new Dictionary<int, int>();

    private Dictionary<int, GameObject> _itemPrefabs = new Dictionary<int, GameObject>();
    private Sprite[] _sprites;

    private bool _enabled = false;
    

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


        for (int i = 0; i < _inventoryHolder.childCount; i++)
        {
            _inventorySlots.Add(_inventoryHolder.GetChild(i));
            _slotsToItemID.Add(i, -1);
        }
        for (int i = 0; i < _quickSlotsHolder.childCount; i++)
        {
            _quickSlots.Add(_quickSlotsHolder.GetChild(i));
            _quickSlotsToItemID.Add(i, -1);
        }
    }

    private void Start()
    {
        UpdateInventory();
    }

    private void Update()
    {
        DebugInventory();
    }

    private void DebugInventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) _enabled = !_enabled;
        if (_enabled)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                Debug.Log("i = " + i + ", items ID = " + _items[i].id + ", amount = " + _items[i].amount);
            }
            for (int i = 0; i < _slotsToItemID.Count; i++)
            {
                Debug.Log("INVENTORY AT KEY " + i + ", VALUE IS " + _slotsToItemID[i]);
            }
            for (int i = 0; i < _quickSlotsToItemID.Count; i++)
            {
                Debug.Log("QUICK AT KEY " + i + ", VALUE IS " + _quickSlotsToItemID[i]);
            }
            for (int i = 0; i < _itemPrefabs.Count; i++)
            {
                Debug.Log("ITEM PREFABS AT KEY  " + i + ", VALUE IS " + _itemPrefabs[i]);
            }
            Debug.Log("\n--------DIVIDER--------DIVIDER--------DIVIDER--------\n");
            _enabled = false;
        }
    }
    /// <summary>
    /// updates visual info of your inventory
    /// </summary>
    public void UpdateInventory()
    {

        foreach (Item item in _items)
        {
            if (!_slotsToItemID.ContainsValue(item.id) && !_quickSlotsToItemID.ContainsValue(item.id))
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

        UpdateVisualOfHolders(_slotsToItemID, _inventorySlots);
        UpdateVisualOfHolders(_quickSlotsToItemID, _quickSlots);
    }

    /// <summary>
    /// Updated visual information about items in different item holders(quick slots and regular inventory)
    /// </summary>
    /// <param name="slotDictionary">dictionary of the holder</param>
    /// <param name="slotHolder">children of the holder</param>
    private void UpdateVisualOfHolders(Dictionary<int, int> slotDictionary, List<Transform> slotHolder)
    {
        for (int i = 0; i < slotHolder.Count; i++)
        {
            Image itemImg = slotHolder[i].GetChild(0).GetComponent<Image>();
            Text itemAmount = itemImg.transform.GetChild(0).GetComponent<Text>();

            if (slotDictionary[i] != -1)
            {
                Item curItem = _items[slotDictionary[i]];
                itemImg.sprite = curItem.img;
                itemImg.color = new Color(1, 1, 1, 1);


                itemAmount.text = 'x' + curItem.amount.ToString();

                itemImg.gameObject.GetComponent<ItemDragDrop>().MakeSlotFilled();
            }
            else
            {
                itemImg.sprite = null;
                itemImg.color = new Color(1, 1, 1, 0);

                itemAmount.text = "";
                itemImg.gameObject.GetComponent<ItemDragDrop>().MakeSlotEmpty();
            }
        }
    }

    /// <summary>
    /// Updates id info after moving an item from one slot to another
    /// </summary>
    /// <param name="previousSlot">the slot the item was moved FROM</param>
    /// <param name="previousQuick">was the previous slot a quick slot</param>
    /// <param name="newSlot">the slot the item was moved TO</param>
    /// <param name="newQuick">was the new slot a quick slot</param>
    public void UpdateAfterMoving(int previousSlot, bool previousQuick, int newSlot, bool newQuick)
    {
        if (previousQuick)
        {
            int itemID = _quickSlotsToItemID[previousSlot];
            if (newQuick)
            {
                _quickSlotsToItemID[previousSlot] = _quickSlotsToItemID[newSlot];
                _quickSlotsToItemID[newSlot] = itemID;
            }
            else
            {
                _quickSlotsToItemID[previousSlot] = _slotsToItemID[newSlot];
                _slotsToItemID[newSlot] = itemID;
            }
        }
        else
        {
            int itemID = _slotsToItemID[previousSlot];
            if (newQuick)
            {
                _slotsToItemID[previousSlot] = _quickSlotsToItemID[newSlot];
                _quickSlotsToItemID[newSlot] = itemID;
            }
            else
            {
                _slotsToItemID[previousSlot] = _slotsToItemID[newSlot];
                _slotsToItemID[newSlot] = itemID;
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
    /// <param name="amount">amount of the item. -1 means remove all of it</param>
    public void RemoveItem(int itemId, int amount = -1)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            Item curItem = _items[i];
            curItem.amount -= amount == -1 ? curItem.amount : amount;

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

    /// <summary>
    /// Gets a phyiscal representation of the item from the inventory
    /// </summary>
    /// <param name="id">item id</param>
    public GameObject GetPrefabFromItemID(int id)
    {
        if (_itemPrefabs != null && _itemPrefabs.ContainsKey(id))
            return _itemPrefabs[id];
        else
            return null;
    }

    /// <summary>
    /// Gets a physical representation of the item from the inventory
    /// </summary>
    /// <param name="slotID">slotID of the item to get a prefab from</param>
    /// <param name="quickSlot">whether its a quick slot or not</param>
    /// <returns></returns>
    public GameObject GetPrefabFromItemInSlot(int slotID, bool quickSlot)
    {
        GameObject prefab = null;
        if (quickSlot && _quickSlotsToItemID.ContainsKey(slotID))
            prefab = GetPrefabFromItemID(_quickSlotsToItemID[slotID]);
        else if (!quickSlot && _slotsToItemID.ContainsKey(slotID))
                prefab = GetPrefabFromItemID(_slotsToItemID[slotID]);

        return prefab;
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
