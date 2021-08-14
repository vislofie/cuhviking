using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private enum ITEM_IDS { Axe = 0 }
    public int ItemID { get { return (int)_itemID; } }

    [SerializeField]
    private LayerMask _collectablesLayerMask;
    [SerializeField]
    private ITEM_IDS _itemID;
    private GameObject _collectableIcon;
    private bool _iconActivated;

    private void Awake()
    {
        _collectableIcon = transform.GetChild(0).gameObject;
        _iconActivated = false;
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.PositiveInfinity, _collectablesLayerMask))
        {
            if (!_iconActivated)
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
}
