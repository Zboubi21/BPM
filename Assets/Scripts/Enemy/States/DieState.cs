using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class DieState : IState
{
    EnemyController m_enemyController;

    public DieState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public void Enter()
    {
        /// play dying animation
        /// play dying sound
        m_enemyController.Agent.isStopped = true;
        m_enemyController.KillNPC(2f);   //Send back to pool with animation time
    }

    public void Exit()
    {
    }

    public void FixedUpdate()
    {
    }

    public void LateUpdate()
    {
    }

    public void Update()
    {
    }
}
