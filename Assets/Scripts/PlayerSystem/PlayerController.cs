using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateEnum;
using EZCameraShake;

public class PlayerController : MonoBehaviour
{
    public static PlayerController s_instance;

#region Public Variables
	[Header("Debug")]
    [SerializeField] StateMachine m_sM = new StateMachine();
    //public float maxRecordPositionTime = 5f;
	// public bool m_useGravity = true;

	[Header("References")]
	public References m_references;
	[Serializable] public class References{
		public Transform m_cameraPivot;
		public PlayerAudioController m_playerAudio;
		public Animator m_weaponAnimator;
        public Transform targetForEnnemies;
	}

	[Header("Movements")]
	public Movements m_movements;
	[Serializable] public class Movements{
		public bool m_useRawInput = true;
		public float m_baseSpeed = 11f;
		public float m_overadrenalineSpeed = 14f;
        public bool lockCursor;
	}

	[Header("Physics")]
	[SerializeField] Physics m_physics;
	[Serializable] class Physics{
		//'AirFriction' determines how fast the controller loses its momentum while in the air;
		public float m_airFriction = 3f;

		//'GroundFriction' is used instead, if the controller is grounded;
		public float m_groundFriction = 100f;

		//Amount of downward gravitation;
		public float m_gravity = 35f;

		public float m_timeToFallWhenNotGrounded = 0.1f;
	}

	[Header("Jump variables")]
	//'Aircontrol' determines to what degree the player is able to move while in the air;
	[SerializeField, Range(0f, 1f)] float m_airControl = 0.8f;
	public Jump m_jump;
	public Jump m_doubleJump;
	[Serializable] public class Jump{
		public float m_height = 2.5f;
		public float m_duration = 0.2f;
	}

	[Header("On Land")]
	[SerializeField] LandCameraShaking[] m_onLandCameraShakes;
	[Serializable] class LandCameraShaking
	{
		public float m_triggerHeight = 1;
		public CameraShake m_cameraShake;
	}

	[Header("Dash")]
	public Dash m_dash;
	[Serializable] public class Dash{
		[Range(0, 1)] public float m_minInputCanDash = 0.5f;
		public float m_distance = 10;
		public float m_timeToDash = 0.25f;
		public float m_dashCooldown = 0.5f;

		[Header("After Dash")]
		public bool m_useRawInput = false;
		public float m_timeToStopUseRawInput = 1;

		[Header("Feedback")]
		public ChangeImageValues m_dashFeedbackScreen;
		public ChangeRotationValue m_dashAnim;
	}

	[Header("Field Of View")]
	public FieldOfView m_fov;
	[Serializable] public class FieldOfView
	{
		[Header("Values")]
		public float m_normalFov = 90f;
		public float m_normalDashFov = 100f;
		public float m_backwardDashFov = 95f;

		[Header("Anims")]
		public FovChanger m_startDash;
		public FovChanger m_endDash;

		[Serializable] public class FovChanger
		{
			public float m_timeToChangeFov;
			public AnimationCurve m_changeFovCurve;
		}
	}

	[Space]

	[SerializeField] ScriptOrder m_scriptOrder;
	[Serializable] class ScriptOrder
	{
		public CameraController m_cameraControls;
		public TransformFollower m_camPivot;
		public TransformFollower m_gunPivot;
	}

	[Serializable] public class CameraShake
	{
		public float m_magnitude = 1;
		public float m_roughness = 1;
		public float m_fadeInTime = 1;
		public float m_fadeOutTime = 1;
	}

#endregion

#region Private Variables
    //References to attached components;
	Transform m_trans;
	Mover m_mover;

	//Names of input axes used for horizontal and vertical input;
	string m_horizontalInputAxis = "Horizontal";
	string m_verticalInputAxis = "Vertical";

    //Movement speed;
    float m_currentSpeed;
	
	float m_jumpSpeed;
	float m_doubleJumpSpeed;

	float m_currentTimeToFallWhenNotGrounded = 0;
	bool m_hasToFall = false;

	//Current momentum;
	Vector3 m_momentum = Vector3.zero;

	//Saved velocity from last frame;
	Vector3 m_savedVelocity = Vector3.zero;

	//Saved horizontal movement velocity from last frame;
	Vector3 m_savedMovementVelocity = Vector3.zero;

	bool m_currentUseRawInput;

	int m_currentJumpNbr = 0;

	bool m_hasDash = false;
	IEnumerator m_dashCooldownCorout;
	IEnumerator m_rawInputAfterDash;

	Vector2 m_playerInputsDirection;
	Vector3 m_playerMoveInputsDirection;

    CameraController m_cameraController;
	WeaponPlayerBehaviour m_playerWeapon;

    float _currentTimefRecord;

    public CameraController CameraControls { get => m_scriptOrder.m_cameraControls; set => m_scriptOrder.m_cameraControls = value; }
    public WeaponPlayerBehaviour PlayerWeapon { get => m_playerWeapon; set => m_playerWeapon = value; }

    #endregion

#region Event Functions
    void Awake()
    {
        SetupSingleton();
		SetupStateMachine();

        m_mover = GetComponent<Mover>();
		m_trans = GetComponent<Transform>();

		m_cameraController = GetComponentInChildren<CameraController>();
		m_playerWeapon = GetComponent<WeaponPlayerBehaviour>();

        m_currentSpeed = m_movements.m_baseSpeed;

		m_jumpSpeed = m_jump.m_height / m_jump.m_duration;
		m_doubleJumpSpeed = m_doubleJump.m_height / m_doubleJump.m_duration;

		m_currentUseRawInput = m_movements.m_useRawInput;

		m_scriptOrder.m_cameraControls.LockCamera = m_movements.lockCursor;
	}

	void OnEnable()
	{

	}

	void Start()
    {
		
    }

	void FixedUpdate()
	{
		CalculateDashDirection();
		m_sM.FixedUpdate();
	}

	void Update()
	{
		m_sM.Update();
		m_scriptOrder.m_cameraControls.UpdateScript();
        m_scriptOrder.m_camPivot.UpdateScript();
		m_scriptOrder.m_gunPivot.UpdateScript();
	}

	void LateUpdate()
	{
		m_sM.LateUpdate();
	}
#endregion

#region Private Functions
	void SetupSingleton()
	{
		if(s_instance == null){
			s_instance = this;
		}else{
			Debug.LogError("Two instance of PlayerController");
		}
	}

	void SetupStateMachine()
	{
		m_sM.AddStates(new List<IState> {
			new PlayerIdleState(this),				// 0 = Idle
			new PlayerRunState(this),				// 1 = Run
			new PlayerFallState(this),				// 2 = Fall
			new PlayerJumpState(this),				// 3 = Jump
			new PlayerDashState(this),				// 4 = Dash
		});

        string[] playerStateNames = System.Enum.GetNames(typeof(PlayerState));
		if(m_sM.States.Count != playerStateNames.Length){
			Debug.LogError("You need to have the same number of State in PlayerController and PlayerStateEnum");
		}

        ChangeState(PlayerState.Idle);
	}

    //Calculate and return movement direction based on player input;
	//This function can be overridden by inheriting scripts to implement different player controls;
	Vector3 CalculateMovementDirection()
	{
		float _horizontalInput;
		float _verticalInput;

		//Get input;
		if(m_currentUseRawInput){
			_horizontalInput = Input.GetAxisRaw(m_horizontalInputAxis);
			_verticalInput = Input.GetAxisRaw(m_verticalInputAxis);
		} else {
			_horizontalInput = Input.GetAxis(m_horizontalInputAxis);
			_verticalInput = Input.GetAxis(m_verticalInputAxis);
		}

		m_playerInputsDirection = new Vector2(_verticalInput, _horizontalInput);

		Vector3 _direction = Vector3.zero;

		//Use camera axes to construct movement direction;
		_direction += m_references.m_cameraPivot.forward * _verticalInput;
		_direction += m_references.m_cameraPivot.right * _horizontalInput;

        //Clamp movement vector to magnitude of '1f';
        if (_direction.magnitude > 1f)
			_direction.Normalize();

		m_playerMoveInputsDirection = _direction;
		return _direction;
	}
	Vector3 CalculateDashDirection()
	{
		float _horizontalInput;
		float _verticalInput;

		_horizontalInput = Input.GetAxis(m_horizontalInputAxis);
		_verticalInput = Input.GetAxis(m_verticalInputAxis);

		if (_horizontalInput > 0)
			_horizontalInput = 1;
		if (_horizontalInput < 0)
			_horizontalInput = -1;
		if (_verticalInput > 0)
			_verticalInput = 1;
		if (_verticalInput < 0)
			_verticalInput = -1;

		// Debug.Log("_horizontalInput = " + _horizontalInput + " | _verticalInput = " + _verticalInput);

		Vector3 _direction = Vector3.zero;

		//Use camera axes to construct movement direction;
		_direction += m_references.m_cameraPivot.forward * _verticalInput;
		_direction += m_references.m_cameraPivot.right * _horizontalInput;

        //Clamp movement vector to magnitude of '1f';
        if (_direction.magnitude > 1f)
			_direction.Normalize();

		// Debug.Log("_direction = " + _direction);
		return _direction;
	}

	//Calculate and return movement velocity based on player input, controller state, ground normal [...];
	Vector3 CalculateMovementVelocity()
	{
		//Calculate (normalized) movement direction;
		Vector3 _velocity = CalculateMovementDirection();

		//Save movement direction for later;
		Vector3 _velocityDirection = _velocity;

		//Multiply (normalized) velocity with movement speed;
		_velocity *= m_currentSpeed;

		//If controller is in the air, multiply movement velocity with 'airControl';
		// if (!PlayerIsGrounded() && m_hasToFall)
			// Debug.Log("need airControl = ");

// ----- !!! À réactiver !!! -----
		if(!PlayerIsGrounded() && m_hasToFall)
			_velocity = _velocityDirection * m_currentSpeed * m_airControl;

		return _velocity;
	}

	//Apply friction to both vertical and horizontal momentum based on 'friction' and 'gravity';
	//Handle sliding down steep slopes;
	void HandleMomentum()
	{
		Vector3 _verticalMomentum = Vector3.zero;
		Vector3 _horizontalMomentum = Vector3.zero;

		//Split momentum into vertical and horizontal components;
		if(m_momentum != Vector3.zero)
		{
			_verticalMomentum = VectorMath.ExtractDotVector(m_momentum, m_trans.up);
			_horizontalMomentum = m_momentum - _verticalMomentum;
		}

        //Add gravity to vertical momentum;
		// if (m_useGravity)
		// {
			_verticalMomentum -= m_trans.up * m_physics.m_gravity * Time.deltaTime;
			if(CurrentState(PlayerState.Idle) || CurrentState(PlayerState.Run))
				_verticalMomentum = Vector3.zero;
		// }

		//Apply friction to horizontal momentum based on whether the controller is grounded;
		if(PlayerIsGrounded() && !m_hasToFall)
			_horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(_horizontalMomentum, m_physics.m_groundFriction, Time.deltaTime, 0f);
		else
			_horizontalMomentum = VectorMath.IncrementVectorLengthTowardTargetLength(_horizontalMomentum, m_physics.m_airFriction, Time.deltaTime, 0f); 

		//Add horizontal and vertical momentum back together;
		m_momentum = _horizontalMomentum + _verticalMomentum;

        if(CurrentState(PlayerState.Jump))
        {
            m_momentum = VectorMath.RemoveDotVector(m_momentum, m_trans.up);
			AddJumpMomentum();
        }
	}
	void AddJumpMomentum()
	{
		if (m_currentJumpNbr == 1)
			m_momentum += m_trans.up * m_jumpSpeed;
		if (m_currentJumpNbr == 2)
			m_momentum += m_trans.up * m_doubleJumpSpeed;
	}

	//Helper functions;

	//Returns 'true' if vertical momentum is above a small threshold;
	bool IsFalling()
	{
		//Calculate current vertical momentum;
		Vector3 _verticalMomentum = VectorMath.ExtractDotVector(m_momentum, m_trans.up);

		//Setup threshold to check against;
		//For most applications, a value of '0.001f' is recommended;
		float _limit = 0.001f;

		//Return true if vertical momentum is above '_limit';
		return(_verticalMomentum.magnitude > _limit);
	}

	IEnumerator StartDashCooldownCorout()
	{
        yield return new WaitForSeconds(m_dash.m_dashCooldown);
        On_PlayerHasDash(false);
	}
	IEnumerator StartRawInputCooldownAfterDash()
	{
		m_currentUseRawInput = m_dash.m_useRawInput;
        yield return new WaitForSeconds(m_dash.m_timeToStopUseRawInput);
		m_currentUseRawInput = m_movements.m_useRawInput;
	}

    
    #endregion

#region Public Functions
    public void ChangeState(PlayerState newPlayerState){
		m_sM.ChangeState((int)newPlayerState);
	}
    public bool CurrentState(PlayerState playerState)
    {
        return m_sM.CurrentStateIndex == (int)playerState;
    }
	public bool LastState(PlayerState playerState)
    {
        return m_sM.LastStateIndex == (int)playerState;
    }

    public void CheckForGround()
	{
		m_mover.CheckForGround();
	}
    public bool PlayerIsGrounded()
	{
		if (m_mover.IsGrounded())
			m_hasToFall = false;
		return m_mover.IsGrounded();
	}

    public bool PlayerIsFalling()
	{
		return IsFalling() && (VectorMath.GetDotProduct(GetMomentum(), m_trans.up) > 0f);
	}

    public bool PlayerInputIsMoving()
	{
		if(CalculateMovementDirection() != Vector3.zero){
			return true;
		}else{
			return false;
		}
	}

    public void Move()
	{
		//Apply friction and gravity to 'momentum';
		HandleMomentum();

		//Calculate movement velocity;
		Vector3 velocity = CalculateMovementVelocity();

		//Add current momentum to velocity;
		velocity += m_momentum;
		
		//If player is grounded or sliding on a slope, extend mover's sensor range;
		//This enables the player to walk up/down stairs and slopes without losing ground contact;
		m_mover.SetExtendSensorRange(PlayerIsGrounded());

		SetPlayerVelocity(velocity);
	}

    public void SetPlayerVelocity(Vector3 velocity)
	{
		//Set mover velocity;		
		m_mover.SetVelocity(velocity);

		//Store velocity for next frame;
        m_savedVelocity = velocity;
        m_savedMovementVelocity = velocity - m_momentum;
	}
	public void ResetPlayerVelocity()
	{
		SetPlayerVelocity(Vector3.zero);
	}
	public void ResetPlayerMomentum()
	{
		m_momentum = Vector3.zero;
	}

	//Returns 'true' if the player presses the jump key;
	public bool IsJumpKeyPressed()
	{
        return (Input.GetButtonDown("Jump"));
	}

	public bool IsDashKeyPressed()
	{
        return (Input.GetButtonDown("Dash"));
	}
	public bool CanDash()
	{
		if (PressMovementInput() && !m_hasDash)
			return true;
		return false;
	}
	bool PressMovementInput()
	{
		if (Input.GetAxis(m_horizontalInputAxis) > m_dash.m_minInputCanDash || Input.GetAxis(m_horizontalInputAxis) < -m_dash.m_minInputCanDash 
		|| Input.GetAxis(m_verticalInputAxis) > m_dash.m_minInputCanDash || Input.GetAxis(m_verticalInputAxis) < -m_dash.m_minInputCanDash)
			return true;
		return false;
	}

	public void On_PlayerIsRunning(bool isRunning)
	{
		m_references.m_playerAudio.On_Run(isRunning);
	}

	public bool CanJump()
	{
		return m_currentJumpNbr < 2; // 2 = max jump nbr
	}
	public void On_PlayerHasJump(bool hasJump)
	{
		if (hasJump)
		{
			m_currentJumpNbr ++;
			m_references.m_playerAudio.On_Jump();
		}
	}
	public void On_PlayerHasDoubleJump(bool hasDoubleJump)
	{
		if (hasDoubleJump)
		{
			m_currentJumpNbr ++;
			m_references.m_playerAudio.On_DoubleJump();
		}
	}

	public void On_PlayerStartDash(bool hasDash)
	{
		m_playerWeapon?.On_PlayerDash(hasDash);
		m_dash.m_dashFeedbackScreen.SwitchValue();
		if (hasDash)
			m_references.m_playerAudio.On_Dash();
	}
	public void On_PlayerHasDash(bool hasDash)
	{
		m_hasDash = hasDash;
	}
	public void StartDashCooldown()
	{
        if (m_dashCooldownCorout != null)
            StopCoroutine(m_dashCooldownCorout);
        m_dashCooldownCorout = StartDashCooldownCorout();
        StartCoroutine(m_dashCooldownCorout);
	}
	public void StartRawInputAfterDash()
	{
        if (m_rawInputAfterDash != null)
            StopCoroutine(m_rawInputAfterDash);
        m_rawInputAfterDash = StartRawInputCooldownAfterDash();
        StartCoroutine(m_rawInputAfterDash);
	}

    //Get last frame's velocity;
	public Vector3 GetVelocity ()
	{
		return m_savedVelocity;
	}

	//Get last frame's movement velocity (momentum is ignored);
	public Vector3 GetMovementVelocity()
	{
		return m_savedMovementVelocity;
	}

	//Get current momentum;
	public Vector3 GetMomentum()
	{
		return m_momentum;
	}

	//Add momentum to controller;
	public void AddMomentum (Vector3 _momentum)
	{
		m_momentum += _momentum;	
	}

	public Vector2 GetPlayerInputsDirection()
	{
		return m_playerInputsDirection;
	}
	public Vector3 GetPlayerMoveInputsDirection()
	{
		return m_playerMoveInputsDirection;
	}
	public Vector3 GetPlayerDashDirection()
	{
		return CalculateDashDirection();
	}

	//Events;
	//This function is called when the player has initiated a jump;
	public void On_JumpStart()
	{
		//Call event;
		if(OnJump != null)
			OnJump(m_momentum);
	}

	//This function is called when the player has lost ground contact, i.e. is either falling or rising, or generally in the air;
	public void On_GroundContactLost()
	{
		//Calculate current velocity;
		//If velocity would exceed the controller's movement speed, decrease movement velocity appropriately;
		//This prevents unwanted accumulation of velocity;
		float _horizontalMomentumSpeed = VectorMath.RemoveDotVector(GetMomentum(), m_trans.up).magnitude;
		Vector3 _currentVelocity = GetMomentum() + Vector3.ClampMagnitude(m_savedMovementVelocity, Mathf.Clamp(m_currentSpeed - _horizontalMomentumSpeed, 0f, m_currentSpeed));

		//Calculate length and direction from '_currentVelocity';
		float _length = _currentVelocity.magnitude;
		
		//Calculate velocity direction;
		Vector3 _velocityDirection = Vector3.zero;
		if(_length != 0f)
			_velocityDirection = _currentVelocity/_length;

		//Subtract from '_length', based on 'movementSpeed' and 'airControl', check for overshooting;
		if(_length >= m_currentSpeed * m_airControl)
			_length -= m_currentSpeed * m_airControl;
		else
			_length = 0f;

		m_momentum = _velocityDirection * _length;
	}

	//This function is called when the player has landed on a surface after being in the air;
	public void On_GroundContactRegained()
	{
		On_PlayerHasJump(false);
		On_PlayerHasDoubleJump(false);
		On_PlayerHasDash(false);
		m_currentJumpNbr = 0;

		//Call 'OnLand' event;
		if(OnLand != null)
			OnLand(m_momentum);

		m_references.m_playerAudio.On_Land();

		// ResetPlayerVelocity();
		ResetPlayerMomentum();

        SetPlayerWeaponAnim("OnLand");
	}

	public void On_BpmLevelChanged(int weaponLvl)
	{
		switch (weaponLvl)
		{
			case 0:
        		SetPlayerWeaponAnim("Lvl1");
			break;
			case 1:
        		SetPlayerWeaponAnim("Lvl2");
			break;
			case 2:
        		SetPlayerWeaponAnim("Lvl3");
			break;
		}
	}
	public void On_ActivateOveradrenaline(bool activate)
	{
		if (activate)
			SetPlayerWeaponAnim("StartFury");
		else
			SetPlayerWeaponAnim("EndFury");
	}

	public bool PlayerHasToFall()
	{
		if(!PlayerIsGrounded() || PlayerIsFalling())
        {
			m_currentTimeToFallWhenNotGrounded += Time.deltaTime;
			if (m_currentTimeToFallWhenNotGrounded > m_physics.m_timeToFallWhenNotGrounded)
			{
				m_currentTimeToFallWhenNotGrounded = 0;
				m_hasToFall = true;
				return true;
			}
        }
		else
		{
			if (m_currentTimeToFallWhenNotGrounded != 0)
				m_currentTimeToFallWhenNotGrounded = 0;
		}
		return false;
	}
	public void HasToFall()
	{
		m_hasToFall = true;
	}

	//Events;
	public delegate void VectorEvent(Vector3 v);
	public event VectorEvent OnJump;
	public event VectorEvent OnLand;

	public void ChangeCameraFov(float newFov, float timeToChangeFov, AnimationCurve changeFovCurve)
	{
		m_cameraController.ChangeCameraFov(newFov, timeToChangeFov, changeFovCurve);
	}

	public void On_OveradrenalineIsActivated(bool isActivated)
	{
		m_currentSpeed = isActivated ? m_movements.m_overadrenalineSpeed : m_movements.m_baseSpeed;
	}

#region Anims
	public void SetPlayerWeaponAnim(string name)
	{
		m_references.m_weaponAnimator?.SetTrigger(name);
	}
	public void SetPlayerWeaponAnim(string name, bool b)
	{
		m_references.m_weaponAnimator?.SetBool(name, b);
	}
	public void SetPlayerWeaponAnim(string name, float value)
	{
		m_references.m_weaponAnimator?.SetFloat(name, value);
	}
	public void SetPlayerWeaponLayerLength(int layerIndex, float weight)
	{
		m_references.m_weaponAnimator?.SetLayerWeight(layerIndex, weight);
	}
	public float GetPlayerWeaponLayerLength(int layerIndex)
	{
		return m_references.m_weaponAnimator.GetLayerWeight(layerIndex);
	}
#endregion

	public void AddCameraShake(float magnitude, float roughness, float fadeInTime, float fadeOutTime)
	{
		CameraShaker.Instance.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
	}

#endregion

}