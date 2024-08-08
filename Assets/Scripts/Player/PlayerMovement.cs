using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Animator _animator;

    private PlayerControls _controls;
    private CharacterController _characterController;

    [Header("Movement Info")]
    [SerializeField] private float _gravityScale = 9.81f;
    [SerializeField] private float _walkSpeed = 1.5f;
    [SerializeField] private float _runSpeed = 3f;
    [Range(10f, 100f)]
    [SerializeField] private float _turnSpeed = 10f;
    private float _speed;
    private float _verticalVelocity;

    private Vector3 _movementDirection;
    public Vector2 MoveInput { get; private set; }

    private bool _isRunning;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();

        _speed = _walkSpeed;

        AssignInputEvents();
    }

    private void Update()
    {
        ApplyMovement();
        ApplyRotation();
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

    private void ApplyRotation()
    {
        Vector3 lookingDirection = _player.Aim.GetMouseHitInfo().point - transform.position;
        lookingDirection.y = 0f;
        lookingDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        _movementDirection = new Vector3(MoveInput.x, 0, MoveInput.y);
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

        _controls.Character.Movement.performed += context => MoveInput = context.ReadValue<Vector2>();
        _controls.Character.Movement.canceled += context => MoveInput = Vector2.zero;

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
