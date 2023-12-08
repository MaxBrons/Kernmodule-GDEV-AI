using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 1;
    public float yPos = 15;
    public float zoomSpeed = 2;
    public float lerpSpeed = 1;
    private Vector3 targetPos;

    private const string HORIZONTAL_MOVEMENT_AXIS_NAME = "Horizontal";
    private const string VERTICAL_MOVEMENT_AXIS_NAME = "Vertical";
    private const string SCROLL_AXIS_NAME = "Mouse ScrollWheel";

    void Start()
    {
        targetPos = new Vector3(0, yPos, 0);
    }

    void Update()
    {
        float vert = Input.GetAxis(VERTICAL_MOVEMENT_AXIS_NAME);
        float hor = Input.GetAxis(HORIZONTAL_MOVEMENT_AXIS_NAME);

        if (vert != 0 || hor != 0) {
            targetPos += (Vector3.forward * vert + hor * Vector3.right).normalized * moveSpeed;
        }

        float scroll = Input.GetAxis(SCROLL_AXIS_NAME);
        if (scroll != 0) {
            yPos += -Mathf.Sign(scroll) * zoomSpeed;
            yPos = Mathf.Clamp(yPos, 1, 100);
            targetPos = new Vector3(targetPos.x, yPos, targetPos.z);
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }
}
