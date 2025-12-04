using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
[Header("Camera Settings")]
public Camera mainCamera;
public float moveSpeed = 10f;
public float rotateSpeed = 80f;
public float zoomSpeed = 50f;
public float minZoom = 15f;
public float maxZoom = 25f;


private Vector3 dragOrigin;
private bool isDragging = false;

private void Update()
{
    if (mainCamera == null)
        mainCamera = Camera.main;
    if (mainCamera == null) return;

    HandleCameraMovement();
    HandleCameraRotation();
    HandleZoom();
    HandleLeftClick();
}

//----------------------------------
// CAMERA MOVEMENT HORYZONTALNE
//----------------------------------
private void HandleCameraMovement()
{
    Vector3 move = Vector3.zero;

    if (Keyboard.current.wKey.isPressed) move += new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z).normalized;
    if (Keyboard.current.sKey.isPressed) move -= new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z).normalized;
    if (Keyboard.current.aKey.isPressed) move -= new Vector3(mainCamera.transform.right.x, 0, mainCamera.transform.right.z).normalized;
    if (Keyboard.current.dKey.isPressed) move += new Vector3(mainCamera.transform.right.x, 0, mainCamera.transform.right.z).normalized;

    mainCamera.transform.position += move * moveSpeed * Time.deltaTime;

    // Dragging with middle mouse
    if (Mouse.current.middleButton.isPressed)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        if (!isDragging)
        {
            dragOrigin = mousePos;
            isDragging = true;
        }
        else
        {
            Vector2 delta = mousePos - new Vector2(dragOrigin.x, dragOrigin.y);
            Vector3 right = new Vector3(mainCamera.transform.right.x, 0, mainCamera.transform.right.z).normalized;
            Vector3 forward = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z).normalized;

            mainCamera.transform.position -= right * delta.x * 0.01f;
            mainCamera.transform.position -= forward * delta.y * 0.01f;

            dragOrigin = mousePos;
        }
    }
    else
    {
        isDragging = false;
    }
}

//----------------------------------
// CAMERA ROTATION POZIOMA
//----------------------------------
private void HandleCameraRotation()
{
    if (Mouse.current.rightButton.isPressed)
    {
        Vector2 delta = Mouse.current.delta.ReadValue();
        mainCamera.transform.Rotate(Vector3.up, delta.x * rotateSpeed * Time.deltaTime, Space.World);
    }
}

//----------------------------------
// CAMERA ZOOM
//----------------------------------
private void HandleZoom()
{
    float scroll = Mouse.current.scroll.ReadValue().y;
    Vector3 pos = mainCamera.transform.position;
    pos += new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z).normalized * scroll * zoomSpeed * Time.deltaTime;
    pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom);
    mainCamera.transform.position = pos;
}

//----------------------------------
// LEFT CLICK HANDLING
//----------------------------------
private void HandleLeftClick()
{
    if (!Mouse.current.leftButton.wasPressedThisFrame) return;

    Vector2 mousePos = Mouse.current.position.ReadValue();
    Ray ray = mainCamera.ScreenPointToRay(mousePos);

    if (Physics.Raycast(ray, out RaycastHit hit))
    {
        Farm farm = hit.collider.GetComponent<Farm>();
        if (farm != null)
        {
            farm.OnFarmClicked();
            Debug.Log("Farm clicked!");
            return;
        }
    }
}


}
