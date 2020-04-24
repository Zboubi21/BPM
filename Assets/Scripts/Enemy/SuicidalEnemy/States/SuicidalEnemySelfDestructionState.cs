using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuicidalEnemyStateEnum;

public class SuicidalEnemySelfDestructionState : IState
{

#region Constructor
    SuicidalEnemyController m_enemyController;
    public SuicidalEnemySelfDestructionState(SuicidalEnemyController enemyController)
    {
        m_enemyController = enemyController;
    }
#endregion

    public void Enter()
    {
        m_enemyController.SetAnimation("SelfDestruction");
        m_enemyController.SetEnemyAgentSpeed(m_enemyController.m_explosionMoveSpeed);
        m_enemyController.On_EnemyEnterInSelfDestructionState();
    }

    public void FixedUpdate()
    {
        m_enemyController.ChasePlayer();
    }

    public void Update()
    {
    }

    public void LateUpdate()
    {
    }

    public void Exit()
    {
    }

}
