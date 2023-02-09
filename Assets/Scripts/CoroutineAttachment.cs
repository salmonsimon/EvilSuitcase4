using System.Collections;
using UnityEngine;

public class CoroutineAttachment : MonoBehaviour
{
    public IEnumerator attachedCoroutine;

    public void StartCoroutine()
    {
        StartCoroutine(attachedCoroutine);
    }

    public void StopCoroutine()
    {
        StopCoroutine(attachedCoroutine);
    }
}
