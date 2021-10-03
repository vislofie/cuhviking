using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.EventSystems;

public class ItemDragDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public int CurrentSlotID { get; private set; }
    public bool QuickSlot { get; private set; }

    public bool IsEmpty { get { return _isEmpty; } }
    private bool _isEmpty;

    private PlayerBrain _playerBrain;
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private ItemSlot _parentSlot;
    private ItemContextMenu _itemContextMenu;

    private Transform _previousParent;

    private void Awake()
    {
        _playerBrain = this.transform.parent.parent.parent.GetComponent<UIInventory>().PlayerBrainScript;
        _rectTransform = this.GetComponent<RectTransform>();
        _canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        _canvasGroup = this.GetComponent<CanvasGroup>();
        _itemContextMenu = GameObject.FindGameObjectWithTag("ItemContextMenu").GetComponent<ItemContextMenu>();

        _isEmpty = true;
        

    }

    private void Start()
    {
        UpdateSlotIDAndPos();
        _itemContextMenu.Deactivate();
    }

    /// <summary>
    /// Opens context menu of the current item
    /// </summary>
    private void OpenContextMenu()
    {
        _itemContextMenu.Activate(CurrentSlotID, QuickSlot);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _playerBrain.ForbidAttackAndInteractionMovement();
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (_itemContextMenu.Activated) _itemContextMenu.Deactivate();
        }
        else if (eventData.button == PointerEventData.InputButton.Right && !_isEmpty)
        {
            OpenContextMenu();
        }
       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _playerBrain.AllowAttackAndInteractionMovement();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isEmpty)
        {
            this.transform.parent.GetComponent<Image>().raycastTarget = false;
            _canvasGroup.blocksRaycasts = false;
            _previousParent = this.transform.parent;
            this.transform.SetParent(this.transform.parent.parent.parent);
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isEmpty)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isEmpty)
        {
            _previousParent.GetComponent<Image>().raycastTarget = true;
            _canvasGroup.blocksRaycasts = true;
            if (_previousParent == this.transform.parent) _rectTransform.anchoredPosition = Vector3.zero;
        } 
    }

    /// <summary>
    /// Updates slot ID of this item and whether its in quick slots or nah
    /// </summary>
    public void UpdateSlotIDAndPos()
    {
        _parentSlot = this.transform.parent.GetComponent<ItemSlot>();
        CurrentSlotID = _parentSlot.SlotNumber;
        QuickSlot = _parentSlot.QuickSlot;
        _rectTransform.anchoredPosition = Vector2.zero;
    }


    /// <summary>
    /// Makes slot empty??
    /// </summary>
    public void MakeSlotEmpty()
    {
        _isEmpty = true;
        _canvasGroup.blocksRaycasts = false;
    }


    /// <summary>
    /// Makes it not empty??
    /// </summary>
    public void MakeSlotFilled()
    {
        _isEmpty = false;
        _canvasGroup.blocksRaycasts = true;
    }

    
}
