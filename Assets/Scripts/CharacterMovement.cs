using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    // ── Movimiento y cámara (sin cambios) ─────────────────────────────────────
    public float speed = 12f;
    public Transform cameraTransform;
    public float mouseSensitivity = 20f;

    float xRotation = 0f;
    float yRotation = 0f;
    private CharacterController controller;

    // ── Recolección de monedas ─────────────────────────────────────────────────
    [Header("Recolección de monedas")]
    public GridManager gridManager;

    [Tooltip("Radio en unidades de mundo para recoger monedas")]
    public float collectRadius = 2f;

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
        yRotation        = transform.localEulerAngles.y;
        controller       = GetComponent<CharacterController>();
    }

    void Update()
    {
        Move();
        look();
        RecogerMonedas();
    }

    // ── Movimiento (sin cambios) ───────────────────────────────────────────────
    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 localMovement = new Vector3(x, 0, z) * speed * Time.deltaTime;

        if (localMovement.sqrMagnitude > 0)
        {
            Vector4 currentPos = new Vector4(transform.position.x, transform.position.y, transform.position.z, 1f);
            Matrix4x4 rotMatrix  = Transformations.RotateM(yRotation, Transformations.AXIS.AX_Y);
            Vector4 worldMovement = rotMatrix * new Vector4(localMovement.x, localMovement.y, localMovement.z, 0f);
            controller.Move(new Vector3(worldMovement.x, worldMovement.y, worldMovement.z));
        }
    }

    // ── Cámara (sin cambios) ───────────────────────────────────────────────────
    void look()
    {
        float MouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        yRotation += MouseX;
        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);

        xRotation -= MouseY;
        xRotation  = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // ── Detección de monedas por distancia (no depende de triggers ni Rigidbody)
    void RecogerMonedas()
    {
        coin[] monedas = FindObjectsOfType<coin>();
        foreach (coin c in monedas)
        {
            if (c == null || c.IsCollected) continue;

            float dist = Vector3.Distance(
                new Vector3(transform.position.x, 0f, transform.position.z),
                new Vector3(c.transform.position.x, 0f, c.transform.position.z)
            );

            if (dist <= collectRadius)
            {
                c.CollectFromNode();
                break;
            }
        }
    }
}