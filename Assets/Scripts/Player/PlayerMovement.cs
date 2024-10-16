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
    [SerializeField] float _stoppingThreshold = 0.1f; // threshold for stopping deceleration

    [Header("SFX")]
    [HideInInspector]
    public AudioSource HeartBeat;
    [HideInInspector]
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
        HeartBeat = SoundManager.Instance.HeartBeatAudio;
        Beat = SoundManager.Instance.BGMaudio;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        _rb = GetComponent<Rigidbody2D>();
        IsMovable = true;

        SetStartState();
    }

    void Update()
    {
        if (!IsMovable) return;

        CurrentState.UpdateState(gameObject);
    }
    void FixedUpdate()
    {
        if (!IsMovable) return;

        CurrentState.FixedUpdateState(gameObject);
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
    #endregion

    #region PublicMethods
    public void Move()
    {
        if (!IsMovable) return;

        // apply acceleration and decceleration
        if (_input.magnitude > 0)
        {
            _velocity = Vector2.MoveTowards(_velocity, _input * _maxSpeed, _acceleration * Time.fixedDeltaTime);
        }
        else
        {
            _velocity = Vector2.MoveTowards(_velocity, Vector2.zero, _deceleration * Time.fixedDeltaTime);
        }

        if(_velocity.magnitude < _stoppingThreshold)
        {
            _velocity = Vector2.zero;
        }

        _rb.velocity = _velocity;
    }
    #endregion
}
