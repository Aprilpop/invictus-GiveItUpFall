using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Flags]
public enum BlobState
{
    Idle,
    Play,
    Win,
    Die,
    Restart,
    Checkpoint
}

public class Blob : MonoBehaviour
{
    private static Blob blob;
    public static Blob Instance { get { return blob; } }

    Rigidbody rigidbody;
    public float speed;
    public static float GlobalGravity = -9.8f;
    public float gravityScale = 1.0f;
    public float maxVelocity;
    Vector3 originalPosition;
    BlobState state = BlobState.Idle;

    [SerializeField]
    float boostTime = 2f;

    [SerializeField]
    GameObject deathParticle;

    [SerializeField]
    GameObject boostEffect;

    [SerializeField]
    SphereCollider boostCollider;

    public SphereCollider BoostCollider { get { return boostCollider; } }

    [SerializeField]
    ParticleSystem particle_splat;

    public bool isBoostActive = false;

    [HideInInspector]
    public Vector3 platformPosition;

    bool addJumpForce = false;

    [SerializeField]
    ParticleSystem ps;

    [SerializeField]
    Gradient ActiveBoostColor;

    [SerializeField]
    Gradient InactiveBoostColor;

    ParticleSystem.ColorOverLifetimeModule boostColor;

    public void SetBlobState(BlobState blobState)
    {
        state = blobState;
    }

    private void Awake()
    {
        blob = this;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        originalPosition = transform.position;
    }

    private void Start()
    {
        boostColor = ps.colorOverLifetime;
    }

    private void OnEnable()
    {
        EventManager.StartListening("Win", Win);
        EventManager.StartListening("Die", StartDieEffect);
        EventManager.StartListening("Boost", ActivateComboBoost);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Win", Win);
        EventManager.StopListening("Die", StartDieEffect);
        EventManager.StopListening("Boost", ActivateComboBoost);
    }
    

    public void ResetPosition()
    {
        deathParticle.SetActive(false);
        transform.position = originalPosition;
        Camera.main.GetComponentInParent<CameraFollow>().ResetCamera();
        DeactivateBoost();
    }

    public void SetPosition(Vector3 position)
    {
        deathParticle.SetActive(false);
        transform.position = position;
        Camera.main.GetComponentInParent<CameraFollow>().ResetCamera();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameLogic.Instance.Restart();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(SlowMotion(2f));
        }
    }

    IEnumerator SlowMotion(float duration)
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    private void FixedUpdate()
    {
        Vector3 gravity = GlobalGravity * gravityScale * Vector3.up;
        rigidbody.AddForce(gravity, ForceMode.Acceleration);
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxVelocity);
    }


    private void OnCollisionEnter(Collision collision)
    {      
        if (state == BlobState.Play)
        {
            ProfileManager.Instance.Combo = -1;

            if (collision.gameObject.CompareTag("Trap") && !isBoostActive)
            {
                if (Vibrate.vib)
                {
                    Handheld.Vibrate();
                    Debug.Log("振动");
                }
                platformPosition = collision.gameObject.transform.parent.transform.position;
                state |= BlobState.Die;               
                EventManager.TriggerEvent("Die");
            }
            else if (collision.gameObject.CompareTag("Win"))
            {
                state = BlobState.Win;
                    DeactivateBoost();
                EventManager.TriggerEvent("Win");
            }
            else
            {
                particle_splat.Play();
                //Handheld.Vibrate();
            }
        }


        if (state != BlobState.Die)
        {
            SoundManager.Play(SfxArrayEnum.Jump, transform.position);
            SoundManager.Play(SfxArrayEnum.Splash, transform.position);
            //particle_splat.Play();
            StartCoroutine(ScaleOverTime(0.1f));

            if (collision.impulse != Vector3.zero)
                rigidbody.AddForce(Vector3.up * speed, ForceMode.Impulse);

            if (isBoostActive)
            {
                isBoostActive = false;
                boostCollider.enabled = true;

                transform.localScale = new Vector3(0.3f, 0.3f, 1.0f);
            }
        }

    }

    public void Win()
    {
        SoundManager.Play(SfxArrayEnum.Win, transform.position);
        Debug.Log("timeScale:" + Time.timeScale);
    }

    public void StartDieEffect()
    {
        SoundManager.Play(SfxArrayEnum.Die, transform.position);
        GameLogic.Instance.CanRotate = false;
        deathParticle.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            if (ProfileManager.Instance.levelnumber % 5 == 0)
                other.transform.parent.gameObject.SetActive(false);
            else
            {
                other.transform.parent.gameObject.SetActive(false);
                other.transform.parent.parent = null;
            }
            SoundManager.Play(SfxArrayEnum.CoinPickUp, transform.position);           
            ProfileManager.Instance.Coin += ProfileManager.Instance.coinPickupAmount;
        }
        else if (other.gameObject.CompareTag("Boost"))
        {
            
            Destroy(other.transform.gameObject);
            ActivateBoost(boostTime);
        }
    }

    public void ActivateBoost(float time)
    {
        rigidbody.velocity = Vector3.zero;
        Vector3 gravity = GlobalGravity * gravityScale * Vector3.up;
        rigidbody.AddForce(gravity, ForceMode.Acceleration);
        isBoostActive = true;
        StartCoroutine(BoostScaleOverTime(0.5f));
        SoundManager.Play(SfxArrayEnum.Boost, transform.position);
        StartCoroutine(StartBoost(time));
    }

    private void ActivateComboBoost()
    {
        boostColor.color = InactiveBoostColor;
        boostEffect.SetActive(true);
        StartCoroutine(BoostScaleOverTime(0.5f));
        obj = SoundManager.Play(SfxArrayEnum.BoostEffect, transform.position, transform);
    }

    public void DeactivateBoost()
    {
        isBoostActive = false;
        boostEffect.SetActive(false);
        boostCollider.enabled = false;
        if(obj != null)
            obj.reset();
    }

    SoundObject obj;

    IEnumerator StartBoost(float time)
    {
        boostCollider.enabled = true;
        boostEffect.SetActive(true);
        boostColor.color = ActiveBoostColor;
        obj =  SoundManager.Play(SfxArrayEnum.BoostEffect, transform.position, transform);
        
        yield return new WaitForSeconds(time);
        boostColor.color = InactiveBoostColor;
        boostCollider.enabled = false;
    }

    IEnumerator ScaleOverTime(float time)
    {
        Vector3 originalScale = transform.localScale;
        Vector3 destinationScale = new Vector3(0.4f, 0.2f, 1.0f);
        bool down = true;
        float currentTime = 0.0f;
        
        if (down)
        {
            while (currentTime <= time)
            {
                transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
                currentTime += Time.deltaTime;
                yield return null;
            }
            down = false;
        }

        if(!down)
        {
            currentTime = 0.0f;
            while (currentTime <= time)
            {
                transform.localScale = Vector3.Lerp(destinationScale, new Vector3(0.3f, 0.3f, 1.0f), currentTime / time);
                currentTime += Time.deltaTime;
                yield return null;
            }
        }
        yield return null;

    }

    IEnumerator BoostScaleOverTime(float time)
    {
        Vector3 originalScale = transform.localScale;
        Vector3 destinationScale = new Vector3(0.25f, 0.35f, transform.localScale.z);
        
        float currentTime = 0.0f;

            while (currentTime <= time)
            {
                transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
                currentTime += Time.deltaTime;
                yield return null;
            }
        yield return null;

    }
}
    
