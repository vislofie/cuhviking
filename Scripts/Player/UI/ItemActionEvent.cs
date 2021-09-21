using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemActionEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private ItemAction _action;

    private ItemContextMenu _contextMenu;

    private void Awake()
    {
        _contextMenu = this.GetComponentInParent<ItemContextMenu>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _contextMenu.ForbidPlayerInteraction();
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _contextMenu.MakeAnAction(_action);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _contextMenu.AllowPlayerInteraction();
    }
}
