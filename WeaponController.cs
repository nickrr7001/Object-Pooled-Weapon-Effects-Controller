using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WeaponController : MonoBehaviour
{
    public bool FullAuto;
    public float fireRate;
    public float reloadRate;
    private bool canFire = true;
    public Vector3 kickPos;
    public Vector3 kickRot;
    public int[] mags;
    private int currentMag = 0;
    public Transform firePoint;
    public GameObject muzzleFlash;
    private ObjectPool<GameObject> pool;
    private void Awake()
    {
        pool = new ObjectPool<GameObject>(
            () => { return Instantiate(muzzleFlash,firePoint); },
            obj => { obj.SetActive(true); },
            obj => { obj.SetActive(false); },
            obj => { Destroy(obj); },
            true,10,100
            );
    }
    public int getHighestMagIndex()
    {
        int retVal = 0;
        for (int i = 0; i < mags.Length; i++)
        {
            if (mags[retVal] < mags[i])
                retVal = i;
        }
        return retVal;
    }
    public int getLowestMagIndex()
    {
        int retVal = 0;
        for (int i = 0; i < mags.Length; i++)
        {
            if (mags[retVal] > mags[i])
                retVal = i;
        }
        return retVal;
    }
    private void Update()
    {
        bool triggerPulled = mags[currentMag] != 0 && canFire && (FullAuto && Input.GetMouseButton(0)) || (!FullAuto && Input.GetMouseButtonDown(0));
        if (triggerPulled)
        {
            StartCoroutine(shoot());
        }
        else if (canFire && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(reload());
        }
    }
    private IEnumerator reload()
    {
        GetComponent<Animator>().Play("Rig_Reload", 0);
        canFire = false;
        yield return new WaitForSeconds(reloadRate);
        canFire = true;
        currentMag = getHighestMagIndex();
    }
    private IEnumerator mfLifeTime()
    {
        var newObj = pool.Get();
        yield return new WaitForSeconds(3);
        pool.Release(newObj);
    }
    private IEnumerator shoot()
    {
        StartCoroutine(mfLifeTime());
        mags[currentMag]--;
        canFire = false;
        transform.localPosition += new Vector3(
            Random.Range(-kickPos.x, kickPos.x),
            Random.Range(-kickPos.y, kickPos.y),
            Random.Range(-kickPos.z, kickPos.z)
            );
        transform.localRotation = Quaternion.Euler(
            transform.localRotation.eulerAngles +
            new Vector3(
            Random.Range(-kickRot.x, kickRot.x),
            Random.Range(-kickRot.y, kickRot.y),
            Random.Range(-kickRot.z, kickRot.z)
            )
            );
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
}
