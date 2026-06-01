using UnityEngine;


public class CharacterMovement :  MonoBehaviour
{
    
    public float speed = 50f;

    public Transform cameraTransform;
    public float mouseSensitivity = 3f;

    float xRotation = 0f;
    float yRotation = 0f;
    void Start()
    {
        Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible=false;

        yRotation = transform.localEulerAngles.y;
        controller = GetComponent<CharacterController>();
    }
    private CharacterController controller;

    void Update(){
        Move();
        look();
    }

    
    void Move(){
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 localMovement = new Vector3(x,0,z) * speed * Time.deltaTime;

        if (localMovement.sqrMagnitude > 0)
        {
            Vector4 currentPos = new Vector4(transform.position.x,transform.position.y,transform.position.z,1f);

            
            Matrix4x4 rotMatrix = Transformations.RotateM(yRotation,Transformations.AXIS.AX_Y);
            Vector4 worldMovement = rotMatrix * new Vector4(localMovement.x,localMovement.y,localMovement.z,0f);

            Matrix4x4 translationMatrix = Transformations.TranslateM(worldMovement.x,worldMovement.y,worldMovement.z);

            Vector4 newPos = translationMatrix*currentPos;
            
            transform.position = new Vector3(newPos.x,newPos.y,newPos.z);
        }
    }
    void look()
    {
        float MouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        yRotation += MouseX;

        transform.localRotation = Quaternion.Euler(0f,yRotation,0f);

        xRotation -= MouseY;

        xRotation = Mathf.Clamp(xRotation,-90f,90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation,0f,0f);
    }
}
