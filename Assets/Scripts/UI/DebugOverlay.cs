using UnityEngine;
using TMPro;


public class DebugOverlay : MonoBehaviour
{
    [Tooltip("Debug Text")]
    public TextMeshProUGUI debugText;

    private float pollingTime = .1f; // How often to update the display
    private float time;
    private int frameCount;

    void Update()
    {
        time += Time.unscaledDeltaTime;
        frameCount++;

        if (time >= pollingTime)
        {
            // Calculate FPS
            int frameRate = Mathf.RoundToInt(frameCount / time);

            // Count active resources using their Tag
            int resourceCount = GameObject.FindGameObjectsWithTag("Resource").Length;

            debugText.text = $"FPS: {frameRate}\nItems: {resourceCount}";

            time -= pollingTime;
            frameCount = 0;
        }
    }
}