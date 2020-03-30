using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class ChaseState : IState
{
    EnemyController m_enemyController;
    WeaponEnemyBehaviour weapon;

    /// Le NPC court après le joueur pour être en range d'attaque
    public ChaseState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }
    GameObject go;

    public void Enter()
    {
        /// play run animation
        m_enemyController.AudioControl.On_Run(true);

        weapon = m_enemyController.GetComponent<WeaponEnemyBehaviour>();
        m_enemyController.CurrentTarget = m_enemyController.Player.transform.position;
#if UNITY_EDITOR
        go = m_enemyController.OnInstantiate(m_enemyController._debug.m_destinationImage, m_enemyController.Player.transform.position);
#endif
    }

    public void Exit()
    {
#if UNITY_EDITOR
        m_enemyController.DestroyObj(go);
#endif
        m_enemyController.AudioControl.On_Run(false);

    }

    public void FixedUpdate()
    {
    }

    public void LateUpdate()
    {
    }
    RaycastHit _hit;
    public void Update()
    {
        float distance = 0;
        if (Vector3.Distance(PlayerController.s_instance.transform.position, m_enemyController.transform.position) < weapon._attack.rangeRadius)
        {
            distance = Vector3.Distance(PlayerController.s_instance.transform.position, m_enemyController.transform.position);
        }
        else
        {
            distance = weapon._attack.rangeRadius;
        }
        m_enemyController.Agent.SetDestination(m_enemyController.CurrentTarget);
        if(m_enemyController.DistanceToTarget <= m_enemyController.WeaponBehavior._attack.rangeRadius && !m_enemyController.Cara.IsDead && !Physics.Raycast(weapon._SMG.firePoint.transform.position, weapon._SMG.firePoint.transform.forward, out _hit, distance, 11))
        {
            m_enemyController.Agent.SetDestination(m_enemyController.transform.position);
            m_enemyController.ChangeState((int)EnemyState.Enemy_AttackState);
        }
    }

    
}
