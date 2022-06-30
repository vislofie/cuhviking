using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    
    public int ItemID { get { return (int)_itemID; } }

    [SerializeField]
    private LayerMask _collectablesLayerMask;
    [SerializeField]
    private ItemID _itemID;
    [SerializeField]
    private int _amount;
    [SerializeField]
    private int _maxAmount;
    [SerializeField]
    private ItemType _itemType;
    [SerializeField]
    private float _distanceToPickUp = 1; // minimum distance to be able for player to pick up this item

    public int Amount { get { return _amount; } }
    public int MaxAmount { get { return _maxAmount; } }
    public ItemType Type { get { return _itemType; } }
    private GameObject _collectableIcon;

    private bool _iconActivated;
    public bool IconActivated => _iconActivated;
    

    private void Awake()
    {
        _collectableIcon = transform.GetChild(0).gameObject;
        _iconActivated = false;
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.PositiveInfinity, _collectablesLayerMask) && Utils.GetDistanceToPlayer(transform.position, Utils.X_AXIS | Utils.Z_AXIS) <= _distanceToPickUp)
        {
            if (hit.collider.gameObject == this.gameObject && !_iconActivated)
            {
                _iconActivated = true;
                _collectableIcon.SetActive(true);
            }
        }
        else
        {
            if (_iconActivated)
            {
                _iconActivated = false;
                _collectableIcon.SetActive(false);
            }
        }
    }

    public void SetAmount(int amount)
    {
        _amount = amount;
    }

    public void ChangeAmount(int dAmount)
    {
        _amount += dAmount;
    }

    public void DestroyItself()
    {
        Destroy(this.gameObject);
    }

}
