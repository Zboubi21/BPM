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
        m_enemyController.On_EnemyIsStunned(true);
        
        m_enemyController.Agent.isStopped = true;
        m_enemyController.Anim.SetBool("IsStun", true);
        switch (m_enemyController.Cara.ImpactPosition)
        {
            case 0:
                m_enemyController.Anim.SetTrigger("Head_Hit");
                break;
            case 1:
                m_enemyController.Anim.SetTrigger("Body_Hit");
                break;
            case 2:
                m_enemyController.Anim.SetTrigger("Stomach_Hit");
                break;
            case 3:
                m_enemyController.Anim.SetTrigger("Rib_Hit");
                break;
            case 4:
                m_enemyController.Anim.SetTrigger("Side_Hit");
                break;
        }
        //m_enemyController.Anim.SetFloat("WhichPart", m_enemyController.Cara.ImpactPosition);
        //Vector3 direction = (PlayerController.s_instance.transform.position - m_enemyController.transform.position).normalized;
        //m_enemyController.Anim.SetLayerWeight(2, 1);

        m_enemyController.StartCoroutine(m_enemyController.IsStun(m_enemyController.Cara.CurrentTimeForStun, EnemyState.Enemy_StunState));

    }

    public void Exit()
    {
        m_enemyController.Agent.isStopped = false;
        //m_enemyController.Anim.SetLayerWeight(2, 0);
        m_enemyController.On_EnemyIsStunned(false);
        
        m_enemyController.Anim.SetBool("IsStun", false);
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
