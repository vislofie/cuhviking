using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed;

    [SerializeField]
    private float _rotationSpeed;

    public void UpdatePosition(float horizontal, float vertical)
    {
        Vector3 newMovement = new Vector3(horizontal, 0, vertical) * Time.deltaTime * _movementSpeed;
        transform.Translate(newMovement);
    }

    public void UpdateRotation()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 1;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        // TODO: ROTATE PLAYER
    }
}
