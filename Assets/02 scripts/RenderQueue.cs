using UnityEngine;

public class RenderQueue : MonoBehaviour
{
    public int renderQueue = 3100; // Of bv. 3200

    void Start()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.renderQueue = renderQueue;
        }
    }
}