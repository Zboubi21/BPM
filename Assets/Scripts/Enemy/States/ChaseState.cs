using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class ChaseState : IState
{
    EnemyController m_enemyController;
    WeaponEnemyBehaviour weapon;
    PlayerController playerController;

    /// Le NPC court après le joueur pour être en range d'attaque
    public ChaseState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }
    GameObject go;

    public void Enter()
    {
        ///Play run animation
        m_enemyController.Anim.SetBool("isMoving", true);
        m_enemyController.Anim.SetTrigger("Run");


        m_enemyController.AudioControl.On_Run(true);
        playerController = PlayerController.s_instance;
        weapon = m_enemyController.GetComponent<WeaponEnemyBehaviour>();
        m_enemyController.CurrentTarget = m_enemyController.ChaseATarget(m_enemyController.Player);
        m_enemyController.StartCoroutine(m_enemyController.MaxTimeInThatState(m_enemyController.maxTimeInStates , EnemyState.Enemy_ChaseState));

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
        m_enemyController.StopCoroutine(m_enemyController.MaxTimeInThatState(m_enemyController.maxTimeInStates, EnemyState.Enemy_ChaseState));

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
        //float distance = 0;
        //if (Vector3.Distance(PlayerController.s_instance.transform.position, m_enemyController.transform.position) < weapon._attack.rangeRadius)
        //{
        //    distance = Vector3.Distance(PlayerController.s_instance.transform.position, m_enemyController.transform.position);
        //}
        //else
        //{
        //    distance = weapon._attack.rangeRadius;
        //}
        m_enemyController.Agent.SetDestination(m_enemyController.CurrentTarget);
        m_enemyController.AnimationBlendTree();
        if((m_enemyController.DistanceToPlayer <= m_enemyController.WeaponBehavior._attack.rangeRadius || m_enemyController.DistanceToPlayer <= m_enemyController.WeaponBehavior._attack.rangeOfAttackNoMatterWhat) && !m_enemyController.Cara.IsDead && !Physics.Linecast(new Vector3(weapon.transform.position.x, weapon.transform.position.y+1f, weapon.transform.position.z), new Vector3(playerController.transform.position.x, playerController.transform.position.y + 1f, playerController.transform.position.z), out _hit, weapon.hittedLayer))
        {
            //Debug.DrawLine(new Vector3(weapon.transform.position.x, weapon.transform.position.y + 1f, weapon.transform.position.z), new Vector3(playerController.transform.position.x, playerController.transform.position.y + 1f, playerController.transform.position.z), Color.green, 1f);
            m_enemyController.Agent.SetDestination(m_enemyController.transform.position);
            m_enemyController.ChangeState((int)EnemyState.Enemy_AttackState);
        }
        else if(m_enemyController.DistanceToTarget <= m_enemyController.Agent.stoppingDistance )
        {
            m_enemyController.ChangeState((int)EnemyState.Enemy_ChaseState);
        }
        //else if (Physics.Linecast(new Vector3(weapon.transform.position.x, weapon.transform.position.y + 1f, weapon.transform.position.z), new Vector3(playerController.transform.position.x, playerController.transform.position.y + 1f, playerController.transform.position.z), out _hit, weapon.hittedLayer))
        //{
        //    //Debug.DrawLine(new Vector3(weapon.transform.position.x, weapon.transform.position.y + 1f, weapon.transform.position.z), new Vector3(playerController.transform.position.x, playerController.transform.position.y + 1f, playerController.transform.position.z), Color.red);
        //}

    }


}
