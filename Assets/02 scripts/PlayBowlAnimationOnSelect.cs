using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class PlayBowlAnimationOnSelect : MonoBehaviour
{
    private float lastSelectTime = 0f;
    public float cooldownDuration = 1f; // aantal seconden tussen selecties

    public AudioSource bowlSound;
    private Animator animator;
    private bool isPlaying = false;

    void Start()
    {
        Debug.Log("PlayBowlAnimationOnSelect script started.");
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
        if (Time.time - lastSelectTime < cooldownDuration)
            return; // negeer als we nog in cooldown zijn

        lastSelectTime = Time.time;

        if (animator == null || bowlSound == null) return;

        if (!isPlaying)
        {
            isPlaying = true;
            Debug.Log("Bowl playing");

            animator.SetBool("Play", true);
            animator.speed = 1f;
            if (!bowlSound.isPlaying)
            {
                bowlSound.Play();
            }
        }
        else
        {
            isPlaying = false;
            Debug.Log("Bowl paused");

            animator.SetBool("Play", false);
            animator.speed = 0f;
            if (bowlSound.isPlaying)
            {
                bowlSound.Pause();
            }
        }
    }
}