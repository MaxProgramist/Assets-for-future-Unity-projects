using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float lerpTime;

    Vector3 targetPosition;
    bool isFollowing = false;

    void Update()
    {
        if (!isFollowing) return;

        targetPosition = new Vector3 (target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpTime);
    }

    public void StartFollow()
    {
        isFollowing = true;
    }

    public void EndFollow()
    {
        isFollowing = false;
    }
}
