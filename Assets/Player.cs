using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerControls controls;
    private CharacterController characterController;

    [Header("Movement info")]
    public float walkSpeed;
    public Vector3 movementDirection;

    private Vector2 moveInput;
    private Vector2 aimInput;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context => moveInput = Vector2.zero;

        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);

        // Magnitude: ベクトルの大きさ(長さ)
        // ゼロより大きければ何らかの入力があるということ
        if (movementDirection.magnitude > 0)
        {
            characterController.Move(movementDirection * Time.deltaTime * walkSpeed);
        }
    }

    private void Shoot()
    {
        Debug.Log("shoot");
    }


    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }


}
