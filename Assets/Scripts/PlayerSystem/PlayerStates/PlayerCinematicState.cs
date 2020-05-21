using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;

public class PlayerCinematicState : IState
{

    PlayerController m_playerController;

    // Constructor (CTOR)
    public PlayerCinematicState(PlayerController playerController)
    {
        m_playerController = playerController;
    }

    public void Enter()
    {
    }
    public void FixedUpdate()
    {
    }
    public void Update()
    {
    }
    public void LateUpdate()
    {
    }
    public void Exit()
    {
    }

}
