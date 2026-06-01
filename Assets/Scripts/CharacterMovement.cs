using UnityEngine;


public class CharacterMovement :  MonoBehaviour
{
    
    public float speed = 5f;

    public Transform cameraTransform;
    public float mouseSensitivity = 100f;

    float xRotation = 0f;

    void Update(){
        Move();
        look();
    }

    void Start()
    {
        Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible=false;
    }
    void Move(){
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 movement = forward*z+right*x;
        transform.position += movement *speed*Time.deltaTime;
    }
    void look()
    {
        float MouseX = Input.GetAxis("Mouse X");
        float MouseY = Input.GetAxis("Mouse Y");

        xRotation -= MouseY;

        xRotation = Mathf.Clamp(xRotation,-90f,90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation,0f,0f);

        transform.Rotate(Vector3.up*MouseX);
    }
}
