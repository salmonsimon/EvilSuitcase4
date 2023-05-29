using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour
{
    #region Configuration

    [SerializeField] private LayerMask hitMask;

    #endregion

    #region Variables

    private Ray crossHairRay;
    private RaycastHit crossHairRaycastHit;

    private bool isReloading;
    private bool isShrinking;

    private Vector3 crosshairOriginalScale;

    #region Weapon Dependant

    private float reloadSpeed;
    public float ReloadSpeed { get { return reloadSpeed; } private set { reloadSpeed = value; } }

    private float expandingValue = .5f;
    public float ExpandingValue { get { return expandingValue; } private set { expandingValue = value; } }

    private float shrinkSpeed = 2f;
    public float ShrinkSpeed { get { return shrinkSpeed; } private set { shrinkSpeed = value; } }

    private float crosshairMaxScale;
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

    private void Start()
    {
        reload.enabled = false;

        crosshairOriginalScale = expanding.rectTransform.localScale;
    }

    private void Update()
    {
        crossHairRay.origin = Camera.main.transform.position;
        crossHairRay.direction = Camera.main.transform.forward;

        if (Physics.Raycast(crossHairRay, out crossHairRaycastHit, 10000, hitMask))
            transform.position = crossHairRaycastHit.point;
        else
            transform.position = crossHairRay.origin + crossHairRay.direction.normalized * 100;
    }

    public void SetupCrossHair(CrosshairConfigurationScriptableObject crosshairConfig)
    {
        dot.sprite = crosshairConfig.Dot;
        inner.sprite = crosshairConfig.Inner;
        expanding.sprite = crosshairConfig.Expanding;

        if (crosshairConfig.WeaponType == WeaponType.Gun)
            reloadSpeed = crosshairConfig.ReloadAnimationClip.length;
        else
            reloadSpeed = -1;

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

        do
        {
            expanding.rectTransform.localScale = new Vector3(expanding.rectTransform.localScale.x - Time.deltaTime * shrinkSpeed,
                                                             expanding.rectTransform.localScale.y - Time.deltaTime * shrinkSpeed,
                                                             expanding.rectTransform.localScale.z - Time.deltaTime * shrinkSpeed);
            yield return new WaitForEndOfFrame();
        }
        while (crosshairOriginalScale.x < expanding.rectTransform.localScale.x);

        isShrinking = false;
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        reload.fillAmount = 0;

        ShowReloadUI(true);
        ShowCrossHairUI(false);

        do
        {
            reload.fillAmount += Time.deltaTime * reloadSpeed;
            yield return new WaitForEndOfFrame();
        }
        while (reload.fillAmount < 1f);

        SetCrossHairUIColor(Color.white);

        ShowReloadUI(false);
        ShowCrossHairUI(true);

        isReloading = false;

        expanding.rectTransform.localScale = crosshairOriginalScale;

        yield return new WaitForEndOfFrame();
    }
}
