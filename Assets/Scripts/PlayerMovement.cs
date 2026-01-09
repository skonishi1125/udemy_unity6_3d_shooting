using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player player;

    private PlayerControls controls;
    private CharacterController characterController;
    private Animator animator;

    [Header("Movement info")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float turnSpeed;
    private float speed;
    private float verticalVelocity;

    public Vector3 movementDirection;
    [SerializeField] private float gravityScale = 9.81f;
    private Vector2 moveInput;
    private bool isRunning;

    private void Start()
    {
        player = GetComponent<Player>();

        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed;

        AssignInputEvents();

    }

    private void Update()
    {
        ApplyMovement();
        ApplyRotation();
        AnimatorController();
    }



    private void AnimatorController()
    {
        /* 
            ベクトル
            movementDirection.normalized
                移動の向きだけ取得(長さ1のベクトル)
            transform.right / forward
                キャラクタ基準の右と前の座標（ローカルx+と、ローカルz+）のこと
                つまりキャラが回ると、この軸も回る
            Vector3.Dot(a,b)
                aとbの内積。cosθはベクトルa・ベクトルb / |a| |b|で求められる。
                単位ベクトル同士の場合、分母が1なので、結果的にcosθが返る。
                つまり -1 : 180°(真逆) 0: 90°(直交) +1: 0°(真正面) という結果になる
            xVelocity
                移動方向と右向きを比較。
                1なら完全に右向き 0なら右でも左でもない(右軸に直交) -1なら左向きであることを示す
            zVelocity
                移動方向と前向きを比較。
                1なら完全に真正面 0なら前でも後ろでもない(左軸に直交) -1なら後ろ向きであることを示す値となる。

         */
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

        animator.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);

        // 走行中かつ、しっかり移動ベクトルの大きさがある場合
        bool playRunAnimation = isRunning && movementDirection.magnitude > 0;
        animator.SetBool("isRunning", playRunAnimation);
    }

    private void ApplyRotation()
    {
        // Playerからポインタへ向かうベクトル
        Vector3 lookingDirection = player.aim.GetMousePosition() - transform.position;
        lookingDirection.y = 0f;
        lookingDirection.Normalize(); // 上で求めたベクトルを単位ベクトルにする(方向だけ取り出す)

        //transform.forward = lookingDirection;

        // 向きたい、目的の方向
        Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection);
        // 目的方向にSlerpで、turnSpeed * deltaTimeで徐々に近づけている。
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);

    }

    private void ApplyMovement()
    {
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();

        // Magnitude: ベクトルの大きさ(長さ)
        // ゼロより大きければ何らかの入力があるということ
        if (movementDirection.magnitude > 0)
        {
            characterController.Move(movementDirection * Time.deltaTime * speed);
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded == false)
        {
            verticalVelocity = verticalVelocity - gravityScale * Time.deltaTime;
            movementDirection.y = verticalVelocity;
        }
        else
        {
            verticalVelocity = -.5f;
        }
    }

    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context => moveInput = Vector2.zero;


        controls.Character.Run.performed += context =>
        {
            speed = runSpeed;
            isRunning = true;
        };

        controls.Character.Run.canceled += context =>
        {
            speed = walkSpeed;
            isRunning = false;
        };
    }



}
