using System.Collections;
using UnityEngine;
using MenuGUI;

[RequireComponent(typeof(Animator))]
public class AnimMenuActivator : MonoBehaviour, MenuActivator
{
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnActivate(ActivatorCallback callback) {

        gameObject.SetActive(true);
        animator.Play("Open");
        StartCoroutine(WaitForAnimFinish(callback));
    }

    public void OnDeactivate(ActivatorCallback callback) {

        animator.Play("Close");
        StartCoroutine(WaitForAnimFinish(callback));
    }

    IEnumerator WaitForAnimFinish(ActivatorCallback callback)
    {
        float length = animator.GetCurrentAnimatorClipInfo(0).Length;
        yield return new WaitForSecondsRealtime(length);
        callback?.Invoke();
    }


}