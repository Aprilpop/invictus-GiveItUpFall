using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTower : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    float touchSpeed;
    [SerializeField]
    float pressSpeed;

    [SerializeField]
    bool drag;

    public bool Drag { get { return drag; } set { drag = value; } }

    Vector3 _mouseReference;
    Vector3 _mouseOffset;
    private bool _isRotating;
    private Vector3 _rotation;



    private void Start()
    {
        _rotation = Vector3.zero;
        drag = ProfileManager.Instance.dragControl;
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
    }

    private void OnMouseDown()
    {
        _isRotating = true;
        _mouseReference = Input.mousePosition;

        if (ProfileManager.Instance.tutorialDrag && ProfileManager.Instance.dragControl)
            EventManager.TriggerEvent("HideDragTutorial");
        else if(ProfileManager.Instance.tutorialTouch && !ProfileManager.Instance.dragControl)
            EventManager.TriggerEvent("HideTouchTutorial");

    }

    private void OnMouseUp()
    {
        _isRotating = false;
    }

    Touch touchPos;

    private void Update()
    {
        if (GameLogic.Instance.CanRotate)
        {
#if UNITY_EDITOR
            if (_isRotating)
            {
                _mouseOffset = (Input.mousePosition - _mouseReference);

                _rotation.y = -(_mouseOffset.x + _mouseOffset.y) * speed;
                gameObject.transform.Rotate(_rotation);
                _mouseReference = Input.mousePosition;

                transform.eulerAngles += _rotation;

            }

#else
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector3 localAngle = transform.localEulerAngles;
                if (drag)
                {
                    if (ProfileManager.Instance.tutorialDrag && ProfileManager.Instance.dragControl)
                        EventManager.TriggerEvent("HideDragTutorial");
                   
                    localAngle.y -= touchSpeed * touch.deltaPosition.x;
                    transform.localEulerAngles = localAngle;
                }
                else
                {
                    if(ProfileManager.Instance.tutorialTouch && !ProfileManager.Instance.dragControl)
                        EventManager.TriggerEvent("HideTouchTutorial");
                    if (touch.position.x < Screen.width / 2)
                    {
                        Debug.Log("left");
                        localAngle.y -= pressSpeed * Time.deltaTime;
                        transform.localEulerAngles = localAngle;
                    }
                    else if (touch.position.x > Screen.width / 2)
                    {
                        Debug.Log("right");
                        localAngle.y += pressSpeed * Time.deltaTime;
                        transform.localEulerAngles = localAngle;
                    }
                }
            }
#endif
        }
    }
}
