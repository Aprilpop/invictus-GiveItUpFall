using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float cameraOffsetY = 0.5f;

    private void Start()
    {
        target = GameLogic.Instance.Blob.transform;
    }

    void LateUpdate()
    {
        if(target.position.y - cameraOffsetY < transform.position.y)
        {
            transform.position = new Vector3(transform.position.x, target.position.y - cameraOffsetY, transform.position.z);
            //ColorPaletteManager.Instance.ShaderPropSet();
        }    
    }

    public void ResetCamera()
    {
        transform.position = new Vector3(transform.position.x, target.position.y - cameraOffsetY, transform.position.z);
    }
}
