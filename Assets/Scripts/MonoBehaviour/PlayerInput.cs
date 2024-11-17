using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }

    private GamePlayerInput _gamePlayerInput;

    public InputAction Move;
    public InputAction Look;
    
    public void Awake()
    {
        Instance = this;

        _gamePlayerInput = new();
        _gamePlayerInput.Player.Enable();
        
        Move = _gamePlayerInput.Player.Move;
        Look = _gamePlayerInput.Player.Look;
    }
}
