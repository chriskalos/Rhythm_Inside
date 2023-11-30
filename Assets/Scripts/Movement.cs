using System.Net.NetworkInformation;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    private CharacterController _characterController;
    private Rigidbody _rigidbody;
    private Vector3 _inputVector;
    private Vector3 _velocity; // Variable to store the vertical velocity

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float dx = Input.GetAxisRaw("Horizontal");
        float dz = Input.GetAxisRaw("Vertical");

        _inputVector = new Vector3(dx, 0, dz).normalized;

        // Check if sprinting
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        Vector3 moveDirection = _inputVector * currentSpeed;
        
        _velocity.y += Physics.gravity.y * Time.deltaTime; // Apply gravity
        
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