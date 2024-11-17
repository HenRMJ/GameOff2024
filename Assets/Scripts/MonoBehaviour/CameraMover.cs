using System;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotationSpeed;

    private PlayerInput _input;

    private void Start()
    {
        _input = PlayerInput.Instance;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += Vector3.up * _moveSpeed * Time.deltaTime;
        }
        
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= Vector3.up * _moveSpeed * Time.deltaTime;
        }
        
        Vector2 moveDirection = _input.Move.ReadValue<Vector2>();
        Vector2 lookDirection = _input.Look.ReadValue<Vector2>();

        Vector3 currentRotation = transform.eulerAngles;
        
        transform.position += (transform.forward * moveDirection.y + transform.right * moveDirection.x) 
                              * _moveSpeed * Time.deltaTime;
        
        currentRotation.y += lookDirection.x * _rotationSpeed * Time.deltaTime;
        currentRotation.x -= lookDirection.y * _rotationSpeed * Time.deltaTime;

        transform.eulerAngles = currentRotation;
    }
}
