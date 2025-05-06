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
        interactable.selectEntered.AddListener(OnSelect);

        if (animator != null)
        {
            animator.SetBool("Play", false); // Zorg dat animatie niet vanzelf start
        }
    }

    public void OnSelect(SelectEnterEventArgs args)
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