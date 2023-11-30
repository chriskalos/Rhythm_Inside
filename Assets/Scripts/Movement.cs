using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    private Rigidbody _rigidbody;
    private Vector3 _inputVector;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float dx = Input.GetAxisRaw("Horizontal");
        float dy = _rigidbody.velocity.y; // Keep the existing vertical velocity
        float dz = Input.GetAxisRaw("Vertical");

        _inputVector = new Vector3(dx, 0, dz).normalized;
        
        // Check if sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _inputVector *= sprintSpeed;
        }
        else
        {
            _inputVector *= walkSpeed;
        }
        
        // Apply the existing y velocity (falling or jumping)
        _inputVector.y = dy;

        // Flip the character if moving left or right
        if (dx != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(dx), transform.localScale.y, transform.localScale.z);
        }
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = _inputVector * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + moveDirection);
    }
}