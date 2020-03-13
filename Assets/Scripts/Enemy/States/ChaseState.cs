using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class ChaseState : IState
{
    EnemyController m_enemyController;

    public ChaseState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }
    GameObject go;

    public void Enter()
    {
        //if(m_enemyController.Agent != null && m_enemyController.CurrentTarget != Vector3.zero)
        //{
            //Vector3 stoppingDistance = new Vector3(m_enemyController.Agent.stoppingDistance, 0.1f, m_enemyController.Agent.stoppingDistance);
        go = m_enemyController.OnInstantiate(m_enemyController._debug.m_destinationImage, m_enemyController.CurrentTarget);
        //}
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
