using UnityEngine;
using TMPro;

public class ResourcePopup : MonoBehaviour
{
    public float floatSpeed = 30f;
    public float lifetime = 1f;
    public Color faceColor = Color.green; // ustaw kolor w inspektorze

    private TMP_Text text;
    private float timer = 0f;

    private void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            // Tworzymy instancję materiału, żeby zmienić tylko ten tekst
            text.fontMaterial = new Material(text.fontMaterial);
            text.fontMaterial.SetColor("_FaceColor", faceColor);
        }
    }

    public void Init(string content)
    {
        if (text != null)
            text.text = content;
    }

    private void Update()
    {
        transform.localPosition += Vector3.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);

        if (text != null)
        {
            Color c = text.color;
            c.a = alpha;
            text.color = c;
        }

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
