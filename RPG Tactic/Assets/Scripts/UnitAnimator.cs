using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform knifeTransform;

    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
            moveAction.OnChangedFloorsStarted += MoveAction_OnChangedFloorsStarted;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }

        if (TryGetComponent<KnifeAction>(out KnifeAction swordAction))
        {
            swordAction.OnKnifeActionStarted += KnifeAction_OnKnifeActionStarted;
            swordAction.OnKnifeActionCompleted += KnifeAction_OnKnifeActionCompleted;
        }

    }

    private void MoveAction_OnChangedFloorsStarted(object sender, MoveAction.OnChangeFloorsStartedEventArgs e)
    {
        if (e.targetGridPosition.floor > e.unitGridPosition.floor)
        {
            animator.SetTrigger("JumpUp");
        }
        else
        {
            animator.SetTrigger("JumpDown");
        }
    }

    private void Start()
    {
        EquipRifle();
    }

    private void KnifeAction_OnKnifeActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }

    private void KnifeAction_OnKnifeActionStarted(object sender, EventArgs e)
    {
        EquipSword();
        animator.SetTrigger("KnifeSlash");
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("isWalking", true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("isWalking", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Shoot");

        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);

        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();
      
        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();

        //targetUnitShootAtPosition.y = shootPointTransform.position.y;
        float unitShoulderHeight = 1.7f;
        targetUnitShootAtPosition.y += unitShoulderHeight;


        bulletProjectile.Setup(targetUnitShootAtPosition);
    }

    private void EquipSword()
    {
        knifeTransform.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
    }

    private void EquipRifle()
    {
        knifeTransform.gameObject.SetActive(false);
        rifleTransform.gameObject.SetActive(true);
    }
}
