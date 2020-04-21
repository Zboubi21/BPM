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

    float m_timer = 0;
    bool m_timerIsDone = false;

    public void Enter()
    {
        m_timer = 0;
        m_timerIsDone = false;
        m_enemyController.SetAnimation("SelfDestruction");
    }

    public void FixedUpdate()
    {
        if (!m_timerIsDone)
            m_timer += Time.deltaTime;
        
        if (m_timer > m_enemyController.m_selfDestruction.m_waitTimeToExplode && !m_timerIsDone)
        {
            m_timerIsDone = true;
            m_enemyController.On_EnemyExplode();
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
