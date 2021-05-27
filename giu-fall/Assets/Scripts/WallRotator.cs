using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRotator : MonoBehaviour
{

    public float speed = 20f;

    void Update()
    {
        float rotX = speed * Mathf.Deg2Rad;

        transform.Rotate(Vector3.up, -rotX);
    }
}
