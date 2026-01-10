using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControls controls;

    [Header("Aim Visual - Laser")]
    [SerializeField] private LineRenderer aimLaser;

    [Header("Aim control")]
    [SerializeField] private Transform aim;

    [SerializeField] private bool isAimingPrecisly;
    [SerializeField] private bool isLockingToTarget;

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

        if (Input.GetKeyDown(KeyCode.L))
            isLockingToTarget = !isLockingToTarget;

        UpdateAimVisuals();
        UpdateAimPosition();
        UpdateCameraPosition();
    }

    private void UpdateAimVisuals()
    {
        Transform gunPoint = player.weapon.GunPoint();
        Vector3 laserDirection = player.weapon.BulletDirection();

        float laserTipLength = .5f;
        float gunDistance = 4f;

        // Line Rendererの位置を銃口にセット
        // 指定した距離の分だけ伸ばす
        Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

        if(Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance))
        {
            endPoint = hit.point;
            laserTipLength = 0f;
        }

        aimLaser.SetPosition(0, gunPoint.position);
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength);

    }
    private void UpdateAimPosition()
    {

        
        Transform target = Target();
        if (target != null && isLockingToTarget)
        {
            aim.position = target.position;
            Debug.Log("target");
            return;
        }

        aim.position = GetMouseHitInfo().point;

        // Obstacleなどにエイムが乗っても、高さを考慮して打つかどうかのフラグ
        if (isAimingPrecisly == false)
            aim.position = new Vector3(aim.position.x, transform.position.y + 1, aim.position.z);
    }





    public Transform Target()
    {
        Transform target = null;

        // マウスの位置にTargetがいた場合、ターゲットにポジションを固定させて当てやすくしている
        if(GetMouseHitInfo().transform.GetComponent<Target>() != null)
        {
            target = GetMouseHitInfo().transform;
        }

        return target;
    }
    public Transform Aim() => aim;

    public bool CanAimPrecisly() => isAimingPrecisly;



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

    #region Camera Region
    private void UpdateCameraPosition()
    {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, DesiredCameraPosition(), cameraSensetivity * Time.deltaTime);
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

    #endregion

    private void AssignInputEvents()
    {

        controls = player.controls;

        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => aimInput = Vector2.zero;


    }

}
