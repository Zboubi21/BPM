using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class SpawnState : IState
{
    
    float m_timer = 0;
    bool m_timeIsDone = false;

    EnemyController m_enemyController;
    public SpawnState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public void Enter()
    {
        m_timer = 0;
        m_timeIsDone = false;
        m_enemyController.ActivateEnemyColliders(false);
    }

    public void FixedUpdate()
    {
        if (!m_timeIsDone)
            m_timer += Time.deltaTime;
        
        if (m_timer >m_enemyController.m_spawn.m_timeToSpawn && !m_timeIsDone)
        {
            m_timeIsDone = true;
            m_enemyController.ChangeState((int)EnemyState.Enemy_RepositionState);
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
        m_enemyController.ActivateEnemyColliders(true);
    }

}
