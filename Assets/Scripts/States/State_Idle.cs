using System.Collections.Generic;
using UnityEngine;

public static class State_Idle
{
    public static float RecalcRandomPointTime = 1f;
    public static float RecalcRandomPointTimer = 0f;

    public static float WaitBeforeMovingMinTime = 4f;
    public static float WaitBeforeMovingMaxTime = 6f;
    public static float CurentWaitBeforeMovingTime = WaitBeforeMovingMinTime;
    public static float WaitBeforeMovingTimer = 0f;

    public static float IdleRadiusCheck = 3f;

    // State
    public static Queue<Vector3> RandomPoints = new();

    public static void UpdateCurrentWaitBeforeMovingTime()
    {
        CurentWaitBeforeMovingTime = Random.Range(WaitBeforeMovingMinTime, WaitBeforeMovingMaxTime);
    }
}
