using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InventoryItemVisual : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public bool InteractionAllowed { get { return _interactionAllowed; } }
    private bool _interactionAllowed;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;

    private PlayerInventory _playerInventory;
    private InventorySlot _slot;


    private void Start()
    {
        ForbidInteraction();
        _rectTransform = this.GetComponent<RectTransform>();
        _canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        _canvasGroup = this.GetComponent<CanvasGroup>();
        _playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        _slot = transform.parent.GetComponent<InventorySlot>();
        AllowInteraction();
    }

    /// <summary>
    /// Forbids to interact player with this item
    /// </summary>
    public void ForbidInteraction()
    {
        _interactionAllowed = false;
    }

    /// <summary>
    /// Allows to interact player with this item
    /// </summary>
    public void AllowInteraction()
    {
        _interactionAllowed = true;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_interactionAllowed)
        {
            _canvasGroup.alpha = 0.6f;
            _canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_interactionAllowed)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_interactionAllowed)
        {
            _canvasGroup.alpha = 1.0f;
            _canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            _playerInventory.CallContextMenu(_slot.ID, _slot.QuickSlot);
        }
    }
}
