﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFSM : ActorFSM
{

    protected Monster currentMonster;


    public override AI CurrentAI
    {
        get
        {
            return currentMonster;
        }
    }

    protected override void HandleCombatAction()
    {
        animator.SetBool("Attacking", true);
    }

    protected override void UpdatePetrolState()
    {
        base.UpdatePetrolState();
        Vector3 playerPos = Player.Instance.transform.position;
        playerPos.y = transform.position.y;
        if(Vector3.Distance(transform.position,playerPos) <= detectionDistance && Player.Instance.CanBeAttacked)
        {
            target = Player.Instance;
            ChangeState(FSMState.COMBAT);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        currentMonster = GetComponent<Monster>();
    }
    protected override void UpdateIdleState()
    {
        if (pathFound)
        {
            ChangeState(FSMState.PETROL);
        }
        else if (!requestedPath)
        {
            Transform targetTrans = TownManager.Instance.CurrentTown.Treasure;
            if (targetTrans != null)
            {
                target = targetTrans.GetComponent<IDamagable>();
                AStarManager.Instance.RequestPath(transform.position, target.transform.position, ChangePath);
                requestedPath = true;
                nextState = FSMState.COMBAT;
            }
            else
            {
                currentMonster.TakeDamage(999999, null);
            }
            
        }
    }


}
