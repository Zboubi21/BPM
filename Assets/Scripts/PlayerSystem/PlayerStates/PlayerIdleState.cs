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

    public void Enter()
    {
        
    }
    public void FixedUpdate()
    {
        m_playerController.CheckForGround();

        if (m_playerController.PlayerHasToFall())
        {
            m_playerController.ChangeState(PlayerState.Fall);
        }

        m_playerController.ResetPlayerVelocity();
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
    }
    public void LateUpdate()
    {

    }
    public void Exit()
    {

    }

}
