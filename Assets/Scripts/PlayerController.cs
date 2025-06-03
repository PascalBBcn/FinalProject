using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 20;
    public Rigidbody2D rb;
    public InputAction playerControls;
    Vector2 direction = Vector2.zero;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        direction = playerControls.ReadValue<Vector2>();
        rb.velocity = new Vector2(direction.x * movementSpeed, direction.y * movementSpeed);
    }



    
}


