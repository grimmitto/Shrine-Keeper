using System.Collections;
using UnityEngine;

public abstract class VisitorSchedule : ScriptableObject
{
    public abstract IEnumerator RunSchedule(NPCMovementController npc);
}
