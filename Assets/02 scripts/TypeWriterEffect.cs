using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public float letterDelay = 0.05f; // Tijd tussen letters
    public string volledigeTekst;
    private TextMeshProUGUI tekstComponent;

    public StartButtonManager startButtonManager; // Om teleport te starten
    private bool isStarted = false;

    private void Awake()
    {
        tekstComponent = GetComponent<TextMeshProUGUI>();
        tekstComponent.text = "";
    }

    public void BeginTypewriter()
    {
        if (!isStarted)
        {
            StartCoroutine(SchrijfTekst());
            isStarted = true;
        }
    }

    IEnumerator SchrijfTekst()
    {
        foreach (char letter in volledigeTekst.ToCharArray())
        {
            tekstComponent.text += letter;
            yield return new WaitForSeconds(letterDelay);
        }

        yield return new WaitForSeconds(1f); // Even wachten na laatste letter
        startButtonManager.TeleporteerNaarWereld(); // Daarna teleporteren!
    }
}