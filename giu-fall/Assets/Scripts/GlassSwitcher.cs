using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassSwitcher : MonoBehaviour
{
    struct Glass
    {
        public Vector3 glassOriginalPos;
        public Quaternion glassOriginalRot;

        public Glass(Vector3 pos, Quaternion rot)
        {
            this.glassOriginalPos = pos;
            this.glassOriginalRot = rot;
        }
    }
    
    Glass glassSetup;

    [SerializeField]
    GameObject glass;

    [SerializeField]
    GameObject brokenGlass;

    public void BrokeGlass()
    {
        glass.SetActive(false);
        brokenGlass.SetActive(true);
        StartCoroutine(DestroyGlass(1f));
    }

    IEnumerator DestroyGlass(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(brokenGlass);
    }

    private void Start()
    {

        glassSetup = new Glass(brokenGlass.transform.position, brokenGlass.transform.rotation);
    }


    public void RestoreGlass()
    {
        glass.SetActive(true);
        GetComponent<BoxCollider>().enabled = true;
        if (brokenGlass == null)
        {
            brokenGlass = Instantiate(Resources.Load<GameObject>("platform_broken"), glassSetup.glassOriginalPos, glassSetup.glassOriginalRot, transform);
            brokenGlass.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SoundManager.Play(SfxArrayEnum.Glass, transform.position);
            BrokeGlass();
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
