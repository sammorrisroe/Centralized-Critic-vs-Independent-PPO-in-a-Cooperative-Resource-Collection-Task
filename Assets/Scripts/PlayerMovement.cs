using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    private InputAction moveAction;
    private Rigidbody rb;

    private bool isCarrying = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void OnEnable()
    {
        if (moveAction != null)
            moveAction.Enable();
    }

    void OnDisable()
    {
        if (moveAction != null)
            moveAction.Disable();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0f, input.y);

        rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Collect resource automatically
        if (other.CompareTag("Resource") && !isCarrying)
        {
            isCarrying = true;
            Destroy(other.gameObject);

            Debug.Log("Collected resource");
        }

        // Deposit automatically
        if (other.CompareTag("Goal") && isCarrying)
        {
            isCarrying = false;

            Debug.Log("Deposited resource");
            
        }
    }

    public bool IsCarrying()
    {
        return isCarrying;
    }
}