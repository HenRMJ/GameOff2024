using Unity.Cinemachine;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _vCam;
    
    [Header("Camera Movement Values")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _fastMoveSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _fastRotationSpeed;
    [SerializeField] private float _cameraHeightMin;
    [SerializeField] private float _cameraHeightMax;

    [Header("Zoom Settings")] 
    [SerializeField] private float fieldOfViewMin;
    [SerializeField] private float fieldOfViewMax;
    [SerializeField] private float zoomAmount;
    [SerializeField] private float zoomSpeed;
    
    private PlayerInput _input;
    private Transform _cameraTransform;
    private CinemachineFollow _vCamFollow;
    private float _targetFieldOfView;
    private float _realMoveSpeed;
    private float _realRotationSpeed;

    private void Start()
    {
        _input = PlayerInput.Instance;
        _cameraTransform = Camera.main.transform;
        _targetFieldOfView = _vCam.Lens.FieldOfView;
        _vCamFollow = _vCam.GetComponent<CinemachineFollow>();
    }

    private void Update()
    {
        Vector2 inputDiretion = _input.Move.ReadValue<Vector2>();
        Vector3 moveDirection = new(inputDiretion.x, 0f, inputDiretion.y);
        
        _realMoveSpeed = _moveSpeed;
        _realRotationSpeed = _rotationSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _realMoveSpeed = _fastMoveSpeed;
            _realRotationSpeed = _fastRotationSpeed;
        }
        
        if (Input.GetKey(KeyCode.T))
        {
            _vCamFollow.FollowOffset.y += _realMoveSpeed * Time.deltaTime;
        }
        
        if (Input.GetKey(KeyCode.R))
        {
            _vCamFollow.FollowOffset.y -= _realMoveSpeed * Time.deltaTime;
        }

        _vCamFollow.FollowOffset.y = Mathf.Clamp(_vCamFollow.FollowOffset.y, _cameraHeightMin, _cameraHeightMax);
        
        moveDirection = _cameraTransform.forward * moveDirection.z + _cameraTransform.right * moveDirection.x;
        moveDirection.y = 0f;
        moveDirection.Normalize();

        transform.position += moveDirection * (_realMoveSpeed * Time.deltaTime);

        float rotationAmount = 0f;
        if (Input.GetKey(KeyCode.Q))
        {
            rotationAmount = 1f;
        }

        if (Input.GetKey(KeyCode.E))
        {
            rotationAmount = -1f;
        }

        transform.eulerAngles += new Vector3(0f, rotationAmount * _realRotationSpeed * Time.deltaTime, 0f);
        if (Input.mouseScrollDelta.y > 0)
        {
            _targetFieldOfView -= zoomAmount;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            _targetFieldOfView += zoomAmount;
        }

        _targetFieldOfView = Mathf.Clamp(_targetFieldOfView, fieldOfViewMin, fieldOfViewMax);

        _vCam.Lens.FieldOfView = Mathf.Lerp(_vCam.Lens.FieldOfView, _targetFieldOfView, zoomSpeed * Time.deltaTime);
    }
}
