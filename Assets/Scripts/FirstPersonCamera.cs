using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 2f;
    public Transform player;
    float mouseX;
    float mouseY;
    float xRotation = 0f;
    public float Xrotation {get { return mouseX; }}
    public float Yrotation {get { return mouseY; }}
    void GetMouseInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
    private void MoveCamera()
    {
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(Vector3.up, mouseX);
    }
    void HideCursor() => Cursor.lockState = CursorLockMode.Locked;
    // Update is called once per frame
    void Update()
    {
        GetMouseInput();
        MoveCamera();
    }
    private void Start()
    {
        HideCursor(); 
    }
}
