using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform cameraTransform;

    void Update()
    {
        if (cameraTransform == null) return;

        // Alleen richting bepalen, maar Y (hoogte) gelijk houden
        Vector3 direction = cameraTransform.position - transform.position;
        direction.y = 0; // negeer verticale rotatie

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }
}