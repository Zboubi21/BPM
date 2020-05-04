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
        Vector3 direction = (PlayerController.s_instance.transform.position - m_enemyController.transform.position).normalized;
        m_enemyController.Anim.SetLayerWeight(2, 1);
        
        m_enemyController.StartCoroutine(m_enemyController.IsStun(m_enemyController.Cara.CurrentTimeForStun, EnemyState.Enemy_StunState));
    }

    public void Exit()
    {
        m_enemyController.Agent.isStopped = false;
        m_enemyController.Anim.SetLayerWeight(2, 0);
        
        m_enemyController.Anim.SetBool("IsLifeLow", false);
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
