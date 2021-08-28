using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdownMotion : MonoBehaviour
{
    const float PI = 3.14f;

    [Range(0.00f, 20.00f)]
    public float span = 1.00f;

    [Range(2.0f, 300.0f)]
    public float change = 10.00f;

    int i = 0;
    float pi = -PI;

    // Update is called once per frame
    void Update()
    {
        if (i < 180)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + span * Mathf.Sin(pi), transform.localPosition.z);
            pi += PI / change;
            if (pi > PI)
            {
                pi = -PI;
            }
        }
        i++;
        i = i % 540;
    }
}