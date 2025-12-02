using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorTexture; // przypisz w Inspectorze
    public Vector2 hotspot = Vector2.zero; // punkt "kliknięcia" kursora
    public CursorMode cursorMode = CursorMode.Auto;

    private void Start()
    {
        if(cursorTexture != null)
            Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
    }
}