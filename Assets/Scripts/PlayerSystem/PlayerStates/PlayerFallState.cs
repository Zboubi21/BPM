using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;

public class PlayerFallState : IState
{

    PlayerController m_playerController;

    // Constructor (CTOR)
    public PlayerFallState(PlayerController playerController)
    {
        m_playerController = playerController;
    }

    public void Enter()
    {
        // m_playerController.On_GroundContactLost();
        m_playerController.SetPlayerWeaponAnim("isFalling", true);

        if (m_playerController.LastState(PlayerState.Idle) || m_playerController.LastState(PlayerState.Run))
        {
		    m_playerController.On_PlayerHasDash(false);
        }
    }
    public void FixedUpdate()
    {
        m_playerController.HasToFall();

        m_playerController.CheckForGround();

        if(m_playerController.PlayerIsGrounded())
        {
            m_playerController.On_GroundContactRegained();
            if(!m_playerController.PlayerInputIsMoving())
            {
                m_playerController.ChangeState(PlayerState.Idle);
            }
            else
            {
                m_playerController.ChangeState(PlayerState.Run);
            }
        }

        m_playerController.Move();
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
        m_playerController.SetPlayerWeaponAnim("isFalling", false);
    }

}
