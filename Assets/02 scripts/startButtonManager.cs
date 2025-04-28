using UnityEngine;

public class StartButtonManager : MonoBehaviour
{
    public GameObject beginScherm;   // Canvas waar de knop in zit
    public GameObject uitlegPanel;   // Panel waar de uitlegtekst zit
    public GameObject startButton;   // De Start-knop zelf
    public Transform wereldPositie;  // De positie van de wereld
    public GameObject xrOrigin;      // XR Origin speler
    public TypewriterEffect typewriterEffect; // Verwijzing naar typewriter script

    public void StartWorld()
    {
        startButton.SetActive(false);     // Verberg de Start-knop
        beginScherm.SetActive(false);     // Verberg het hele Canvas (of achtergrond)
        uitlegPanel.SetActive(true);      // Toon de uitlegtekst
        typewriterEffect.BeginTypewriter(); // Start de typewriter-animatie
    }

    public void TeleporteerNaarWereld()
    {
        uitlegPanel.SetActive(false);          // Verberg uitlegtekst
        xrOrigin.transform.position = wereldPositie.position; // Verplaats speler
        xrOrigin.transform.rotation = wereldPositie.rotation;
    }
}