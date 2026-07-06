using UnityEngine;

public class SpoonMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 15f;
    [SerializeField] float horizontalRange = 5f;
    [SerializeField] float minDepth = -3f;
    [SerializeField] float maxDepth = 3f;
    [SerializeField] float minHeight = 0.5f;
    [SerializeField] float maxHeight = 5f;
    [SerializeField] float heightSpeed = 5f;
    [SerializeField] float heightSmooth = 8f;
    [SerializeField] float maxYRotation = 45f;
    [SerializeField] float yRotationSmooth = 8f;
    [SerializeField] float tiltAmount = 15f;
    [SerializeField] float tiltSmooth = 8f;
    [SerializeField] Vector3 rotationOffset;

    private Rigidbody rb;
    private Vector3 tarPos;
    private Vector3 curTilt;
    private float tarHeight;
    private float curHeight;
    private float curRot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tarHeight = transform.position.y;
        curHeight = transform.position.y;
    }

    void Update()
    {
        float mouseXNorm = Mathf.Clamp01(Input.mousePosition.x / Screen.width);
        float xPos = Mathf.Lerp(-horizontalRange, horizontalRange, mouseXNorm);

        float mouseYNorm = Mathf.Clamp01(Input.mousePosition.y / Screen.height);
        float zPos = Mathf.Lerp(minDepth, maxDepth, mouseYNorm);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        tarHeight += scroll * heightSpeed;
        tarHeight = Mathf.Clamp(tarHeight, minHeight, maxHeight);
        curHeight = Mathf.Lerp(curHeight, tarHeight, Time.deltaTime * heightSmooth);
        tarPos = new Vector3(xPos, curHeight, zPos);

        float tarYRot = Mathf.Lerp(maxYRotation, 0f, mouseYNorm);
        curRot = Mathf.Lerp(curRot, tarYRot, Time.deltaTime * yRotationSmooth);
        float mouseMoveX = Input.GetAxis("Mouse X");
        float mouseMoveY = Input.GetAxis("Mouse Y");
        float tarTiltZ = Mathf.Clamp(-mouseMoveX * tiltAmount, -tiltAmount, tiltAmount);
        float tarTiltX = Mathf.Clamp(mouseMoveY * tiltAmount, -tiltAmount, tiltAmount);

        curTilt.x = Mathf.Lerp(curTilt.x, tarTiltX, Time.deltaTime * tiltSmooth);
        curTilt.z = Mathf.Lerp(curTilt.z, tarTiltZ, Time.deltaTime * tiltSmooth);
    }

    void FixedUpdate()
    {
        Vector3 smoothPos = Vector3.Lerp(rb.position, tarPos, Time.fixedDeltaTime * moveSpeed);
        rb.MovePosition(smoothPos);

        Quaternion tarRot = Quaternion.Euler(
            curTilt.x + rotationOffset.x,
            curRot + rotationOffset.y,
            curTilt.z + rotationOffset.z
        );
        rb.MoveRotation(tarRot);
    }
}
