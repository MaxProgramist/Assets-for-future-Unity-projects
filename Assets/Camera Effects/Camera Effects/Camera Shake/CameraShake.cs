using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float magnetude;
    [SerializeField] private float tileBtwNextStep;

    Vector3 startPosition;
    bool isShaking = false;

    public void StartShake(float duration)
    {
        if (isShaking) return;

        startPosition = transform.position;
        StartCoroutine(Shake(duration));
    }

    IEnumerator Shake(float duration)
    {
        isShaking = true;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            Vector3 offsetFromStart = new Vector3(Random.Range(-magnetude, magnetude), Random.Range(-magnetude, magnetude));
            transform.position = startPosition + offsetFromStart;

            yield return new WaitForSeconds(tileBtwNextStep);
            timeElapsed += tileBtwNextStep;
        }

        transform.position = startPosition;
        isShaking = false;
    }
}
