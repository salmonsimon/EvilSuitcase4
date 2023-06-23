using System.Collections;
using UnityEngine;

public class FPSManager : MonoBehaviour
{
    private float count;

    private IEnumerator Start()
    {
        GUI.depth = 2;
        while (true)
        {
            count = 1f / Time.unscaledDeltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnGUI()
    {
        GUI.skin.label.fontSize = 50;
        GUI.Label(new Rect(40, 40, 300, 300), "FPS: " + Mathf.Round(count));
    }
}
