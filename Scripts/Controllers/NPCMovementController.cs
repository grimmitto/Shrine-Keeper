using UnityEngine;
using System;
using System.Collections;


[RequireComponent(typeof(CharacterController))]
public class NPCMovementController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float stoppingDistance = 1f;

    private CharacterController controller;
    private Coroutine moveRoutine;

    public bool IsIdle;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void MoveTo(Transform target)
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            IsIdle = true;
        }
        IsIdle = false;
        moveRoutine = StartCoroutine(MoveToRoutine(target));
    }

    private IEnumerator MoveToRoutine(Transform target)
    {
        while (target != null)
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0;
            float dist = dir.magnitude;

            if (dist <= stoppingDistance)
                break;

            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);

            controller.Move(transform.forward * moveSpeed * Time.deltaTime);
            controller.Move(Vector3.down * 9.81f * Time.deltaTime);

            yield return null; // wait one frame
        }

        // reached destination, hand control back to whatever AI script
        moveRoutine = null;
        OnArrived();
    }

    private void OnArrived()
    {
        // Optional: you can hook this to your AI script
        // or send an event to trigger the next behavior
    }
}
