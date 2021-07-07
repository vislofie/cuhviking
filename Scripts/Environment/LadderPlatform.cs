using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderPlatform : MonoBehaviour
{
    [SerializeField]
    private bool _start;

    /// <summary>
    /// Activates climbing and decides where it should start - at the start platform or the end platform
    /// </summary>
    public void ActivateClimbing()
    {
        this.GetComponentInParent<Ladder>().ActivateClimbing(_start);
    }
}
