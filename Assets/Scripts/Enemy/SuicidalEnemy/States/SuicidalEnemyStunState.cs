using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuicidalEnemyStateEnum;

public class SuicidalEnemyStunState : IState
{
    
#region Constructor
    SuicidalEnemyController m_enemyController;
    public SuicidalEnemyStunState(SuicidalEnemyController enemyController)
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
        m_enemyController.StopEnemyMovement(true);
        m_enemyController.On_EnemyStartStun(true);
    }

    public void FixedUpdate()
    {
        if (!m_timerIsDone)
            m_timer += Time.deltaTime;
        
        if (m_timer > m_enemyController.EnemyChara._enemyCaractéristique._stunResistance.timeOfStun && !m_timerIsDone)
        {
            m_timerIsDone = true;
            if (m_enemyController.LastState(EnemyState.ChaseState) || m_enemyController.LastState(EnemyState.SelfDestructionState))
                m_enemyController.ChangeState(m_enemyController.GetLastState());
            else
                m_enemyController.ChangeState(EnemyState.ChaseState);
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
        m_enemyController.StopEnemyMovement(false);
        m_enemyController.On_EnemyStartStun(false);
    }
    
}
