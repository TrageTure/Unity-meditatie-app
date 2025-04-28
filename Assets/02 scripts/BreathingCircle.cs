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
    public Renderer breathingRenderer; // NU Renderer ipv UI Image!
    public Color goodColor = Color.green;
    public Color badColor = Color.red;
    public float tolerance = 0.1f;

    [Header("Ademhalingsritme")]
    public BreathingAnimation breathingAnimation;

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

        // 🔥 EXTRA DEBUG: toon puur hoeveel geluid er gemeten wordt
        Debug.Log($"📣 Microfoon loudness: {loudness:F3}");

        float inverseLoudness = 1f - loudness;

        Vector3 targetScale = Vector3.Lerp(minScale, maxScale, inverseLoudness);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothSpeed);

        // 🌟 Vergelijk inverseLoudness met ademhalingsritme
        float zonDoel = Mathf.Lerp(breathingAnimation.minScale, breathingAnimation.maxScale, (Mathf.Sin(Time.time * breathingAnimation.speed) + 1f) / 2f);
        float verschil = Mathf.Abs(inverseLoudness - (zonDoel - breathingAnimation.minScale) / (breathingAnimation.maxScale - breathingAnimation.minScale));

        // 🌈 Verander kleur én emissie van de cirkel op basis van het verschil
        if (breathingRenderer != null)
        {
            Color targetColor = (verschil < tolerance) ? goodColor : badColor;
            breathingRenderer.material.color = Color.Lerp(breathingRenderer.material.color, targetColor, Time.deltaTime * 5f);

            // ✨ Emission aanpassen
            Color emissionColor = (verschil < tolerance) ? goodColor * 2f : badColor * 0.5f;
            breathingRenderer.material.SetColor("_EmissionColor", Color.Lerp(breathingRenderer.material.GetColor("_EmissionColor"), emissionColor, Time.deltaTime * 5f));
        }
    }
}