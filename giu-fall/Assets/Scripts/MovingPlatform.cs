using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Sideway move")]
    [SerializeField]
    bool sideWaysMove = true;
    public float speed = 10f;
    public float minAngle;
    public float maxAngle;
    float yRot;
    bool inverse = false;

    [Header("Flip move")]
    [SerializeField]
    bool aroundMove = false;
    public float speed2 = 100f;
    float xRot;
    public float minAngle2;
    public float maxAngle2;
    bool inverse2 = false;
    public float delay = 0;

    void Start()
    {
       
    }

    private void OnEnable()
    {
        ResetMovement();
        speed /= 3;
    }

    public void ResetMovement()
    {
        inverse = false;
        yRot = 0;
        xRot = 0;
    }

    float time = 0;

    void Update()
    {
        if (sideWaysMove)
        {
            if (yRot > maxAngle)
                inverse = true;
            else if (yRot < minAngle)
                inverse = false;

            if (inverse)
            {
                transform.RotateAround(Vector3.zero, Vector3.up, speed * Time.deltaTime);
                yRot -= speed * Time.deltaTime;
            }
            else
            {
                transform.RotateAround(Vector3.zero, -Vector3.up, speed * Time.deltaTime);
                yRot += speed * Time.deltaTime;
            }
        }

        if (aroundMove)
        {
            if (xRot > maxAngle2)
            {
                inverse2 = true;
                xRot = maxAngle2;
                time = delay;
            }
            else if (xRot < minAngle2)
            {
                xRot = minAngle2;
                inverse2 = false;
                time = delay;
            }

            if (time <= 0)
            {
                if (inverse2)
                {
                    transform.RotateAround(transform.position, transform.right, speed2 * Time.deltaTime);
                    xRot -= speed2 * Time.deltaTime;
                }
                else
                {
                    transform.RotateAround(transform.position, -transform.right, speed2 * Time.deltaTime);
                    xRot += speed2 * Time.deltaTime;
                }
            }
            time -= Time.deltaTime; 
        }
    }
}
