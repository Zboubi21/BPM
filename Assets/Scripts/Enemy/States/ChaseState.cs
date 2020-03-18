﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class ChaseState : IState
{
    EnemyController m_enemyController;


    /// Le NPC court après le joueur pour être en range d'attaque
    public ChaseState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }
    GameObject go;

    public void Enter()
    {
        m_enemyController.CurrentTarget = m_enemyController.Player.transform.position;
        go = m_enemyController.OnInstantiate(m_enemyController._debug.m_destinationImage, m_enemyController.Player.transform.position);
    }

    public void Exit()
    {
        m_enemyController.DestroyObj(go);
    }

    public void FixedUpdate()
    {
    }

    public void LateUpdate()
    {
    }

    public void Update()
    {
        m_enemyController.Agent.SetDestination(m_enemyController.Player.transform.position);
        if(m_enemyController.DistanceToTarget <= m_enemyController.WeaponBehavior._attack.rangeRadius && !m_enemyController.Cara.IsDead)
        {
            //m_enemyController.Agent.SetDestination(m_enemyController.transform.position);
            m_enemyController.ChangeState((int)EnemyState.Enemy_AttackState);
        }
    }

    
}
