using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [Range(0.0f, 100.0f)]
    public float rotationSpeed = 10.0f; 
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}