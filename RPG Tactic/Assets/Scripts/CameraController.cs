using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float MIN_FOLLOW_Y_OFFSET = 2F;
    private const float MAX_FOLLOW_Y_OFFSET = 15F;

    public static CameraController Instance { get; private set; }

    [SerializeField] private CinemachineCamera cinemachineVirtualCamera;

    private CinemachineFollow cinemachinTransposer;
    private Vector3 targetFollowOffset;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cinemachinTransposer = cinemachineVirtualCamera.GetComponent<CinemachineFollow>();
        targetFollowOffset = cinemachinTransposer.FollowOffset;
    }

    private void Update()
    {
        HandleMovement();

        HandleRotation();

        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();

        float moveSpeed = 10f;

        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

        float rotationSpeed = 100f;
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float zoomIncreaseAmount = 1f;
        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        float zoomSpeed = 5f;
        cinemachinTransposer.FollowOffset = Vector3.Lerp(cinemachinTransposer.FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }
    public float GetCameraHeight()
    {
        return targetFollowOffset.y;
    }
}
