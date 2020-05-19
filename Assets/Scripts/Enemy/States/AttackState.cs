using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class AttackState : IState
{

    EnemyController m_enemyController;
    WeaponEnemyBehaviour m_weaponEnemyBehaviour;
    Vector3 lastFrameTargetPos;

    Vector3 direction;
    Vector3 torsoDirection;

    Quaternion lookRotation;
    Quaternion torsoLookRotation;
    
    float angularDistance;
    //float torsoAngularDistance;

    float time;
    float maxTime;

    float agentSpeed;

    bool go;

    public AttackState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
        m_weaponEnemyBehaviour = m_enemyController.GetComponent<WeaponEnemyBehaviour>();
    }

    public void Enter()
    {

        ///Play attack animation
        
        m_enemyController.Anim.SetBool("isMoving", false);
        m_enemyController.Anim.SetTrigger("Shoot");

        ///Play attack sound (enter state not shoot)

        lastFrameTargetPos = new Vector3(PlayerController.s_instance.gameObject.transform.position.x, PlayerController.s_instance.gameObject.transform.position.y + m_enemyController.YOffset, PlayerController.s_instance.gameObject.transform.position.z);

        m_enemyController.Agent.velocity = Vector3.zero;
        m_enemyController.Agent.isStopped = true;
        agentSpeed = m_enemyController.Agent.speed;
        m_enemyController.Agent.speed = 0;

        go = true;
        time = 0;

        m_enemyController.Agent.updateRotation = true;
        //m_weaponEnemyBehaviour.StartCoroutine(m_weaponEnemyBehaviour.OnEnemyShoot(m_weaponEnemyBehaviour._attack.nbrOfShootOnRafale, m_weaponEnemyBehaviour._attack.timeBetweenEachBullet, m_weaponEnemyBehaviour._attack.minTimeBetweenEachBurst, m_weaponEnemyBehaviour._attack.maxTimeBetweenEachBurst, lastFrameTargetPos));
    }

    public void Exit()
    {
        m_weaponEnemyBehaviour.StopCoroutine(m_weaponEnemyBehaviour.OnEnemyShoot(m_weaponEnemyBehaviour._attack.nbrOfShootOnRafale, m_weaponEnemyBehaviour._attack.timeBetweenEachBullet, m_weaponEnemyBehaviour._attack.minTimeBetweenEachBurst, m_weaponEnemyBehaviour._attack.maxTimeBetweenEachBurst, lastFrameTargetPos));

        m_enemyController.Agent.isStopped = false;
        m_enemyController.Agent.speed = agentSpeed;


    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {
    }
    
    public void LateUpdate()
    {
        if (RotateTowards(lastFrameTargetPos) && go)
        {
            go = false;
            m_weaponEnemyBehaviour.StartCoroutine(m_weaponEnemyBehaviour.OnEnemyShoot(m_weaponEnemyBehaviour._attack.nbrOfShootOnRafale, m_weaponEnemyBehaviour._attack.timeBetweenEachBullet, m_weaponEnemyBehaviour._attack.minTimeBetweenEachBurst, m_weaponEnemyBehaviour._attack.maxTimeBetweenEachBurst, lastFrameTargetPos));
        }
        //    RotateOnAttack();

        //if (!go)
        //{
        //}
    }

    private bool RotateTowards(Vector3 target)
    {
        if (Mathf.InverseLerp(0, maxTime, time) >= 1)
        {
            return true;
        }
        else if (Mathf.InverseLerp(0, maxTime, time) == 0)
        {
            direction = (target - m_enemyController.transform.position).normalized;
            lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            angularDistance = Quaternion.Angle(m_enemyController.transform.rotation, lookRotation);

            torsoDirection = (target - m_enemyController._debug.boneToMove.transform.position).normalized;
            torsoLookRotation = Quaternion.LookRotation(new Vector3(torsoDirection.x, torsoDirection.y, torsoDirection.z));
            //torsoAngularDistance = Quaternion.Angle(m_enemyController._debug.boneToMove.transform.rotation, torsoLookRotation);

            maxTime = angularDistance / (m_enemyController.Cara._enemyCaractéristique._move.rotationSpeed);
            if (maxTime == 0)
            {
                maxTime = 0.01f;
            }
#if UNITY_EDITOR
            if (m_enemyController._debug.useDebugLogs)
            {
                Debug.Log(m_enemyController + " I have to wait " + maxTime + " seconds before starting to attack.");
            }
#endif
            time += Time.deltaTime;
            return false;
        }
        else
        {
            time += Time.deltaTime;
            m_enemyController.transform.rotation = Quaternion.Slerp(m_enemyController.transform.rotation, lookRotation, Mathf.InverseLerp(0, maxTime, time));
            SlerpSpineRotation(Quaternion.Slerp(m_enemyController._debug.boneToMove.transform.rotation, torsoLookRotation, Mathf.InverseLerp(0, maxTime, time)));
            return false;
        }
    }


    void RotateOnAttack()
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = PlayerController.s_instance.gameObject.transform.position - m_enemyController.transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = m_enemyController.Cara._enemyCaractéristique._move.rotationSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(m_enemyController.transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(m_enemyController.transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        m_enemyController._debug.boneToMove.transform.rotation = Quaternion.LookRotation(newDirection);
    }


    void SlerpSpineRotation(Quaternion value)
    {
        m_enemyController._debug.boneToMove.transform.rotation = value;
        m_enemyController.finalStateOfTheBoneRotation = m_enemyController._debug.boneToMove.transform.rotation;
    }

}
