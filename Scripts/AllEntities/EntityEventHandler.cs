using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEventHandler : MonoBehaviour
{
    public delegate void HearDelegate(GameObject obj);

    // delegates that are called when noise from current entity is produced
    private HearDelegate _hearingDelegates = null;


    /// <summary>
    /// Shows whether given method contains in _hearingDelegates delegates
    /// </summary>
    /// <param name="method"></param>
    private bool HearContains(HearDelegate method)
    {
        if (_hearingDelegates != null)
        {
            System.Delegate[] list = _hearingDelegates.GetInvocationList();
            foreach (System.Delegate deleg in list)
                if ((System.Delegate)method == deleg)
                    return true;
            return false;
        }
        return false;
    }

    /// <summary>
    /// Adding a delegate for reacting to noise from current entity when the delegate is called
    /// </summary>
    /// <param name="method">Delegate</param>
    public void AddHearer(HearDelegate method)
    {
        if (!HearContains(method))
            _hearingDelegates += method;
    }

    /// <summary>
    /// Removing a delegate for reacting to noise from current entity when the delegate is called
    /// </summary>
    /// <param name="method"></param>
    public void RemoveHearer(HearDelegate method)
    {
        if (HearContains(method))
            _hearingDelegates -= method;
    }

    /// <summary>
    /// Calls delegates responsible for reacting to noise from this current entity
    /// </summary>
    public void CallHearers()
    {
        if (_hearingDelegates != null)
            _hearingDelegates.Invoke(this.gameObject);
    }
}
