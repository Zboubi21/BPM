using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyStateEnum;

public class DieState : IState
{
    EnemyController m_enemyController;

    public DieState(EnemyController enemyController)
    {
        m_enemyController = enemyController;
    }
    public void Enter()
    {
        /// play dying animation
        /// play dying sound
        Vector3 direction = (PlayerController.s_instance.transform.position - m_enemyController.transform.position).normalized;
        m_enemyController.Anim.SetLayerWeight(1, 1);
        m_enemyController.Anim.SetTrigger("Dead");
        m_enemyController.Anim.SetFloat("DeadlyHitValueX", direction.x);
        m_enemyController.Anim.SetFloat("DeadlyHitValueY", direction.z);
        m_enemyController.Agent.isStopped = true;
        m_enemyController.KillNPC(m_enemyController.m_waitTimeToDie);   //Send back to pool with animation time
        AddRightScore();
    }

    public void Exit()
    {
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
    bool isKillOnlyOnWeakSpots;
    void AddRightScore()
    {
        GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.killSomething.killAnEnemy);

        #region Rusher Weak Spot Kill
        foreach (bool b in m_enemyController.Cara.CheckWeakSpotHit)
        {
            if (b == false)
            {
                isKillOnlyOnWeakSpots = false;
                break;
            }
            isKillOnlyOnWeakSpots = true;

        }
        if (isKillOnlyOnWeakSpots)
        {
            GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.killSomething.rusherWeakSpotKill);
        }
        #endregion

        if (!PlayerController.s_instance.PlayerIsGrounded())
        {
            GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.killSomething.rusherWeakSpotKill);
        }

        if (!m_enemyController.HasShoot)
        {
            GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.killSomething.killedBeforeItShots);
        }

        GameManager.Instance.CountTheKill();
    }
}
