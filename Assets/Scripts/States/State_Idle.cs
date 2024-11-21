using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class State_Idle
{
    public static float RecalcIdleTime = 1f;
    public static float RecalcIdleTimer = 0f;
    public static Queue<Vector3> RandomPoints = new Queue<Vector3>();
    public static float MoveIdleTime = 5f;
}