using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Blueprint : MonoBehaviour
{
    [Header("Colors")]
    public Color validColor = new Color(0f, 0.6f, 0f, 0.3f);
    public Color invalidColor = new Color(0.6f, 0f, 0f, 0.3f);

    [Header("Transparency & Pulsing")]
    [Range(0f, 1f)] public float alpha = 0.3f;
    public float pulseAmount = 0.1f;
    public float pulseSpeed = 3f;

    private Renderer[] allRenderers;
    private Vector3 initialScale;

    private void Awake()
    {
        // Pobierz wszystkie MeshRenderer i SkinnedMeshRenderer w dzieciach
        allRenderers = GetComponentsInChildren<Renderer>();
        initialScale = transform.localScale;

        // Ustaw początkową przezroczystość
        SetMaterialAlpha(alpha);
    }

    private void Update()
    {
        // Pulsowanie skali
        float scaleFactor = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = initialScale * scaleFactor;
    }

    private void SetMaterialAlpha(float a)
    {
        foreach (var r in allRenderers)
        {
            if (r == null) continue;

            foreach (var mat in r.materials)
            {
                if (mat == null) continue;

                if (mat.HasProperty("_BaseColor"))
                {
                    Color c = mat.GetColor("_BaseColor");
                    c.a = a;
                    mat.SetColor("_BaseColor", c);
                }
                else
                {
                    Color c = mat.color;
                    c.a = a;
                    mat.color = c;
                }
            }
        }
    }

    public void SetValid(bool isValid)
    {
        Color targetColor = isValid ? validColor : invalidColor;
        targetColor.a = alpha;

        foreach (var r in allRenderers)
        {
            if (r == null) continue;

            foreach (var mat in r.materials)
            {
                if (mat == null) continue;

                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", targetColor);
                else
                    mat.color = targetColor;
            }
        }
    }
}
