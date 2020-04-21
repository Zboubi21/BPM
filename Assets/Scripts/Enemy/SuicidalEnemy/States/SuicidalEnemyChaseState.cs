using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuicidalEnemyStateEnum;

public class SuicidalEnemyChaseState : IState
{

#region Constructorv
    SuicidalEnemyController m_enemyController;
    public SuicidalEnemyChaseState(SuicidalEnemyController enemyController)
    {
        m_enemyController = enemyController;
    }
#endregion

    public void Enter()
    {
        m_enemyController.SetAnimation("Chase");
    }

    public void FixedUpdate()
    {
        m_enemyController.ChasePlayer();
        if (m_enemyController.GetPlayerDistance() <= m_enemyController.m_selfDestruction.m_startWaitToExplodeRange.m_range)
        {
            m_enemyController.ChangeState(EnemyState.SelfDestructionState);
        }
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
