using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour
{
    #region Configuration

    [SerializeField] private LayerMask hitMask;

    [SerializeField] private Transform crossHairTarget;
    public Transform CrossHairTarget { get { return crossHairTarget; } }

    #endregion

    #region Variables

    private Ray crossHairRay;
    private RaycastHit crossHairRaycastHit;

    private bool isReloading;
    private float currentReloadingTime = -1f;

    private bool isShrinking;

    private Vector3 crosshairOriginalScale;

    #region Weapon Dependant

    private float reloadAnimationDuration;
    public float ReloadSpeed { get { return reloadAnimationDuration; } private set { reloadAnimationDuration = value; } }

    private float expandingValue = .3f;
    public float ExpandingValue { get { return expandingValue; } private set { expandingValue = value; } }

    private float shrinkSpeed = 1f;
    public float ShrinkSpeed { get { return shrinkSpeed; } private set { shrinkSpeed = value; } }

    private float crosshairMaxScale = 2f;
    public float CrosshairMaxScale { get { return crosshairMaxScale; } private set { crosshairMaxScale = value; } }

    #region CrossHair Images

    [SerializeField] private Image dot;
    public Image Dot { get { return dot; } private set { dot = value; } }

    [SerializeField] private Image inner;
    public Image Inner { get { return inner; } private set { inner = value; } }

    [SerializeField] private Image expanding;
    public Image Expanding { get { return expanding; } private set { expanding = value; } }

    [SerializeField] private Image reload;
    public Image Reload { get { return reload; } private set { reload = value; } }

    #endregion

    #endregion

    #endregion

    private ThirdPersonShooterController playerThirdPersonShooterController;

    private void Start()
    {
        reload.enabled = false;

        crosshairOriginalScale = expanding.rectTransform.localScale;

        playerThirdPersonShooterController = GameManager.instance.GetPlayer().GetComponent<ThirdPersonShooterController>();
    }

    private void Update()
    {
        crossHairRay.origin = Camera.main.transform.position;
        crossHairRay.direction = Camera.main.transform.forward;
        
        transform.position = crossHairRay.origin + crossHairRay.direction * 100;
    }

    private void LateUpdate()
    {
        float currentCrossHairTargetDepth = Vector3.Distance(crossHairRay.origin, crossHairTarget.position);

        if (Physics.Raycast(crossHairRay, out crossHairRaycastHit, 10000, hitMask))
            crossHairTarget.position = crossHairRaycastHit.point;
        else
            crossHairTarget.position = crossHairRay.origin + crossHairRay.direction * currentCrossHairTargetDepth;
    }

    public void SetupCrossHair(CrosshairConfigurationScriptableObject crosshairConfig)
    {
        dot.sprite = crosshairConfig.Dot;
        inner.sprite = crosshairConfig.Inner;
        expanding.sprite = crosshairConfig.Expanding;

        if (crosshairConfig.WeaponType == WeaponType.Gun)
        {
            reloadAnimationDuration = crosshairConfig.ReloadAnimationClip.length;

            if (crosshairConfig.HasExpandingImage)
            {
                crosshairMaxScale = crosshairConfig.CrosshairMaxScale;
                expandingValue = crosshairConfig.ExpandingValue;
                shrinkSpeed = crosshairConfig.ShrinkingSpeed;
            }
        }
        else
            reloadAnimationDuration = -1;

        expanding.rectTransform.localScale = crosshairOriginalScale;

        ShowReloadUI(false);
        ShowCrossHairUI(true);
        SetCrossHairUIColor(Color.white);
    }

    private void ShowReloadUI(bool value)
    {
        reload.enabled = value;
    }

    public void ShowCrossHairUI(bool value)
    {
        
        inner.enabled = inner.sprite ? value : false;
        dot.enabled = dot.sprite ? value : false;
        expanding.enabled = expanding.sprite ? value : false;
    }

    private void SetCrossHairUIColor(Color newColor)
    {
        inner.color = newColor;
        dot.color = newColor;
        expanding.color = newColor;
    }

    public void OutOfBullets()
    {
        SetCrossHairUIColor(Color.red);
    }

    public void ReloadWeapon()
    {
        if (!isReloading)
            StartCoroutine(ReloadCoroutine());
    }

    public void ExpandCrosshair()
    {
        if (expanding.rectTransform.localScale.x < crosshairMaxScale)
        {
            expanding.rectTransform.localScale += new Vector3(expandingValue, expandingValue, expandingValue);
        }
        else
            expanding.rectTransform.localScale = new Vector3(crosshairMaxScale, crosshairMaxScale, crosshairMaxScale);


        if (!isShrinking)
            StartCoroutine(ShrinkCrosshair());
    }

    private IEnumerator ShrinkCrosshair()
    {
        isShrinking = true; 

        while (crosshairOriginalScale.x < expanding.rectTransform.localScale.x)
        {
            expanding.rectTransform.localScale = new Vector3(expanding.rectTransform.localScale.x - Time.deltaTime * shrinkSpeed,
                                                             expanding.rectTransform.localScale.y - Time.deltaTime * shrinkSpeed,
                                                             expanding.rectTransform.localScale.z - Time.deltaTime * shrinkSpeed);

            yield return null;
        }

        expanding.rectTransform.localScale = crosshairOriginalScale;

        isShrinking = false;
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        reload.fillAmount = 0;
        currentReloadingTime = 0f;

        ShowReloadUI(true);
        ShowCrossHairUI(false);

        while (currentReloadingTime < reloadAnimationDuration)
        {
            currentReloadingTime += Time.deltaTime;

            reload.fillAmount =  currentReloadingTime / reloadAnimationDuration;

            yield return null;
        }

        reload.fillAmount = 1f;

        SetCrossHairUIColor(Color.white);

        ShowReloadUI(false);

        yield return null;

        isReloading = false;

        expanding.rectTransform.localScale = crosshairOriginalScale;

        yield return new WaitForEndOfFrame();
    }
}
