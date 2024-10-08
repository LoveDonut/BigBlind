using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using PlayerState;

// made by KimDaehui
public class PlayerMovement : MonoBehaviour
{
    #region References
    [Header("Move")]
    [SerializeField] float _acceleration = 50f;    // acceleration
    [SerializeField] float _deceleration = 20f;    // decceleration
    [SerializeField] float _maxSpeed = 7.5f;         // max move speed

    [Header("SFX")]
    public AudioSource HeartBeat;
    public AudioSource Beat;
    #endregion

    #region PrivateVariables


    Rigidbody2D _rb;
    Vector2 _input;
    Vector2 _velocity;
    #endregion

    #region PublicVariables
    [HideInInspector] public bool IsMovable;
    [HideInInspector] public StateMachine CurrentState;
    #endregion

    #region PrivateMethods
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        _rb = GetComponent<Rigidbody2D>();
        IsMovable = true;
    }

    void Update()
    {
        Move();
    }

    void OnMove(InputValue value)
    {
        if (!IsMovable) return;

        _input = value.Get<Vector2>();

        _input.Normalize(); // keep the speed in the diagonal direction the same
    }

    void SetStartState()
    {
        CurrentState = new IdleState();

        CurrentState.EnterState(gameObject);
    }

    void FixedUpdate()
    {
        if (!IsMovable) return;

        // set speed
        _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
    }
    #endregion

    #region PublicMethods
    public void Move()
    {
        if (!IsMovable) return;

        // set acceleration and decceleration
        if (_input.magnitude > 0)
        {
            _velocity += _input * _acceleration * Time.deltaTime;
        }
        else
        {
            _velocity = Vector2.MoveTowards(_velocity, Vector2.zero, _deceleration * Time.deltaTime);
        }

        // limit max speed
        _velocity = Vector2.ClampMagnitude(_velocity, _maxSpeed);
    }
    #endregion
}
