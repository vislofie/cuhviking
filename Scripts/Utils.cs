using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static uint X_AXIS = 0b_100;
    public static uint Y_AXIS = 0b_010;
    public static uint Z_AXIS = 0b_001;

    private static GameObject _playerObj = null;

    /// <summary>
    /// Gets distance from given position to player's position
    /// </summary>
    /// <param name="pos">given position</param>
    /// <param name="axes">axes to consider in calculation</param>
    /// <returns></returns>
    public static float GetDistanceToPlayer(Vector3 pos, uint axes)
    {
        if (_playerObj == null)
            _playerObj = GameObject.FindGameObjectWithTag("Player");

        Vector3 cutOff = new Vector3((axes & (1 << 2)) >> 2, (axes & (1 << 2)) >> 2, axes & 1); // vector that used to cutoff unneeded axes
        return Vector3.Distance(Vector3.Scale(pos, cutOff), Vector3.Scale(_playerObj.transform.position, cutOff));
    }

    public static float GetAngleFromVectorXZ(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public static Vector3 GetVectorFromAngleXZ(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
    }
}
