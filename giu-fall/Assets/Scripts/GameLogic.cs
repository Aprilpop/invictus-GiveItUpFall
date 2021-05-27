using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum GameLogicState
{
    Play,
    Idle
}

public class GameLogic : MonoBehaviour
{
    static GameLogic _instance;
    public static GameLogic Instance { get { return _instance; } }

    [SerializeField]
    float waitAfterWin;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private Blob blob;
    public Blob Blob { get { return blob; } set { blob = value; } }

    private bool canRotate;
    public bool CanRotate { get { return canRotate; } set { canRotate = value; } }

    void Start()
    {
        GameObject player = Instantiate(Resources.Load<GameObject>("Blob"));
        blob = player.GetComponent<Blob>();
        ProfileManager.Instance.SetCharacter(ProfileManager.Instance.currentBlob);
        ProfileManager.Instance.SetTrail(ProfileManager.Instance.currentTrailName);
        CanRotate = false;
    }

    public void OnWin()
    {
        CanRotate = false;
        ProfileManager.Instance.levelnumber += 1;
        NextLevel();
    }

    public void NextLevel()
    {
        LevelManager.Instance.LevelGenerator(ProfileManager.Instance.levelnumber);
        blob.ResetPosition();
        CanRotate = true;
        Blob.Instance.SetBlobState(BlobState.Play);
        EventManager.TriggerEvent("NextLevel");
        ProfileManager.Instance.rewardDoubleWatched = false;
        Time.timeScale = 1f;
        //Debug.LogError("=========>>>NextLevel");
        //EventDispatcher.Instance.RemoveEventListener(EventKey.AdShowSuccessCallBack, OnNextLevelVideoSuccess);
    }

    public void Restart()
    {
        ProfileManager.Instance.Score = 0;
        LevelManager.Instance.ReloadCurrentLevel();
        blob.ResetPosition();
        CanRotate = true;
        Blob.Instance.SetBlobState(BlobState.Play);
        Time.timeScale = 1f;
    }

    public void BackToMain()
    {
        ProfileManager.Instance.Score = 0;
        Blob.Instance.SetBlobState(BlobState.Idle);
        LevelManager.Instance.ReloadCurrentLevel();
        blob.ResetPosition();
        CanRotate = false;
        Time.timeScale = 1f;
        // MenuGUI.MenuManager.Instance.Back();
    }


    public void StartFromCheckpoint()
    {
        Blob.Instance.SetBlobState(BlobState.Play);
        LevelManager.Instance.Checkpoint();
        CanRotate = true;
        Time.timeScale = 1f;
    }
}
