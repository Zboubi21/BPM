using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;

public class PlayerIdleState : IState
{

    PlayerController m_playerController;

    // Constructor (CTOR)
    public PlayerIdleState(PlayerController playerController)
    {
        m_playerController = playerController;
    }

    // First timer
    float m_targetedTimer = 0;
    float m_timer = 0;
    bool m_timerIsDone = false;

    // Second timer
    float m_waitTimeToReset = 0;
    float m_secondTimer = 0;
    bool m_startTimer = false;

    bool m_hasResetTimerBeacausePlayerShoot = false;

    public void Enter()
    {
        m_playerController.SetPlayerWeaponAnim("isMoving", false);
        m_playerController.SetPlayerWeaponAnim("Move", 0);
        m_hasResetTimerBeacausePlayerShoot = false;
        SetFirstTimerParameters();
    }
    void SetFirstTimerParameters()
    {
        m_targetedTimer = Random.Range(m_playerController.m_idle.m_minTimeToBreath, m_playerController.m_idle.m_maxTimeToBreath);
        m_timer = 0;
        m_timerIsDone = false;
    }
    public void FixedUpdate()
    {
        m_playerController.CheckForGround();

        if (m_playerController.PlayerHasToFall())
        {
            m_playerController.On_GroundContactLost();   // Rajouté le 25/03 à 13h55 pour test de changer le feeling du 2e saut après le 1er
            m_playerController.ChangeState(PlayerState.Fall);
        }

        m_playerController.ResetPlayerVelocity();

        if (m_playerController.PlayerWeapon.IsShooting && !m_hasResetTimerBeacausePlayerShoot)
        {
            m_hasResetTimerBeacausePlayerShoot = true;
            SetFirstTimerParameters();
            return;
        }
        else if (!m_playerController.PlayerWeapon.IsShooting && m_hasResetTimerBeacausePlayerShoot)
        {
            m_hasResetTimerBeacausePlayerShoot = false;
        }

        // First timer
        if (!m_timerIsDone)
        {
            m_timer += Time.deltaTime;
            if (m_timer > m_targetedTimer)
            {
                m_timerIsDone = true;
                m_playerController.SetPlayerWeaponAnim("Breath");
                m_waitTimeToReset = m_playerController.m_idle.m_waitAnimTime;
                m_startTimer = true;
                m_secondTimer = 0;
            }
        }
        // Second timer
        else if (m_startTimer)
        {
            m_secondTimer += Time.deltaTime;
            if (m_secondTimer > m_waitTimeToReset)
            {
                m_startTimer = false;
                SetFirstTimerParameters();
            }
        }        
    }
    public void Update()
    {
        if(m_playerController.PlayerInputIsMoving())
        {
            m_playerController.ChangeState(PlayerState.Run);
        }

        if(m_playerController.IsJumpKeyPressed() && m_playerController.CanJump())
        {
            m_playerController.On_PlayerHasJump(true);
            m_playerController.ChangeState(PlayerState.Jump);
        }
        if(m_playerController.IsDashKeyPressed())
        {
            m_playerController.ChangeState(PlayerState.Dash);
        }
    }
    public void LateUpdate()
    {

    }
    public void Exit()
    {

    }

}
