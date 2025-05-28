using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BreathingCircle : MonoBehaviour
{
    [Header("Instellingen")]
    public float sensitivity = 1000f;
    public float smoothSpeed = 5f;
    public Vector3 minScale = new Vector3(1f, 0.01f, 1f);
    public Vector3 maxScale = new Vector3(1.2f, 0.01f, 1.2f);

    [Header("Feedback Instellingen")]
    public Renderer breathingRenderer; // Renderer ipv UI Image!
    public Color goodColor = Color.green;
    public Color badColor = Color.red;
    public float tolerance = 0.05f; // Hoe nauwkeurig je moet volgen

    [Header("Ademhalingsritme")]
    public BreathingAnimation breathingAnimation;

    [Header("Visuele Correctie")]
    public float scaleOffset = 0.05f; // ➔ Hoeveel groter de ademcirkel visueel moet lijken

    private AudioSource audioSource;
    private float[] samples = new float[256];
    private bool micReady = false;

    void Start()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("🚫 Geen microfoon gevonden!");
            return;
        }

        Debug.Log("🎤 Geselecteerde microfoon: " + Microphone.devices[0]);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, 1, 44100);
        audioSource.loop = true;
        audioSource.mute = true;

        while (!(Microphone.GetPosition(null) > 0)) { }
        audioSource.Play();
        micReady = true;

        Debug.Log("🎉 Microfoon gestart en AudioSource speelt af!");
    }

    void Update()
    {
        if (!micReady) return;

        int micPos = Microphone.GetPosition(null) - samples.Length;
        if (micPos < 0) return;

        audioSource.clip.GetData(samples, micPos);

        float sum = 0f;
        foreach (var sample in samples)
        {
            sum += Mathf.Abs(sample);
        }

        float loudness = sum / samples.Length * sensitivity;
        loudness = Mathf.Clamp01(loudness);

        // 🔥 Debug volume microfoon
        // Debug.Log($"📣 Microfoon loudness: {loudness:F3}");

        float inverseLoudness = 1f - loudness;

        Vector3 targetScale = Vector3.Lerp(minScale, maxScale, inverseLoudness);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothSpeed);

        // 🌟 Correcte schaalvergelijking mét offset
        float zonDoel = Mathf.Lerp(breathingAnimation.minScale, breathingAnimation.maxScale, (Mathf.Sin(Time.time * breathingAnimation.speed) + 1f) / 2f);
        float ademSchaal = Mathf.Lerp(breathingAnimation.minScale, breathingAnimation.maxScale, inverseLoudness);

        // ➡️ Visueel corrigeren
        ademSchaal += scaleOffset;

        float verschil = ademSchaal - zonDoel;

        float mappedDifference = 0f;
        if (verschil <= 0f)
        {
            // Ademcirkel kleiner of gelijk aan zon -> smooth verschil
            mappedDifference = Mathf.Clamp01(Mathf.Abs(verschil) / tolerance);
        }
        else
        {
            // Ademcirkel groter dan zon -> direct maximaal rood
            mappedDifference = 1f;
        }

        if (breathingRenderer != null)
        {
            // 🌈 Smooth kleur blending
            Color targetColor = Color.Lerp(goodColor, badColor, mappedDifference);
            breathingRenderer.material.color = Color.Lerp(breathingRenderer.material.color, targetColor, Time.deltaTime * 5f);

            // ✨ Smooth emissie blending
            Color targetEmission = Color.Lerp(goodColor * 2f, badColor * 0.5f, mappedDifference);
            breathingRenderer.material.SetColor("_EmissionColor", Color.Lerp(breathingRenderer.material.GetColor("_EmissionColor"), targetEmission, Time.deltaTime * 5f));
        }
    }
}