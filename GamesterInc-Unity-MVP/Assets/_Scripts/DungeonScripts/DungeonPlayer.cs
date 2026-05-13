using System;
using UnityEngine;

public class DungeonPlayer : DungeonCharacter
{
    private PlayerMovement movement;

    protected override void Start()
    {
        movement = GetComponent<PlayerMovement>();
        base.Start();
    }
    
    public void ActivatePlayer(bool pCanMove, bool resetWalk = false)
    {
        movement.canMove = pCanMove;
        if (resetWalk)
            movement.moveToPos = transform.position;
    }

    public void SetMoveToPos(Vector2 pPos)
    {
        movement.moveToPos = pPos;
        movement.isMovingToClick = true;
    }

    public void SetCombatAnimations(bool isAttacking, bool inCombat = true)
    {
        movement.animator.SetBool("inCombat", inCombat);
        if (isAttacking)
            movement.animator.SetTrigger("Attacking");
    }
}