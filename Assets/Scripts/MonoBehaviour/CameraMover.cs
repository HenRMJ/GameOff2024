using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private float _dollyDuration;
    [SerializeField] private float _dollyInMax;
    [SerializeField] private float _dollyOutMax;

    [Header("Zoom Settings")] 
    [SerializeField] private float fieldOfViewMin;
    [SerializeField] private float fieldOfViewMax;
    [SerializeField] private float zoomAmount;
    [SerializeField] private float zoomSpeed;

    private PlayerInput _input;
    private CinemachineFollow _vCamFollow;
    
    private Transform _cameraTransform;
    private Coroutine _smoothHeightChange;
    private Coroutine _smoothDolly;

    private float _originalCameraHeight;
    private float _originalCameraDistance;
    private float _originalFieldOfView;
    private float _targetFieldOfView;
    private float _realMoveSpeed;
    private float _realRotationSpeed;

    
    private void Start()
    {
        _input = PlayerInput.Instance;
        _cameraTransform = Camera.main.transform;
        _targetFieldOfView = _vCam.Lens.FieldOfView;
        _vCamFollow = _vCam.GetComponent<CinemachineFollow>();
        _originalFieldOfView = _targetFieldOfView;
        _originalCameraHeight = _vCamFollow.FollowOffset.y;
        _originalCameraDistance = _vCamFollow.FollowOffset.z;
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
        
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _targetFieldOfView = _originalFieldOfView;
                if (_smoothHeightChange != null)
                {
                    StopCoroutine(_smoothHeightChange);
                }
                _smoothHeightChange = StartCoroutine(SmoothHeightChange(_realMoveSpeed));
            }
            
            if (Input.mouseScrollDelta.y > 0)
            {
                if (_smoothDolly != null)
                {
                    StopCoroutine(_smoothDolly);
                }

                _smoothDolly = StartCoroutine(SmoothDolly(_dollyDuration, _realMoveSpeed));
            }

            if (Input.mouseScrollDelta.y < 0)
            {
                if (_smoothDolly != null)
                {
                    StopCoroutine(_smoothDolly);
                }

                _smoothDolly = StartCoroutine(SmoothDolly(_dollyDuration, -_realMoveSpeed));
            }
        }
        else
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                _targetFieldOfView -= zoomAmount;
            }

            if (Input.mouseScrollDelta.y < 0)
            {
                _targetFieldOfView += zoomAmount;
            }
        }
        
        _targetFieldOfView = Mathf.Clamp(_targetFieldOfView, fieldOfViewMin, fieldOfViewMax);
        _vCam.Lens.FieldOfView = Mathf.Lerp(_vCam.Lens.FieldOfView, _targetFieldOfView, zoomSpeed * Time.deltaTime);
    }

    private IEnumerator SmoothHeightChange(float realSpeed)
    {
        while (!Mathf.Approximately(_vCamFollow.FollowOffset.y, _originalCameraHeight) || !Mathf.Approximately(_vCamFollow.FollowOffset.z, _originalCameraDistance))
        {
            _vCamFollow.FollowOffset.y =
                Mathf.Lerp(_vCamFollow.FollowOffset.y, _originalCameraHeight, realSpeed * Time.deltaTime);
            
            _vCamFollow.FollowOffset.z =
                Mathf.Lerp(_vCamFollow.FollowOffset.z, _originalCameraDistance, realSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator SmoothDolly(float duration, float speed)
    {
        float timer = 0f;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            
            _vCamFollow.FollowOffset.z += speed * Time.deltaTime;
            yield return null;
        }

        _vCamFollow.FollowOffset.z = Mathf.Clamp(_vCamFollow.FollowOffset.z, _dollyOutMax, _dollyInMax);
    }
}
