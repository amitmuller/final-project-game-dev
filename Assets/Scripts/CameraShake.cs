using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    void Start()
    {
        Instance = this;
    }

    public IEnumerator camShake(float time, float magnitude)
    {
        Vector3 ogPositionCamera = transform.localPosition;

        float shakeTime = 0.0f;

        while(shakeTime < time)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, ogPositionCamera.z);

            shakeTime += Time.deltaTime;

            yield return null;

        }

        transform.localPosition = ogPositionCamera;

    }
}
