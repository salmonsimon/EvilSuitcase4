using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuitcaseTransitionUI : MonoBehaviour {

    [SerializeField] private Material material;

    private float maskAmount = 0f;
    private float targetValue = -.1f;

    private void Update() {

        float maskAmountChange = targetValue > maskAmount ? +.1f : -.1f;
        maskAmount += maskAmountChange * Time.deltaTime * 6f;
        maskAmount = Mathf.Clamp01(maskAmount);

        material.SetFloat("_MaskAmount", maskAmount);
    }

    public void StartTransition()
    {
        targetValue = 1f;
    }

    public void EndTransition()
    {
        targetValue = -.1f;
    }
}