using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuicidalEnemyStateEnum;

public class SuicidalEnemyChaseState : IState
{

#region Constructor
    SuicidalEnemyController m_enemyController;
    public SuicidalEnemyChaseState(SuicidalEnemyController enemyController)
    {
        m_enemyController = enemyController;
    }
#endregion

    bool m_checkTimer = false;
    float m_timer = 0;
    bool m_timerIsDone = false;

    public void Enter()
    {
        m_timer = 0;
        m_timerIsDone = false;
        m_enemyController.SetAnimation("Chase");
    }

    public void FixedUpdate()
    {
        m_enemyController.ChasePlayer();

        if (m_enemyController.EnemyInClosedRangeOfPlayer())
                m_enemyController.ChangeState(EnemyState.SelfDestructionState);

        if (m_enemyController.EnemyInRangeOfPlayer())
        {
            m_checkTimer = true;
        }  
        else
        {
            m_checkTimer = false;
            m_timer = 0;
        }

        if (m_checkTimer)
        {
            m_timer += Time.deltaTime;
            if (m_timer > m_enemyController.m_selfDestruction.m_waitTimeToStartSelfDestruction && !m_timerIsDone)
            {
                m_timerIsDone = true;
                m_enemyController.ChangeState(EnemyState.SelfDestructionState);
            }
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
