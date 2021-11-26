using UnityEngine;
// ------------------------------------------------------------------------------ 
// Quiz  
// Written by: Alex Fourneaux 40022711
// For COMP 376 – Fall 2021 
// -----------------------------------------------------------------------------
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField, Min(1)] private float sensitivity = 400;

    private float mouseX, mouseY;

    private float distanceToTarget;
    private float cameraZoom;

    public float smoothing = 5;
    private float raycastBuffer = 0.1f;
    private float zoomDelay = 0.2f;
    private float zoomTimer;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (target != null)
        {
            Vector3 offset = transform.position - target.transform.position;
            distanceToTarget = offset.magnitude;

            Vector3 xzDirection = Vector3.ProjectOnPlane(offset, Vector3.up);
            mouseY = Vector3.Angle(offset, xzDirection);
        }
        
    }

    private void Update()
    {
        if (target != null)
        {
            mouseX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            mouseY -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            mouseY = Mathf.Clamp(mouseY, 20, 70);

            // See if any environment pieces are between the camera and player. If so, zoom the camera in. Otherwise, reset camera zoom
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, (transform.position - target.transform.position).magnitude - raycastBuffer, (1 << 8))) {
                 cameraZoom += Time.deltaTime * smoothing;
                 zoomTimer = zoomDelay;
                 if (cameraZoom > distanceToTarget - raycastBuffer) {
                     cameraZoom = distanceToTarget - raycastBuffer;
                 }
            } else {
                zoomTimer -= Time.deltaTime;
                if (cameraZoom > 0 && zoomTimer <= 0) {
                    cameraZoom -= Time.deltaTime * smoothing;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Quaternion desiredRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(mouseY, mouseX, 0), smoothing * Time.deltaTime);

            transform.position = target.transform.position + desiredRotation * -Vector3.forward * (distanceToTarget - cameraZoom);
            transform.LookAt(target.transform);
        }
    }
}
