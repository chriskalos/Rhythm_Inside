using System.Net.NetworkInformation;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    private Animator _animator;
    private CharacterController _characterController;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _inputVector;
    private Vector3 _velocity; // Variable to store the vertical velocity

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _velocity.y = -5f; // Some downward momentum so that the character doesn't fall too slowly in the beginning

        _animator = GetComponent<Animator>();
        
        // Shadow casting stuff (I hate Unity)
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        _spriteRenderer.receiveShadows = true;
    }

    void Update()
    {
        float dx = Input.GetAxisRaw("Horizontal");
        float dz = Input.GetAxisRaw("Vertical");

        _inputVector = new Vector3(dx, 0, dz).normalized;
        
        // Determine movement state to set animation
        bool isMoving = _inputVector.magnitude > 0;
        Debug.Log(isMoving);
        Debug.Log(_inputVector.magnitude);
        bool isSprinting = isMoving && Input.GetKey(KeyCode.LeftShift);
        
        _animator.SetBool("IsWalking", isMoving && !isSprinting);
        _animator.SetBool("IsSprinting", isSprinting);

        // Check if sprinting
        float currentSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
        Vector3 moveDirection = _inputVector * currentSpeed;
        
        // Apply gravity (increasing velocity downwards)
        if (_characterController.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -5f; // Reset the downward velocity when grounded
        }
        _velocity.y += Physics.gravity.y * Time.deltaTime; // Apply gravity
        
        // Debug.Log(_characterController.isGrounded);
        // Debug.Log(_velocity.y);
        
        // Combine movement and gravity, then move the character
        _characterController.Move((moveDirection + _velocity) * Time.deltaTime);

        // Flip the character if moving left or right
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
}