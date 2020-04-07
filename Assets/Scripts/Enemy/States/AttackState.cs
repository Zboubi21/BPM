using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class AttackState : IState
{

    EnemyController m_enemyController;
    WeaponEnemyBehaviour m_weaponEnemyBehaviour;
    public Transform from;
    public Transform to;

    float timeCount = 0.0f;
    float _currentTime;
    Vector3 relativePos;
    Vector3 initForward;
    Transform lastFrameTargetPos;

    public AttackState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }


    public void Enter()
    {
        m_weaponEnemyBehaviour = m_enemyController.GetComponent<WeaponEnemyBehaviour>();

        ///Play attack animation
        m_enemyController.Anim.SetTrigger("Aim");

        ///Play attack sound (enter state not shoot)

        relativePos = m_enemyController.Player.position - m_enemyController.transform.position;
        initForward = m_enemyController.transform.forward;
        lastFrameTargetPos = m_enemyController.Player;

        m_enemyController.Agent.isStopped = true;

        timeCount = 0;
        _currentTime = 0;
        go = true;
        time = 0;

    }

    public void Exit()
    {
        m_weaponEnemyBehaviour.StopCoroutine(m_weaponEnemyBehaviour.OnEnemyShoot(m_weaponEnemyBehaviour._attack.nbrOfShootOnRafale, m_weaponEnemyBehaviour._attack.timeBetweenEachBullet, m_weaponEnemyBehaviour._attack.minTimeBetweenEachBurst, m_weaponEnemyBehaviour._attack.maxTimeBetweenEachBurst, lastFrameTargetPos));
        m_enemyController.Agent.isStopped = false;
        //m_enemyController.Agent.updateRotation = true;
    }

    public void FixedUpdate()
    {

    }

    public void LateUpdate()
    {
    }
    bool go;
    public void Update()
    {
        if (RotateTowards(lastFrameTargetPos) && go)
        {
            go = false;
            m_weaponEnemyBehaviour.StartCoroutine(m_weaponEnemyBehaviour.OnEnemyShoot(m_weaponEnemyBehaviour._attack.nbrOfShootOnRafale, m_weaponEnemyBehaviour._attack.timeBetweenEachBullet, m_weaponEnemyBehaviour._attack.minTimeBetweenEachBurst, m_weaponEnemyBehaviour._attack.maxTimeBetweenEachBurst, lastFrameTargetPos));
        }
        /*if (m_enemyController.DistanceToTarget > m_enemyController.Agent.stoppingDistance)
        {
            m_enemyController.ChangeState((int)EnemyState.Enemy_ChaseState);
        }*/
        /*if(m_enemyController.PlayerController.AllPreviousPos[m_enemyController.Cara.CurrentIndexInLateLookAt] != null)
        {
            m_enemyController.transform.LookAt(m_enemyController.PlayerController.AllPreviousPos[m_enemyController.Cara.CurrentIndexInLateLookAt]);
        }
        else
        {
            Debug.LogError("You didn't wait long enough, the player records 5 seconds of its movement, if you spawn enemies before 5 seconds they won't know at what to look at");
        }*/
        //LateLookAt();

    }
    /* void LateLookAt()
     {
         Quaternion rotation;
         if (m_enemyController.Player.position == lastFrameTargetPos)
         {
             if (_currentTime < m_enemyController.Cara._enemyCaractéristique._move.timeOfLateLookAt)
             {
                 _currentTime += Time.deltaTime;
             }
             else
             {
                 _currentTime = m_enemyController.Cara._enemyCaractéristique._move.timeOfLateLookAt;
             }
             rotation = Quaternion.LookRotation(Vector3.Slerp(initForward, relativePos, Mathf.InverseLerp(0, m_enemyController.Cara._enemyCaractéristique._move.timeOfLateLookAt, _currentTime)));

         }
         else
         {
             relativePos = m_enemyController.Player.position - m_enemyController.transform.position;
             initForward = m_enemyController.transform.forward;
             rotation = Quaternion.LookRotation(Vector3.Slerp(initForward, relativePos, (1 / m_enemyController.Cara._enemyCaractéristique._move.timeOfLateLookAt) * Time.deltaTime));
             _currentTime = 0;
         }
         lastFrameTargetPos = m_enemyController.Player.position;
         m_enemyController.transform.eulerAngles = new Vector3(m_enemyController.transform.eulerAngles.x, rotation.eulerAngles.y, m_enemyController.transform.eulerAngles.z);
     }*/
    float time;
    private bool RotateTowards(Transform target)
    {
        time += Time.deltaTime * (m_enemyController.Cara.rotationSpeed/100);
        if(time >= 1)
        {
            return true;
        }
        else
        {
            Vector3 direction = (target.position - m_enemyController.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            m_enemyController.transform.rotation = Quaternion.Slerp(m_enemyController.transform.rotation, lookRotation, time);
            return false;
        }
    }

}
