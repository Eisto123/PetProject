using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class State_Idle
{
    public static float RecalcIdleTime = 1f;
    public static float RecalcIdleTimer = 0f;
    public static float WaitIdleMinTime = 4f;
    public static float WaitIdleMaxTime = 6f;
    public static float CurentIdleWaitTime = WaitIdleMinTime;
    public static float IdleRadiusCheck = 3f;

    // State
    public static Queue<Vector3> RandomPoints = new();

    public static void UpdateCurrentIdleWaitTime()
    {
        CurentIdleWaitTime = Random.Range(WaitIdleMinTime, WaitIdleMaxTime);
    }
}
