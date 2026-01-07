using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVisualController : MonoBehaviour
{
    [SerializeField] private Transform[] gunTransforms;

    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform autoRifle;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform rifle;

    private Transform currentGun;

    [Header("Left hand IK")]
    [SerializeField] private Transform leftHand;

    private void Start()
    {
        SwitchOn(pistol);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchOn(pistol);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchOn(revolver);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchOn(autoRifle);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SwitchOn(shotgun);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            SwitchOn(rifle);
    }

    private void SwitchOn(Transform gunTransform)
    {
        SwitchOffGuns();
        gunTransform.gameObject.SetActive(true);
        currentGun = gunTransform;
        AttachLeftHand();
    }

    private void SwitchOffGuns()
    {
        for (int i = 0; i < gunTransforms.Length; i++)
        {
            gunTransforms[i].gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = currentGun.GetComponentInChildren<LeftHandTargetTransform>().transform;
        leftHand.localPosition = targetTransform.localPosition;
        leftHand.localRotation = targetTransform.localRotation;
    }
}
