using System.Collections;
using System.Collections.Generic;
using UnityEngine;



struct Platform
{
    public Transform transform;
    public Vector3 originalPosition;

    public Platform(Transform transform, Vector3 position)
    {
        this.transform = transform;
        this.originalPosition = position;
    }
}



public class RandomPlatforms : MonoBehaviour
{
    List<Platform> platforms = new List<Platform>();

    List<Transform> simplePlatforms = new List<Transform>();

    GameObject coin;

    public Transform GetRandomPlatform()
    {
        int index = Random.Range(0, simplePlatforms.Count);
        
        return simplePlatforms[index].transform;
    }

    public float speed = 20;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Platform platform = new Platform(transform.GetChild(i), transform.GetChild(i).position);
            platforms.Add(platform);

            if(ProfileManager.Instance.levelnumber % 5 != 0)
            {
                if (!transform.GetChild(i).CompareTag("Trap"))
                {
                    simplePlatforms.Add(transform.GetChild(i).transform);
                }
            }           
        }

        if (simplePlatforms.Count > 0 && transform.position != Vector3.zero)
        {
            Transform randomPlatform = GetRandomPlatform();
            coin = ObjectPooler.Instance.SpawnFromPool("Coin", randomPlatform.position, Quaternion.identity, randomPlatform);
            simplePlatforms.Clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            SoundManager.Play(SfxArrayEnum.PlatformBreak, transform.position);
            GetComponent<BoxCollider>().enabled = false;
            ProfileManager.Instance.Combo += 1;
            ProfileManager.Instance.Score += ProfileManager.Instance.levelnumber + ProfileManager.Instance.Combo;


                if (ProfileManager.Instance.Combo >= 3 && !Blob.Instance.isBoostActive)
                {
                    Blob.Instance.isBoostActive = true;
                    EventManager.TriggerEvent("Boost");
            }
            

            move = true;

            if (Blob.Instance.BoostCollider.enabled && !Blob.Instance.isBoostActive)
            {
                Blob.Instance.DeactivateBoost();
            }
            
        }
    }
    
    bool move = false;
    float t = 0;
    
    private void Update()
    {
        if (move)
        {
            Vector3 origo = transform.position;
            for (int i = 0; i < platforms.Count; i++)
            {
                Transform child = platforms[i].transform;
                //if (child.GetComponent<MeshRenderer>() != null && child.GetComponent<MeshRenderer>().isVisible)
                //{
                    Vector3 pos = child.position;
                    Vector3 dir = (pos - origo).normalized * Time.unscaledDeltaTime * speed; dir.y += Time.unscaledDeltaTime;
                    child.position = pos + dir;
               //}
            }
        }
    }

    private void OnEnable()
    {
        move = false;
        for (int i = 0; i < platforms.Count; i++)
        {
            platforms[i].transform.position = platforms[i].originalPosition;
        }
        GetComponent<BoxCollider>().enabled = true;
    }
    
    public void ResetPlatforms()
    {
        move = false;
        GetComponent<BoxCollider>().enabled = true;
        for (int i = 0; i < platforms.Count; i++)
        {
            platforms[i].transform.position = platforms[i].originalPosition;
            if (platforms[i].transform.GetComponent<GlassSwitcher>())
                platforms[i].transform.GetComponent<GlassSwitcher>().RestoreGlass();

            if (platforms[i].transform.GetComponent<MovingPlatform>())
                platforms[i].transform.GetComponent<MovingPlatform>().ResetMovement();
        }
    }

    private void OnDisable()
    {

    }
}
