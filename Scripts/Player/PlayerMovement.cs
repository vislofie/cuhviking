using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Tooltip("0 - crouching speed, 1 - walking speed, 2 - running speed")]
    [SerializeField]
    private float[] _movementSpeeds = new float[3] { 1.0f, 3.0f, 5.0f };

    [SerializeField]
    private float _movementSpeed;

    public Vector3 MousePosition { get { return _mousePos; } }
    private Vector3 _mousePos;

    /// <summary>
    /// Moves player according to given horziontal and vertical input axes
    /// </summary>
    /// <param name="horizontal">Value of horizontal axis</param>
    /// <param name="vertical">Value of vertical axis</param>
    public void UpdatePosition(float horizontal, float vertical)
    {
        Vector3 newMovement = new Vector3(horizontal, 0, vertical) * Time.deltaTime * _movementSpeed;

        transform.Translate(newMovement, Space.World);
    }

    /// <summary>
    /// Rotates player towards a point from mouse position
    /// </summary>
    public void UpdateRotation()
    {
        _mousePos = Input.mousePosition;
        _mousePos.z = Camera.main.transform.position.y;
        _mousePos = Camera.main.ScreenToWorldPoint(_mousePos);
        _mousePos.y = transform.position.y;

        transform.LookAt(_mousePos);
    }

    /// <summary>
    /// Changes movement speed dependant on movementState of the player
    /// </summary>
    /// <param name="movementState">Player's movement state</param>
    public void ChangeMovementSpeed(MovementState movementState)
    {
        _movementSpeed = _movementSpeeds[(int)movementState];
    }
}
