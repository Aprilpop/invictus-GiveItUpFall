using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinHolder : MonoBehaviour
{
    public float speed = 20f;

    void Update()
    {
            float rotX = speed * Mathf.Deg2Rad;

            transform.Rotate(Vector3.up, -rotX);        
    }

    private void OnEnable()
    {
        EventManager.StartListening("Win", Disable);
    }

    private void Disable()
    {
        if (ProfileManager.Instance.levelnumber % 5 != 0)
        {
            transform.parent.parent = null;
            transform.parent.gameObject.SetActive(false);
        }
    }


    private void OnDisable()
    {
        EventManager.StopListening("Win", Disable);
        
    }

}
