using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BreathingCircle : MonoBehaviour
{
    [Header("Instellingen")]
    public float sensitivity = 1000f; // tijdelijk hoog
    public float smoothSpeed = 5f;
    public Vector3 minScale = new Vector3(1f, 0.01f, 1f);
    public Vector3 maxScale = new Vector3(1.2f, 0.01f, 1.2f);

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

        // 🔁 Draai de loudness om: stil = 1, luid = 0
        float inverseLoudness = 1f - loudness;

        // Debug output
        string debugSamples = string.Join(", ", samples[..10]);
        Debug.Log("📊 Volume: " + loudness.ToString("F3") + " (omgekeerd: " + inverseLoudness.ToString("F3") + ") | Samples: [" + debugSamples + "]");

        Vector3 targetScale = Vector3.Lerp(minScale, maxScale, inverseLoudness);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothSpeed);

        Debug.Log("📏 Nieuwe schaal: " + transform.localScale.ToString("F3"));
    }
    
}