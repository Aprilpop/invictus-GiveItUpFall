using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour
{

    [SerializeField]
    float delay;

    void Start()
    {
        StartCoroutine(LoadNextScene(delay));
    }

    IEnumerator LoadNextScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Level");
    }
}
