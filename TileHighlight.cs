using UnityEngine;

public class TileHighlight : MonoBehaviour
{
    private Renderer[] renderers;
    private Color[] baseColors;
    private bool isHighlighted = false;
    private bool canBuild = true;
    private float lerpSpeed = 12f;
    private bool renderersReady = false;

    void Update()
    {
        if (!renderersReady)
        {
            renderers = GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                baseColors = new Color[renderers.Length];
                for (int i = 0; i < renderers.Length; i++)
                {
                    // Tworzymy instancję materiału, aby podświetlenie nie wpływało na inne obiekty
                    renderers[i].material = new Material(renderers[i].material);
                    baseColors[i] = renderers[i].material.color;
                }
                renderersReady = true;
            }
        }

        if (!renderersReady) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            Color target;
            if (isHighlighted)
                target = canBuild ? Color.green : Color.red; // zielony jeśli można budować, czerwony jeśli nie
            else
                target = baseColors[i];

            renderers[i].material.color = Color.Lerp(renderers[i].material.color, target, Time.deltaTime * lerpSpeed);
        }
    }

    // Nowa metoda z argumentem bool canBuild
    public void SetHighlight(bool state, bool canBuildHere)
    {
        isHighlighted = state;
        canBuild = canBuildHere;
    }
}
