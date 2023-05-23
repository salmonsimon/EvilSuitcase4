using System.Collections;
using UnityEngine;

public class CoroutineAttachment : MonoBehaviour
{
    public IEnumerator attachedCoroutine;

    public void StartCoroutine()
    {
        if (transform.parent.gameObject.activeSelf)
            StartCoroutine(attachedCoroutine);
    }

    public void StopCoroutine()
    {
        if (transform.parent.gameObject.activeSelf)
            StopCoroutine(attachedCoroutine);
    }
}
