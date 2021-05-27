using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum LevelType
{
    FirstLevel,
    SecondLevel,
    ThirdLevel,
    BonusLevel,
    RandomLevel
}

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;
    public static LevelManager Instance { get { return _instance; } }

    [SerializeField]
    GameObject startPlatform;

    [SerializeField]
    GameObject endPlatform;

    [SerializeField]
    GameObject[] spawnablePlatforms;

    [SerializeField]
    GameObject bonusPlatform;

    [SerializeField]
    int simplePlatformBorder;

    [SerializeField]
    int glassPlatformBorder;

    [SerializeField]
    int spikedPlatformBorder;

    [SerializeField]
    float platformYdifference;

    [SerializeField]
    GameObject BoostPickUp;
    
    int levelLength;

    public Transform finalpoint;

    GameObject checkpointPlatform;
 
    List<GameObject> generatedPlatforms = new List<GameObject>();

    string lastPlayedLevel = null;
    string[] stringToArray;
    List<string> lastLevel = new List<string>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

    void Start()
    {
        OnStart();
        
    }

    public void OnStart()
    {
        lastPlayedLevel = ProfileManager.Instance.lastPlayedLevel;
        levelLength = ProfileManager.Instance.levelLength;

        if (ProfileManager.Instance.levelnumber % 5 != 0 && lastPlayedLevel != "")
            LoadLastPlayedLevel();
        else
            LevelGenerator(ProfileManager.Instance.levelnumber);
    }

    public void ResetLevel()
    {
        DeleteCurrentLevel();
        generatedPlatforms.Clear();
        ProfileManager.Instance.boostOnLevel = 0;
        lastLevel.Clear();

        OnStart();
    }

    public void LevelGenerator(int level)
    {
        levelLength = ProfileManager.Instance.levelLength;
        
        if (level % 5 == 0)
        {
            GenerateBonusLevel();
        }
        else
        {
            switch (level)
            {
                case 1:
                    GenerateRandomLevel(simplePlatformBorder);
                    break;
                case 2:
                    GenerateRandomLevel(glassPlatformBorder);
                    break;
                case 3:
                    GenerateRandomLevel(spikedPlatformBorder);
                    break;
                default:
                    GenerateRandomLevel(spikedPlatformBorder);
                    break;
            }
        }
               
    }

    public void LoadLastPlayedLevel()
    {
        stringToArray = lastPlayedLevel.Split(";"[0]);

        Vector3 platformPosition = Vector3.zero;

        GameObject start = Instantiate(startPlatform, platformPosition, Quaternion.identity, transform);       

        generatedPlatforms.Add(start);
        platformPosition.y -= platformYdifference;
        int boostPosition = Random.Range(3, 15);
        for (int i = 0; i < stringToArray.Length; i++)
        {
            for (int j = 0; j < spawnablePlatforms.Length; j++)
            {
                if(stringToArray[i].Equals(spawnablePlatforms[j].name))
                {
                    GameObject platform = Instantiate(spawnablePlatforms[j], platformPosition, Quaternion.identity, transform);
                    generatedPlatforms.Add(platform);

                    if (i == boostPosition)
                    {
                        GameObject boost = Instantiate(BoostPickUp, new Vector3(platformPosition.x, platformPosition.y + 1, platformPosition.z), Quaternion.identity, platform.transform);
                    }

                    platformPosition.y -= platformYdifference; 
                }
            }
        }

        GameObject end = Instantiate(endPlatform, platformPosition, Quaternion.identity, transform);
        generatedPlatforms.Add(end);
        finalpoint = end.transform;
        ColorPaletteManager.Instance.SetEnvironmentColor(ProfileManager.Instance.levelColor);
    }

    public void GenerateBonusLevel()
    {

        DeleteCurrentLevel();
        generatedPlatforms.Clear();
        ProfileManager.Instance.boostOnLevel = 0;
        lastLevel.Clear();
        int yRot = 10;
        int rotationY = 10;
        Vector3 platformPosition = Vector3.zero;
        Quaternion rotation = Quaternion.identity;

        GameObject start = Instantiate(startPlatform, platformPosition, Quaternion.identity, transform);
        generatedPlatforms.Add(start);
        platformPosition.y -= platformYdifference;
        for (int i = 0; i < levelLength; i++)
        {
            rotationY = Random.Range(-30, 30);
            GameObject platform = Instantiate(bonusPlatform, platformPosition, rotation, transform);
            generatedPlatforms.Add(platform);
            rotation = Quaternion.Euler(0, yRot,0);
            yRot += rotationY;
            platformPosition.y -= platformYdifference;
            lastLevel.Add(bonusPlatform.name);
        }

        GameObject end = Instantiate(endPlatform, platformPosition, Quaternion.identity, transform);
        generatedPlatforms.Add(end);
        finalpoint = end.transform;

        lastPlayedLevel = string.Join(";", lastLevel);
        ProfileManager.Instance.lastPlayedLevel = lastPlayedLevel;
        ProfileManager.Instance.levelLength -= 2;
        ProfileManager.Instance.Save();
        ColorPaletteManager.Instance.SetEnvironmentColor();
    }

    public void GenerateRandomLevel(int platformCount)
    {

        DeleteCurrentLevel();
        generatedPlatforms.Clear();
        lastLevel.Clear();

        Vector3 platformPosition = Vector3.zero;

        GameObject start = Instantiate(startPlatform, platformPosition, Quaternion.identity, transform);
        generatedPlatforms.Add(start);
        platformPosition.y -= platformYdifference;

        int boostPosition = Random.Range(3,15);

        for (int i = 0; i < levelLength; i++)
        {
            int index = UnityEngine.Random.Range(0, platformCount);
            GameObject platform = Instantiate(spawnablePlatforms[index], platformPosition, Quaternion.identity, transform);
            generatedPlatforms.Add(platform);

            if(i == boostPosition)
            {
                GameObject boost = Instantiate(BoostPickUp, new Vector3(platformPosition.x, platformPosition.y + 1, platformPosition.z), Quaternion.identity, platform.transform);
            }

            platformPosition.y -= platformYdifference;
            //platformYdifference = Random.Range(1.8f, 2.5f);
            lastLevel.Add(spawnablePlatforms[index].name);
        }

        

        GameObject end = Instantiate(endPlatform, platformPosition, Quaternion.identity, transform);
        generatedPlatforms.Add(end);
        finalpoint = end.transform;

        lastPlayedLevel = string.Join(";", lastLevel);
        ProfileManager.Instance.lastPlayedLevel = lastPlayedLevel;
        if(ProfileManager.Instance.levelLength < 100)
            ProfileManager.Instance.levelLength++;
        ProfileManager.Instance.Save();
        ColorPaletteManager.Instance.SetEnvironmentColor();
        
    }


    public void GenerateRandomLevel()
    {
        DeleteCurrentLevel();
        generatedPlatforms.Clear();
        ProfileManager.Instance.boostOnLevel = 0;
        lastLevel.Clear();

        Vector3 platformPosition = Vector3.zero;

        GameObject start = Instantiate(startPlatform, platformPosition, Quaternion.identity, transform);
        generatedPlatforms.Add(start);
        platformPosition.y -= platformYdifference;

        for (int i = 0; i < levelLength; i++)
        {
            int index = UnityEngine.Random.Range(0,spawnablePlatforms.Length);
            GameObject platform = Instantiate(spawnablePlatforms[index], platformPosition, Quaternion.identity, transform);
            generatedPlatforms.Add(platform);            

            platformPosition.y -= platformYdifference;
            lastLevel.Add(spawnablePlatforms[index].name);
        }

        GameObject end = Instantiate(endPlatform, platformPosition, Quaternion.identity, transform);
        generatedPlatforms.Add(end);
        finalpoint = end.transform;

        lastPlayedLevel = string.Join(";", lastLevel);
        ProfileManager.Instance.lastPlayedLevel = lastPlayedLevel;
        ProfileManager.Instance.Save();
        ColorPaletteManager.Instance.SetEnvironmentColor();
    }

    public void DeleteCurrentLevel()
    {
        if (checkpointPlatform != null)
            Destroy(checkpointPlatform);
        int count = generatedPlatforms.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject item = generatedPlatforms[i];
            Destroy(item);            
        }
    }

    public void ReloadCurrentLevel()
    {
        if(checkpointPlatform != null)
            Destroy(checkpointPlatform);
        for (int i = 0; i < generatedPlatforms.Count; i++)
        {
            if(generatedPlatforms[i].GetComponent<RandomPlatforms>() != null)
                generatedPlatforms[i].GetComponent<RandomPlatforms>().ResetPlatforms();
        }
    }

    public void Checkpoint()
    {
        Vector3 checkpointPosition = new Vector3(Vector3.zero.x, Blob.Instance.platformPosition.y + platformYdifference, Vector3.zero.z);
        Vector3 blobPosition = new Vector3(Blob.Instance.transform.position.x, Blob.Instance.platformPosition.y + platformYdifference + 0.5f, Blob.Instance.transform.position.z);
        checkpointPlatform = Instantiate(startPlatform, checkpointPosition, Quaternion.identity, transform);
        Blob.Instance.SetPosition(blobPosition);
    }
}
