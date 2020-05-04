using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class ElectricalStunState : IState
{

    EnemyController m_enemyController;

    bool hasAlreadyBeenStuned;

    public ElectricalStunState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }

    public void Enter()
    {
        /// play elec stun animation
        /// play elec stun sound
        /// play elec stun FX
        m_enemyController.Agent.isStopped = true;
        m_enemyController.StartCoroutine(m_enemyController.IsStun(m_enemyController.Cara.CurrentTimeForElectricalStun, EnemyState.Enemy_ElectricalStunState));
        if (!hasAlreadyBeenStuned)
        {
            GameManager.Instance.CountElectricalyStunEnemy();
            hasAlreadyBeenStuned = true;
        }
    }

    public void Exit()
    {
        m_enemyController.Anim.SetBool("IsStun", false);
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
