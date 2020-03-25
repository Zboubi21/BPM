using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;

public class PlayerJumpState : IState
{

    float m_timer = 0;
    bool m_haseJump = false;
    float m_jumpDuration = 0;

    PlayerController m_playerController;

    // Constructor (CTOR)
    public PlayerJumpState(PlayerController playerController)
    {
        m_playerController = playerController;
    }

    public void Enter()
    {
        // m_playerController.m_useGravity = false;

        m_timer = 0;
        m_haseJump = false;
        m_jumpDuration = m_playerController.LastState(PlayerState.Jump) ? m_playerController.m_doubleJump.m_duration : m_playerController.m_jump.m_duration;

        // if (!m_playerController.LastState(PlayerState.Jump))   // Rajouté le 25/03 à 13h55 pour test de changer le feeling du 2e saut après le 1er
            m_playerController.On_GroundContactLost();

        m_playerController.On_JumpStart();

        if (m_playerController.LastState(PlayerState.Idle) || m_playerController.LastState(PlayerState.Run))
        {
		    m_playerController.On_PlayerHasDash(false);
        }
    }
    public void FixedUpdate()
    {
        m_timer += Time.deltaTime;
        if(m_timer > m_jumpDuration && !m_haseJump)
        {
            m_haseJump = true;
            m_playerController.ChangeState(PlayerState.Fall);
        }

        if(!m_haseJump)
        {
            m_playerController.Move();
        }
        else
        {
            m_playerController.CheckForGround();
            m_playerController.Move();
        }
    }
    public void Update()
    {
        if(m_playerController.IsJumpKeyPressed() && m_playerController.CanJump())
        {
            m_playerController.On_PlayerHasDoubleJump(true);
            m_playerController.ChangeState(PlayerState.Jump);
        }

        if(m_playerController.IsDashKeyPressed() && m_playerController.CanDash())
        {
            m_playerController.ChangeState(PlayerState.Dash);
        }
    }
    public void LateUpdate()
    {

    }
    public void Exit()
    {
        // m_playerController.m_useGravity = true;
    }

}
