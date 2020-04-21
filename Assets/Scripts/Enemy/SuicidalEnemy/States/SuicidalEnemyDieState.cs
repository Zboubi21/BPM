using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuicidalEnemyStateEnum;

public class SuicidalEnemyDieState : IState
{

#region Constructor
    SuicidalEnemyController m_enemyController;
    public SuicidalEnemyDieState(SuicidalEnemyController enemyController)
    {
        m_enemyController = enemyController;
    }
#endregion

    float m_timer = 0;
    bool m_timerIsDone = false;

    public void Enter()
    {
        m_enemyController.StopEnemyMovement(true);
        m_enemyController.SetAnimation("Die");
    }

    public void FixedUpdate()
    {
        if (!m_timerIsDone)
            m_timer += Time.deltaTime;
        
        if (m_timer > m_enemyController.m_waitTimeToDie && !m_timerIsDone)
        {
            m_timerIsDone = true;
            m_enemyController.On_EnemyDie();
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
