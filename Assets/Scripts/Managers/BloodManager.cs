using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class BloodManager : MonoBehaviour
{
    [SerializeField] private bool infiniteDecal;

    private Light directionalLight;
    public Light DirectionalLight { get { return directionalLight; } set { directionalLight = value; } }

    [SerializeField] private GameObject bloodAttachment;

    [SerializeField] private GameObject[] bloodEffectPrefabs;

    [SerializeField] private GameObject bloodContainer;

    private Dictionary<string, ObjectPool<GameObject>> bloodPools = new Dictionary<string, ObjectPool<GameObject>>();

    private Vector3 worldUp = Vector3.up;

    private int defaultBloodToSpwan = 300;

    private void Awake()
    {
        foreach (GameObject bloodPrefab in bloodEffectPrefabs)
        {
            if (!bloodPools.ContainsKey(bloodPrefab.name))
            {
                bloodPools.Add(bloodPrefab.name, new ObjectPool<GameObject>(() => Instantiate(bloodPrefab, bloodContainer.transform)));
            }

            for (int i = 0; i < defaultBloodToSpwan; i++)
            {
                GameObject spawnedBlood = bloodPools[bloodPrefab.name].Get();

                spawnedBlood.AddComponent<PoolableObject>();
                spawnedBlood.GetComponent<PoolableObject>().ObjectPool = bloodPools[bloodPrefab.name];

                spawnedBlood.gameObject.SetActive(false);
            }
        }

        BloodInitializationPoolRelease();
    }

    private void OnEnable()
    {
        GameObject light = GameObject.FindGameObjectWithTag(Config.DIRECTIONAL_LIGHT_TAG);

        if (light)
            directionalLight = light.GetComponent<Light>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        directionalLight = null;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject light = GameObject.FindGameObjectWithTag(Config.DIRECTIONAL_LIGHT_TAG);

        if (light)
            directionalLight = light.GetComponent<Light>();
    }


    //private int effectIdx = 0;

    public void SpawnBloodOnHit(RaycastHit hit)
    {
        float angle = Mathf.Atan2(hit.normal.x, hit.normal.z) * Mathf.Rad2Deg + 180;

        var effectIdx = Random.Range(0, bloodEffectPrefabs.Length);

        //if (effectIdx == bloodEffectPrefabs.Length) effectIdx = 0;

        if (!bloodPools.ContainsKey(bloodEffectPrefabs[effectIdx].name))
        {
            bloodPools.Add(bloodEffectPrefabs[effectIdx].name, new ObjectPool<GameObject>(() => Instantiate(bloodEffectPrefabs[effectIdx], bloodContainer.transform)));
        }

        GameObject spawnedBlood = bloodPools[bloodEffectPrefabs[effectIdx].name].Get();

        spawnedBlood.transform.position = hit.point;
        spawnedBlood.transform.rotation = Quaternion.Euler(0, angle + 90, 0);

        if (!spawnedBlood.activeSelf)
        {
            spawnedBlood.SetActive(true);
        }
        else
        {
            spawnedBlood.AddComponent<PoolableObject>();
            spawnedBlood.GetComponent<PoolableObject>().ObjectPool = bloodPools[bloodEffectPrefabs[effectIdx].name];
        }

        //effectIdx++;

        var settings = spawnedBlood.GetComponent<BFX_BloodSettings>();
        settings.FreezeDecalDisappearance = infiniteDecal;
        settings.LightIntensityMultiplier = directionalLight.intensity;

        var nearestBone = GetNearestObject(hit.transform.root, hit.point);
        if (nearestBone != null)
        {
            var attachBloodInstance = Instantiate(bloodAttachment);

            var bloodT = attachBloodInstance.transform;
            bloodT.position = hit.point;
            bloodT.localRotation = Quaternion.identity;
            bloodT.localScale = Vector3.one * Random.Range(0.75f, 1.2f);
            bloodT.LookAt(hit.point + hit.normal, worldUp);
            bloodT.Rotate(90, 0, 0);
            bloodT.transform.parent = nearestBone;
        }
    }

    public void SpawnBloodOnHit(Transform hitObject, Vector3 hitPosition, Vector3 hitNormal)
    {
        float angle = Mathf.Atan2(hitNormal.x, hitNormal.z) * Mathf.Rad2Deg + 180;

        var effectIdx = Random.Range(0, bloodEffectPrefabs.Length);

        if (!bloodPools.ContainsKey(bloodEffectPrefabs[effectIdx].name))
        {
            bloodPools.Add(bloodEffectPrefabs[effectIdx].name, new ObjectPool<GameObject>(() => Instantiate(bloodEffectPrefabs[effectIdx], bloodContainer.transform)));
        }

        GameObject spawnedBlood = bloodPools[bloodEffectPrefabs[effectIdx].name].Get();

        spawnedBlood.transform.position = hitPosition;
        spawnedBlood.transform.rotation = Quaternion.Euler(0, angle + 90, 0);

        if (!spawnedBlood.activeSelf)
        {
            spawnedBlood.SetActive(true);
        }
        else
        {
            spawnedBlood.AddComponent<PoolableObject>();
            spawnedBlood.GetComponent<PoolableObject>().ObjectPool = bloodPools[bloodEffectPrefabs[effectIdx].name];
        }

        var settings = spawnedBlood.GetComponent<BFX_BloodSettings>();
        settings.FreezeDecalDisappearance = infiniteDecal;
        settings.LightIntensityMultiplier = directionalLight.intensity;

        // FOR NOW NOT USING THIS PART, SINCE WE CANT ADD DECALS TO SKINNED MESHES PROPERLY
        /*
        var nearestBone = GetNearestObject(hitObject.transform.root, hitPosition);
        if (nearestBone != null)
        {
            var attachBloodInstance = Instantiate(bloodAttachment);

            var bloodT = attachBloodInstance.transform;
            bloodT.position = hitPosition;
            bloodT.localRotation = Quaternion.identity;
            bloodT.localScale = Vector3.one * Random.Range(0.75f, 1.2f);
            bloodT.LookAt(hitPosition + hitNormal, worldUp);
            bloodT.Rotate(90, 0, 0);
            bloodT.transform.parent = nearestBone;
        }
        */
    }

    Transform GetNearestObject(Transform hitObject, Vector3 hitPos)
    {
        var closestPos = 100f;
        Transform closestSkinnedMesh = null;
        var skinnedMeshRenderers = hitObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            var dist = Vector3.Distance(skinnedMeshRenderer.transform.position, hitPos);
            if (dist < closestPos)
            {
                closestPos = dist;
                closestSkinnedMesh = skinnedMeshRenderer.transform;
            }
        }

        var distRoot = Vector3.Distance(hitObject.position, hitPos);
        if (distRoot < closestPos)
        {
            closestPos = distRoot;
            closestSkinnedMesh = hitObject;
        }
        return closestSkinnedMesh;
    }

    public void BloodCleanup()
    {
        foreach (Transform blood in bloodContainer.transform)
        {
            ObjectPool<GameObject> pool = blood.GetComponent<PoolableObject>().ObjectPool;

            if (blood.gameObject.activeSelf)
            {
                blood.gameObject.SetActive(false);

                pool.Release(blood.gameObject);
            }
        }
    }

    public void BloodInitializationPoolRelease()
    {
        foreach (Transform blood in bloodContainer.transform)
        {
            ObjectPool<GameObject> pool = blood.GetComponent<PoolableObject>().ObjectPool;
            pool.Release(blood.gameObject);
        }
    }
}
