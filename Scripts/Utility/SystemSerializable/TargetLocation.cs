using System;
using UnityEngine;

[Serializable]
public class TargetLocation
{
    [Tooltip("Where the NPC should move to.")]
    public Transform location;

    [Tooltip("How long the NPC should wait at this location (seconds).")]
    public Vector2 waitTimeRange = new Vector2(0f, 0f);

    public float GetRandomWaitTime()
    {
        return UnityEngine.Random.Range(waitTimeRange.x, waitTimeRange.y);
    }
}