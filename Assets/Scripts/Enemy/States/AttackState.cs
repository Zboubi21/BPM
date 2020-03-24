using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class AttackState : IState
{

    EnemyController m_enemyController;
    WeaponEnemyBehaviour m_weaponEnemyBehaviour;

    public AttackState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }


    public void Enter()
    {
        ///Play attack animation
        ///Play attack sound (enter state not shoot)

        m_weaponEnemyBehaviour = m_enemyController.GetComponent<WeaponEnemyBehaviour>();
        m_weaponEnemyBehaviour.StartCoroutine(m_weaponEnemyBehaviour.OnEnemyShoot(m_weaponEnemyBehaviour._attack.nbrOfShootOnRafale, m_weaponEnemyBehaviour._attack.timeBetweenEachBullet, m_weaponEnemyBehaviour._attack.timeBetweenEachBurst));

    }

    public void Exit()
    {
        m_weaponEnemyBehaviour.StopCoroutine(m_weaponEnemyBehaviour.OnEnemyShoot(m_weaponEnemyBehaviour._attack.nbrOfShootOnRafale, m_weaponEnemyBehaviour._attack.timeBetweenEachBullet, m_weaponEnemyBehaviour._attack.timeBetweenEachBurst));
    }

    public void FixedUpdate()
    {

    }

    public void LateUpdate()
    {
    }

    public void Update()
    {
        /*if (m_enemyController.DistanceToTarget > m_enemyController.Agent.stoppingDistance)
        {
            m_enemyController.ChangeState((int)EnemyState.Enemy_ChaseState);
        }*/
        m_enemyController.gameObject.transform.LookAt(m_enemyController.Player);
    }

}
