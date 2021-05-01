using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;

    public void UpdatePosition(float horizontal, float vertical)
    {
        Vector3 newMovement = new Vector3(horizontal, 0, vertical) * Time.deltaTime * movementSpeed;
        transform.Translate(newMovement);
    }
}
