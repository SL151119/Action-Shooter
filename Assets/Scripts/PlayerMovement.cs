using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player _player;
    private PlayerControls _controls;
    private CharacterController _characterController;

    [SerializeField] private Animator _animator;

    [Header("Movement Info")]
    [SerializeField] private float _gravityScale = 9.81f;
    [SerializeField] private float _walkSpeed = 1.5f;
    [SerializeField] private float _runSpeed = 3f;
    private float _speed;
    private Vector3 _movementDirection;
    private float _verticalVelocity;
    private bool _isRunning;

    [Header("Aim Info")]
    [SerializeField] private Transform _aim;
    [SerializeField] private LayerMask _aimLayerMask;
    private Vector3 _lookingDirection;

    private Vector2 _moveInput;
    private Vector2 _aimInput;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();

        _speed = _walkSpeed;

        AssignInputEvents();
    }

    private void Update()
    {
        ApplyMovement();
        AimTowardsMouse();
        AnimatorControllers();
    }

    private void AnimatorControllers()
    {
        float xVelocity = Vector3.Dot(_movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(_movementDirection.normalized.normalized, transform.forward);

        _animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
        _animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);

        bool playRunAnimation = _isRunning && _movementDirection.magnitude > 0;
        _animator.SetBool("isRunning", playRunAnimation);
    }

    private void AimTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(_aimInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _aimLayerMask))
        {
            _lookingDirection = hitInfo.point - transform.position;
            _lookingDirection.y = 0f;
            _lookingDirection.Normalize();

            transform.forward = _lookingDirection;

            _aim.position = new Vector3(hitInfo.point.x, transform.position.y + 1, hitInfo.point.z);
        }
    }

    private void ApplyMovement()
    {
        _movementDirection = new Vector3(_moveInput.x, 0, _moveInput.y);
        ApplyGravity();

        if (_movementDirection.magnitude > 0)
        {
            _characterController.Move(_speed * Time.deltaTime * _movementDirection);
        }
    }

    private void ApplyGravity()
    {
        if (!_characterController.isGrounded)
        {
            _verticalVelocity = _verticalVelocity - _gravityScale * Time.deltaTime;
            _movementDirection.y = _verticalVelocity;
        }
        else
        {
            _verticalVelocity = -0.5f;
        }
    }

    private void AssignInputEvents()
    {
        _controls = _player.Controls;

        _controls.Character.Movement.performed += context => _moveInput = context.ReadValue<Vector2>();
        _controls.Character.Movement.canceled += context => _moveInput = Vector2.zero;

        _controls.Character.Aim.performed += context => _aimInput = context.ReadValue<Vector2>();
        _controls.Character.Aim.canceled += context => _aimInput = Vector2.zero;

        _controls.Character.Run.performed += context =>
        {
            _speed = _runSpeed;
            _isRunning = true;
        };

        _controls.Character.Run.canceled += context =>
        {
            _speed = _walkSpeed;
            _isRunning = false;
        };
    }
}
