using UnityEngine;

public class GlowAnimator : MonoBehaviour
{
    public Renderer targetRenderer;
    public Color glowColor = Color.cyan;
    public float glowSpeed = 2f; // Speed at which glow fades in and out

    private Material targetMaterial;
    private bool glowing = false;
    private float emissionIntensity = 0f;
    private bool increasing = true;

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
        {
            targetMaterial = targetRenderer.material;
            targetMaterial.EnableKeyword("_EMISSION");
            targetMaterial.SetColor("_EmissionColor", Color.black);
        }
        else
        {
            Debug.LogWarning("GlowAnimator: targetRenderer not assigned and no Renderer component found.");
        }
    }

    public void StartGlow()
    {
        glowing = true;
        gameObject.SetActive(true); // Ensure frame is visible
    }

    public void StopGlow()
    {
        glowing = false;
        emissionIntensity = 0f;

        if (targetMaterial != null)
        {
            targetMaterial.SetColor("_EmissionColor", Color.black);
        }

        gameObject.SetActive(false); // Hide frame
    }

    void Update()
    {
        if (!glowing) return;

        // Animate intensity up and down
        if (increasing)
        {
            emissionIntensity += glowSpeed * Time.deltaTime;
            if (emissionIntensity >= 1f)
            {
                emissionIntensity = 1f;
                increasing = false;
            }
        }
        else
        {
            emissionIntensity -= glowSpeed * Time.deltaTime;
            if (emissionIntensity <= 0f)
            {
                emissionIntensity = 0f;
                increasing = true;
            }
        }

        Color currentEmission = glowColor * emissionIntensity;
        targetMaterial.SetColor("_EmissionColor", currentEmission);
    }
}
