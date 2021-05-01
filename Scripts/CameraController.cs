using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private float lerpSpeed;

    private void FixedUpdate()
    {
        Vector3 newPosition = Vector3.Lerp(transform.position, new Vector3(targetTransform.position.x, transform.position.y, targetTransform.position.z), lerpSpeed * Time.deltaTime);
        transform.position = newPosition;
    }
}
