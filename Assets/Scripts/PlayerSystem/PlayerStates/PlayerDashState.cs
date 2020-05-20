using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;

public class PlayerDashState : IState
{
    
    float m_dashTimer = 0;
    bool m_haseDash = false;

    Vector3 m_dashDirection;
    float m_dashSpeed;

    PlayerController m_playerController;

    // Constructor (CTOR)
    public PlayerDashState(PlayerController playerController)
    {
        m_playerController = playerController;
    }

    public void Enter()
    {
        m_playerController.m_dash.m_dashBreaker.On_StartDash(true);

        m_playerController.StartDashCooldown(true);

        m_playerController.StartChangeDashLayerValueCorout(1, false);
        m_playerController.SetPlayerWeaponLayerLength(1, 1);

        m_playerController.SetPlayerWeaponAnim("Dash");
        m_playerController.m_dash.m_dashAnim?.SwitchValue();

        m_playerController.On_PlayerHasDash(true);
        m_playerController.On_PlayerStartDash(true);
        m_playerController.GetComponent<WeaponPlayerBehaviour>().CanShoot = false;
        m_dashDirection = m_playerController.GetPlayerDashDirection();

        m_dashTimer = 0;
        m_haseDash = false;

        m_playerController.ResetPlayerVelocity();
        m_playerController.ResetPlayerMomentum();   // Rajouté le 25/03 à 13h55 pour test de changer le feeling du 2e saut après le 1er

        m_dashSpeed = m_playerController.m_dash.m_distance / m_playerController.m_dash.m_timeToDash;

        if (m_playerController.GetPlayerInputsDirection() == new Vector2(-1, 0))
            m_playerController.ChangeCameraFov(m_playerController.GetTargetedDashBackardCameraFOV(), m_playerController.m_fieldOfView.m_startDash.m_timeToChangeFov, m_playerController.m_fieldOfView.m_startDash.m_changeFovCurve);
        else
            m_playerController.ChangeCameraFov(m_playerController.GetTargetedDashForwardCameraFOV(), m_playerController.m_fieldOfView.m_startDash.m_timeToChangeFov, m_playerController.m_fieldOfView.m_startDash.m_changeFovCurve);
    }
    public void FixedUpdate()
    {
        m_dashTimer += Time.deltaTime;
        if(m_dashTimer > m_playerController.m_dash.m_timeToDash && !m_haseDash)
        {
            m_haseDash = true;
            ExitStateAfterDash();
        }

        m_playerController.SetPlayerVelocity(m_dashDirection * m_dashSpeed);
    }
    public void Update()
    {

    }
    public void LateUpdate()
    {

    }
    public void Exit()
    {
        m_playerController.GetComponent<WeaponPlayerBehaviour>().CanShoot = true;

        m_playerController.ChangeCameraFov(m_playerController.GetTargetedCameraFOV(), m_playerController.m_fieldOfView.m_endDash.m_timeToChangeFov, m_playerController.m_fieldOfView.m_endDash.m_changeFovCurve);

        m_playerController.On_PlayerStartDash(false);

        if (m_playerController.PlayerIsGrounded())
        {
            m_playerController.StartDashCooldown();

            if (m_playerController.LastState(PlayerState.Jump) || (m_playerController.LastState(PlayerState.Fall)))
            {
                m_playerController.On_GroundContactRegained();
            }
        }

        m_playerController.StartRawInputAfterDash();
        m_playerController.StartChangeDashLayerValueCorout(0, true);

        m_playerController.m_dash.m_dashBreaker.On_StartDash(false);
    }

    void ExitStateAfterDash()
    {
        m_playerController.CheckForGround();

        if (m_playerController.PlayerIsGrounded())
        {
            if(!m_playerController.PlayerInputIsMoving())
            {
                m_playerController.ChangeState(PlayerState.Idle);
            }
            else
            {
                m_playerController.ChangeState(PlayerState.Run);
            }
        }
        else
        {
            m_playerController.On_GroundContactLost();   // Rajouté le 25/03 à 13h55 pour test de changer le feeling du 2e saut après le 1er
            m_playerController.ChangeState(PlayerState.Fall);
        }
    }

}
