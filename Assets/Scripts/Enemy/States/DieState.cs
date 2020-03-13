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
        m_enemyController.Agent.isStopped = true;
        m_enemyController.KillNPC(2f);   //Send back to pool
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
