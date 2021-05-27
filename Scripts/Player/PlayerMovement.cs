using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed;

    private Vector3 _mousePos;

    // Moves player according to horziontal and vertical input axes
    public void UpdatePosition(float horizontal, float vertical)
    {
        Vector3 newMovement = new Vector3(horizontal, 0, vertical) * Time.deltaTime * _movementSpeed;

        transform.Translate(newMovement, Space.World);
    }

    // Rotates player towards a point from mouse position
    public void UpdateRotation()
    {
        _mousePos = Input.mousePosition;
        _mousePos.z = Camera.main.transform.position.y;
        _mousePos = Camera.main.ScreenToWorldPoint(_mousePos);
        _mousePos.y = transform.position.y;

        transform.LookAt(_mousePos);
    }
}
