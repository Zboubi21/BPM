using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuicidalEnemyStateEnum;

public class SuicidalEnemySpawnState : IState
{
    
    float m_timer = 0;
    bool m_timeIsDone = false;

#region Constructor
    SuicidalEnemyController m_enemyController;
    public SuicidalEnemySpawnState(SuicidalEnemyController enemyController)
    {
        m_enemyController = enemyController;
    }
#endregion

    public void Enter()
    {
        m_timer = 0;
        m_timeIsDone = false;
    }

    public void FixedUpdate()
    {
        if (!m_timeIsDone)
            m_timer += Time.deltaTime;
        
        // if (m_timer >m_enemyController.m_spawn.m_timeToSpawn && !m_timeIsDone)
        // {
        //     m_timeIsDone = true;
        //     m_enemyController.ChangeState(SuicidalEnemyState.ChaseState);
        // }
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
