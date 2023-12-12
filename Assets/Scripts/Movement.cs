using System;
using System.Net.NetworkInformation;
using UnityEngine;

/// <summary>
/// Controls the player's movement, including walking, sprinting, and animation.
/// </summary>
public class Movement : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;

    private Animator _animator;
    private CharacterController _characterController;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _inputVector;
    private Vector3 _velocity;
    private bool _battleStarted = false;

    void Start()
    {
        // Initialize components
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // Set initial downward momentum
        _velocity.y = -5f; // Some downward momentum so that the character doesn't fall too slowly in the beginning

        // Shadow casting settings
        // Because Unity thinks SpriteRenderers casting shadows is forbidden by international law
        _spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        _spriteRenderer.receiveShadows = true;
    }

    void Update()
    {
        // Get input from player
        float dx = Input.GetAxisRaw("Horizontal");
        float dz = Input.GetAxisRaw("Vertical");

        // Normalize input vector
        _inputVector = new Vector3(dx, 0, dz).normalized;

        // Determine movement state for animation
        bool isMoving = _inputVector.sqrMagnitude > 0;
        _animator.SetBool("IsWalking", isMoving);

        // Check for sprinting and adjust speed and animation speed
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        _animator.SetFloat("AnimationSpeed", Input.GetKey(KeyCode.LeftShift) ? 1.5f : 1.0f);

        // Calculate movement direction
        Vector3 moveDirection = _inputVector * currentSpeed;

        // Apply gravity
        if (_characterController.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -5f; // Reset downward velocity when grounded
        }
        _velocity.y += Physics.gravity.y * Time.deltaTime; // Apply gravity effect

        // Combine movement and gravity, then move the character
        _characterController.Move((moveDirection + _velocity) * Time.deltaTime);

        // Flip the character sprite based on movement direction
        if (dx != 0)
        {
            // Introducing a variable to make this access efficient
            // Dear Professor, JetBrains Rider told me to do this
            // and it makes sense, so wherever you see this, know that
            // my IDE is telling me to do it.
            var localScale = transform.localScale;
            localScale = new Vector3(Mathf.Sign(dx), localScale.y, localScale.z);
            transform.localScale = localScale;
        }
    }

    /// <summary>
    /// Triggers a battle when the player collides with an enemy.
    /// </summary>
    /// <param name="other">Collider that the player has collided with.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && !_battleStarted)
        {
            _battleStarted = true; // Set flag to indicate battle has started
            GameManager.Instance.StartBattle(); // Start the battle
        }
    }
}
