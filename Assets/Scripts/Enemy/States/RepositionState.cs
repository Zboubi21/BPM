﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class RepositionState : IState
{
    EnemyController m_enemyController;


    /// Le NPC se reposition en fonction de la position du Player
    public RepositionState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }
    GameObject go;

    public void Enter()
    {
        m_enemyController.CurrentTarget = m_enemyController.FindBestSpotsInRangeOfTarget(m_enemyController.Player);
        go = m_enemyController.OnInstantiate(m_enemyController._debug.m_destinationImage, m_enemyController.CurrentTarget);
    }

    public void Exit()
    {
        m_enemyController.DestroyObj(go);
    }

    public void FixedUpdate()
    {
        m_enemyController.Agent.SetDestination(m_enemyController.CurrentTarget);
        if(m_enemyController.DistanceToTarget <= m_enemyController.Agent.stoppingDistance && !m_enemyController.Cara.IsDead)
        {
            m_enemyController.ChangeState((int)EnemyState.Enemy_AttackState);
        }
    }

    public void LateUpdate()
    {
    }

    public void Update()
    {
    }

    
}
