using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;

    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Transform aim;

    private void Start()
    {
        player = GetComponent<Player>();
        player.controls.Character.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {

        GameObject newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        newBullet.GetComponent<Rigidbody>().velocity = BulletDirection() * bulletSpeed;
        Destroy(newBullet, 10);

        GetComponentInChildren<Animator>().SetTrigger("Fire");

    }

    // 弾丸の方向決定
    private Vector3 BulletDirection()
    {
        // direction = aim - gunPoint
        // ベクトルは、終点 - 始点で表せる。なので、マウスポジションに向かってgunPointから伸びるベクトルを表す
        // normalizedで長さを1にすることで単位ベクトルになり、向きだけ取り出せる。
        // 出てきた単位ベクトルのdirectionを、後でbulletSpeedをかけて、大きさを加えてやる。
        Vector3 direction = (aim.position - gunPoint.position).normalized;

        if (player.aim.CanAimPrecisly() == false)
            direction.y = 0; // 上下方向を無視して水平に飛ばすようにしている

        weaponHolder.LookAt(aim);
        gunPoint.LookAt(aim);

        return direction;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + weaponHolder.forward * 25);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(gunPoint.position, gunPoint.position + BulletDirection() * 25);
    }
}
