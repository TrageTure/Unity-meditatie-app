using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class PlayBowlAnimationOnSelect : MonoBehaviour
{
    public AudioSource bowlSound;
    private Animator animator;
    private bool isPlaying = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        var interactable = GetComponent<XRSimpleInteractable>();
        interactable.activated.AddListener(OnActivate);

        if (animator != null)
        {
            animator.SetBool("Play", false); // Zorg dat animatie niet vanzelf start
        }
    }

    public void OnActivate(ActivateEventArgs args)
    {
        isPlaying = !isPlaying;
        Debug.Log("Bowl toggled: " + (isPlaying ? "Play" : "Pause"));

        if (animator != null)
        {
            animator.SetBool("Play", isPlaying);
            animator.speed = isPlaying ? 1f : 0f;
        }

        if (bowlSound != null)
        {
            if (isPlaying && !bowlSound.isPlaying)
            {
                bowlSound.Play();
            }
            else if (!isPlaying && bowlSound.isPlaying)
            {
                bowlSound.Pause();
            }
        }
    }
}