using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerWeaponVisuals visualController;

    private void Start()
    {
        visualController = GetComponentInParent<PlayerWeaponVisuals>();
    }

    public void ReloadIsOver()
    {
        visualController.MaximizeRigWeight();

        // リロードが完了したら、弾丸を増やすような処理を書く
    }

    // 武器切り替え
    public void WeaponGrabIsOver()
    {


        visualController.SetBusyGrabbingWeaponTo(false);
    }

    public void ReturnRig()
    {
        visualController.MaximizeRigWeight();
        visualController.MaximizeLeftHandWeight(); // 左手も操作するので、そのリグの調整(再び銃に手を添える)
    }


}
