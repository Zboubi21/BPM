using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;

public class PlayerRunState : IState
{

    PlayerController m_playerController;

    // Constructor (CTOR)
    public PlayerRunState(PlayerController playerController)
    {
        m_playerController = playerController;
    }

    public void Enter()
    {
        m_playerController.On_PlayerIsRunning(true);
    }
    public void FixedUpdate()
    {
        m_playerController.CheckForGround();

        if (m_playerController.PlayerHasToFall())
        {
            m_playerController.On_GroundContactLost();   // Rajouté le 25/03 à 13h55 pour test de changer le feeling du 2e saut après le 1er
            m_playerController.ChangeState(PlayerState.Fall);
        }

        m_playerController.Move();
    }
    public void Update()
    {
        if(!m_playerController.PlayerInputIsMoving())
        {
            m_playerController.ChangeState(PlayerState.Idle);
        }

        if(m_playerController.IsJumpKeyPressed() && m_playerController.CanJump())
        {
            m_playerController.On_PlayerHasJump(true);
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
        m_playerController.On_PlayerIsRunning(false);
    }

}
