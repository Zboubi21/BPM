using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class DefensiveState : IState
{
    EnemyController m_enemyController;

    public DefensiveState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    GameObject go;


    public void Enter()
    {
        ///play run animation
        m_enemyController.Anim.SetTrigger("Run");

        m_enemyController.AudioControl.On_Run(true);

        m_enemyController.CurrentTarget = m_enemyController.MoveBackward(m_enemyController.Player);
#if UNITY_EDITOR
        go = m_enemyController.OnInstantiate(m_enemyController._debug.m_destinationImage, m_enemyController.CurrentTarget);
#endif
    }

    public void Exit()
    {
#if UNITY_EDITOR
        m_enemyController.DestroyObj(go);
#endif
        m_enemyController.AudioControl.On_Run(false);

    }

    public void FixedUpdate()
    {
    }

    public void LateUpdate()
    {
    }

    public void Update()
    {
        m_enemyController.Agent.SetDestination(m_enemyController.CurrentTarget);
        if ((m_enemyController.DistanceToTarget <= m_enemyController.Agent.stoppingDistance || m_enemyController.DistanceToPlayer <= m_enemyController.WeaponBehavior._attack.rangeOfAttackNoMatterWhat) && !m_enemyController.Cara.IsDead)
        {
            m_enemyController.ChangeState((int)EnemyState.Enemy_AttackState);
        }
        m_enemyController.gameObject.transform.LookAt(m_enemyController.Player);
    }
}
