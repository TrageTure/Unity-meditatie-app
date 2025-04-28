using UnityEngine;

public class BreathingAnimation : MonoBehaviour
{
    public float minScale = 0.8f; // Min grootte
    public float maxScale = 1.2f; // Max grootte
    public float speed = 1.0f;    // Snelheid ademhaling

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
        transform.localScale = originalScale * scale;
    }
}