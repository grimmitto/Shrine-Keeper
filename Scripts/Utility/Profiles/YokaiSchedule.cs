using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NPC/Schedules/Yokai Schedule")]
public class YokaiSchedule : VisitorSchedule
{
    [Header("Yokai Route")]
    public List<TargetLocation> route = new List<TargetLocation>();

    public override IEnumerator RunSchedule(NPCMovementController npc)
    {
        if (route == null || route.Count == 0)
            yield break;

        foreach (var step in route)
        {
            if (step.location == null)
                continue;

            npc.MoveTo(step.location);
            yield return new WaitUntil(() => npc.IsIdle);

            float waitTime = step.GetRandomWaitTime();
            if (waitTime > 0)
                yield return new WaitForSeconds(waitTime);
        }
    }
}
