using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BloodManager : MonoBehaviour
{
    [SerializeField] private bool infiniteDecal;
    //public bool InfiniteDecal { get { return infiniteDecal; } }

    private Light directionalLight;
    public Light DirectionalLight { get { return directionalLight; } set { directionalLight = value; } }

    [SerializeField] private GameObject bloodAttachment;

    [SerializeField] private GameObject[] bloodEffectPrefabs;

    private GameObject bloodContainer;

    private Vector3 worldUp = Vector3.up;

    private void OnEnable()
    {
        bloodContainer = new GameObject("Blood Container");

        GameObject light = GameObject.FindGameObjectWithTag(Config.DIRECTIONAL_LIGHT_TAG);

        if (light)
            directionalLight = light.GetComponent<Light>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        Destroy(bloodContainer.gameObject);

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

        var bloodInstance = Instantiate(bloodEffectPrefabs[effectIdx], hit.point, Quaternion.Euler(0, angle + 90, 0), bloodContainer.transform);

        //effectIdx++;

        var settings = bloodInstance.GetComponent<BFX_BloodSettings>();
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

        var bloodInstance = Instantiate(bloodEffectPrefabs[effectIdx], hitPosition, Quaternion.Euler(0, angle + 90, 0), bloodContainer.transform);

        var settings = bloodInstance.GetComponent<BFX_BloodSettings>();
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
}
