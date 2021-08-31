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

    private Transform _previousParent;

    
    

    private void Awake()
    {
        _playerBrain = this.transform.parent.parent.parent.GetComponent<UIInventory>().PlayerBrainScript;
        _rectTransform = this.GetComponent<RectTransform>();
        _canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        _canvasGroup = this.GetComponent<CanvasGroup>();

        _isEmpty = true;
        UpdateSlotIDAndPos();

    }

    public void OnPointerDown(PointerEventData eventData)
    {
       // Debug.Log("OnPointerDown");
        _playerBrain.ForbidAttackAndInteractionMovement();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp");
        _playerBrain.AllowAttackAndInteractionMovement();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
        if (!_isEmpty)
        {
            this.transform.parent.GetComponent<Image>().raycastTarget = false;
            _canvasGroup.blocksRaycasts = false;
            _previousParent = this.transform.parent;
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
            this.transform.parent.GetComponent<Image>().raycastTarget = true;
            _canvasGroup.blocksRaycasts = true;
            if (_previousParent == this.transform.parent) _rectTransform.anchoredPosition = Vector3.zero;
        } 
    }

    public void UpdateSlotIDAndPos()
    {
        _parentSlot = this.transform.parent.GetComponent<ItemSlot>();
        CurrentSlotID = _parentSlot.SlotNumber;
        QuickSlot = _parentSlot.QuickSlot;
        _rectTransform.anchoredPosition = Vector2.zero;
    }

    public void MakeSlotEmpty()
    {
        _isEmpty = true;
    }

    public void MakeSlotFilled()
    {
        _isEmpty = false;
    }

    
}
