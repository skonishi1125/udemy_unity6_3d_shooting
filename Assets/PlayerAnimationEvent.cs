using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private WeaponVisualController visualController;

    private void Start()
    {
        visualController = GetComponentInParent<WeaponVisualController>();
    }

    public void ReloadIsOver()
    {
        visualController.ReturnRigWeightToOne();

        // リロードが完了したら、弾丸を増やすような処理を書く
    }

    // 武器切り替え
    public void WeaponGrabIsOver()
    {


        visualController.SetBusyGrabbingWeaponTo(false);
    }

    public void ReturnRig()
    {
        visualController.ReturnRigWeightToOne();
        visualController.ReturnWeightToLeftHandIK(); // 左手も操作するので、そのリグの調整(再び銃に手を添える)
    }


}
