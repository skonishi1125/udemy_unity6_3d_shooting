using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;

    [Header("Aim control")]
    [SerializeField] private Transform aim;

    [SerializeField] private bool isAimingPrecisly;


    [Header("Camera Control")]
    [SerializeField] private Transform cameraTarget;
    [Range(.5f, 1f)]
    [SerializeField] private float minCameraDistance = 1.5f;
    [Range(1, 3f)]
    [SerializeField] private float maxCameraDistance = 4f;
    [Range(3f, 5f)]
    [SerializeField] private float cameraSensetivity = 5f;

    [Space]
    [SerializeField] private LayerMask aimLayerMask;
    private Vector2 aimInput;
    private RaycastHit lastKnownMouseHit;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            isAimingPrecisly = !isAimingPrecisly;

        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensetivity * Time.deltaTime);
    }

    private void UpdateAimPosition()
    {
        aim.position = GetMouseHitInfo().point;

        // Obstacleなどにエイムが乗っても、高さを考慮して打つかどうかのフラグ
        if (isAimingPrecisly == false)
            aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z);
    }

    public bool CanAimPrecisly()
    {
        if (isAimingPrecisly)
            return true;

        return false;
    }


    private Vector3 DesiredCameraPosition()
    {

        float actualMaxCameraDistance = player.movement.moveInput.y < -.5f ? minCameraDistance : maxCameraDistance;


        // マウス位置
        // プレイヤーとマウスの向きを、0-1で正規化して出す。
        Vector3 desiredCameraPosition = GetMouseHitInfo().point;
        Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

        // プレイヤーとマウスの距離
        float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition);

        float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, minCameraDistance, actualMaxCameraDistance);
        desiredCameraPosition = transform.position + aimDirection * clampedDistance;

        // 設定した最大値, 最小値の閾値を超える場合は調節する
        // エイム位置にカメラが合うようになっているので、
        // プレイヤーとエイムの最大値を調整してが画面外から出てしまうのを防いでいる。
        // (今回は単純にclampで表したので、コメントアウトした）
        //if (distanceToDesiredPosition > maxCameraDistance)
        //{
        //    desiredAimPosition = transform.position + aimDirection * maxCameraDistance;

        //}
        //else if (distanceToDesiredPosition < minCameraDistance)
        //{
        //    desiredAimPosition = transform.position + aimDirection * minCameraDistance;

        //}

        desiredCameraPosition.y = transform.position.y + 1;


        return desiredCameraPosition;
    }

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lastKnownMouseHit = hitInfo;
            return hitInfo;
        }

        return lastKnownMouseHit;

    }

    private void AssignInputEvents()
    {

        controls = player.controls;

        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => aimInput = Vector2.zero;


    }

}
