using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OuterLayer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEndDragHandler
{
    [SerializeField]
    private ItemContextMenu _contextMenu;

    private bool _playerMovementBlocked;

    private void Start()
    {
        _playerMovementBlocked = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (_contextMenu.Activated)
            {
                _contextMenu.Deactivate();
                _contextMenu.ForbidPlayerInteraction();
                _playerMovementBlocked = true;
            }
        }
            

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_playerMovementBlocked)
        {
            _contextMenu.AllowPlayerInteraction();
            _playerMovementBlocked = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    
}
