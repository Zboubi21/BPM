using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class StunState : IState
{

    EnemyController m_enemyController;

    public StunState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public void Enter()
    {
        /// play stun animation
        /// play stun sound
        /// play stun FX
        m_enemyController.Agent.isStopped = true;
        m_enemyController.StartCoroutine(m_enemyController.IsStun());
    }

    public void Exit()
    {
        m_enemyController.Agent.isStopped = false;
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
