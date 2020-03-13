using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class IdleState : IState
{
    EnemyController m_enemyController;

    public IdleState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public void Enter()
    {
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
