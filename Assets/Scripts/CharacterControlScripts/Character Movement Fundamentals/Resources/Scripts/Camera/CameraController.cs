using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script rotates a gameobject based on user input.
//Rotation around the x-axis (vertical) can be clamped/limited by setting 'upperVerticalLimit' and 'lowerVerticalLimit'.
public class CameraController : MonoBehaviour {

	//Names of input axes;
	const string horizontalInputAxis = "Mouse X";
	const string verticalInputAxis = "Mouse Y";

	//Invert rotation around x- or y-axis;
	public bool invertHorizontalInput = false;
	public bool invertVerticalInput = true;

	//Current rotation values (in degrees);
	float currentXAngle = 0f;
	float currentYAngle = 0f;

	//Upper and lower limits (in degrees) for vertical rotation (along the local x-axis of the gameobject);
	[Range(0f, 90f)]
	public float upperVerticalLimit = 60f;
	[Range(0f, 90f)]
	public float lowerVerticalLimit = 60f;

	//Variables to store old rotation values for interpolation purposes;
	float oldHorizontalInput = 0f;
	float oldVerticalInput = 0f;

	//Camera turning speed; 
	public float cameraSpeed = 50f;

	//If 'useMouseInput' is set to 'true', 'cameraMouseSpeed' is used instead of 'cameraSpeed' and input is not multiplied with 'Time.deltaTime';
	public bool useMouseInput = true;

	//Camera turning speed when using mouse input (if 'useMouseInput' is set to 'true');
	public float cameraMouseSpeed = 1f;

	// protected Vector3 lastPosition;

	//Whether camera rotation values will be smoothed;
	public bool smoothCameraRotation = false;

	//This value controls how smoothly the old camera rotation angles will be interpolated toward the new camera rotation angles;
	//Setting this value to '50f' (or above) will result in no smoothing at all;
	//Setting this value to '1f' (or below) will result in very noticable smoothing;
	//For most situations, a value of '25f' is recommended;
	[Range(1f, 50f)]
	public float cameraSmoothingFactor = 25f;

	//Variables for storing current facing direction and upwards direction;
	Vector3 facingDirection;
	Vector3 upwardsDirection;

	//References to transform and camera components;
	protected Transform m_trans;
	protected Camera m_camera;

	IEnumerator m_fovChangerCorout;

#region Event Functions
	//Setup references.
	void Awake () {
		m_trans = transform;

		m_camera = GetComponentInChildren<Camera>();

		// lastPosition = m_trans.position;

		//Set angle variables to current rotation angles of this transform;
		currentXAngle = m_trans.localRotation.eulerAngles.x;
		currentYAngle = m_trans.localRotation.eulerAngles.y;

		//Execute camera rotation code once to calculate facing and upwards direction;
		RotateCamera(0f, 0f);

		Setup();

        Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void UpdateScript()
	{
		HandleCameraRotation();
	}
	// void Update()
	// {
	// 	HandleCameraRotation();
	// }
	void LateUpdate()
	{
		// transform.position = new Vector3(34.54f, 1.7f, 44.01f);
	}
#endregion

#region Private Functions
	//This function is called right after Awake(); It can be overridden by inheriting scripts;
	protected virtual void Setup()
	{
		
	}

	//Return user input; This function can be overridden by extending scripts to implement custom input;
	protected virtual Vector2 GetInput()
	{
		return new Vector2(Input.GetAxis(horizontalInputAxis), Input.GetAxis(verticalInputAxis));
	}

	//Get user input and handle camera rotation;
	//This method can be overridden in classes derived from this base class to modify camera behaviour;
	protected virtual void HandleCameraRotation()
	{
		//Get user input;
		Vector2 _inputVector = GetInput();
		
		//Get input values;
		float _inputHorizontal = _inputVector.x;
		float _inputVertical = _inputVector.y;

		if(!useMouseInput)
		{
			//When not using mouse input, multiply input with 'cameraSpeed' and delta time;
			_inputHorizontal *= Time.deltaTime * cameraSpeed;
			_inputVertical *= Time.deltaTime * cameraSpeed;
		}
		else
		{
			//Multiply input with 'cameraMouseSpeed';
			_inputHorizontal *= cameraMouseSpeed;
			_inputVertical *= cameraMouseSpeed;
		}

		RotateCamera(_inputHorizontal, _inputVertical);
	}

	//Rotate camera; 
	protected void RotateCamera(float _newHorizontalInput, float _newVerticalInput)
	{
		if(invertHorizontalInput)
			_newHorizontalInput *= (-1f);

		if(invertVerticalInput)
			_newVerticalInput *= (-1f);

		if(smoothCameraRotation)
		{
			//Lerp input;
			oldHorizontalInput = Mathf.Lerp (oldHorizontalInput, _newHorizontalInput, Time.deltaTime * cameraSmoothingFactor / Time.timeScale);
			oldVerticalInput = Mathf.Lerp (oldVerticalInput, _newVerticalInput, Time.deltaTime * cameraSmoothingFactor / Time.timeScale);
		}
		else
		{
			//Replace old input directly;
			oldHorizontalInput = _newHorizontalInput;
			oldVerticalInput = _newVerticalInput;
		}

		//Add input to camera angles;
		currentXAngle += oldVerticalInput;
		currentYAngle += oldHorizontalInput;

		//Clamp vertical rotation;
		currentXAngle = Mathf.Clamp(currentXAngle, -upperVerticalLimit, lowerVerticalLimit);

		UpdateRotation();
	}

	//Update camera rotation based on x and y angles;
	protected void UpdateRotation()
	{
		m_trans.localRotation = Quaternion.Euler(new Vector3(0, currentYAngle, 0));

		//Save 'facingDirection' and 'upwardsDirection' for later;
		facingDirection = m_trans.forward;
		upwardsDirection = m_trans.up;

		m_trans.localRotation = Quaternion.Euler(new Vector3(currentXAngle, currentYAngle, 0));
	}

	IEnumerator FovChangerCorout(float newFov, float timeToChangeFov, AnimationCurve changeFovCurve)
	{
		float fromFov = GetCameraFov();
        float fracJourney = 0;
        float distance = Mathf.Abs(fromFov - newFov);
        float vitesse = distance / timeToChangeFov;
        float actualFov = fromFov;

        while (actualFov != newFov)
        {
            fracJourney += (Time.deltaTime) * vitesse / distance;
            actualFov = Mathf.Lerp(fromFov, newFov, changeFovCurve.Evaluate(fracJourney));
			SetCameraFOV(actualFov);
            yield return null;
        }
	}
#endregion

#region Public Functions
	//Set the camera's field-of-view (FOV);
	public void SetCameraFOV(float fov)
	{
        m_camera.fieldOfView = fov;
	}
	public float GetCameraFov()
	{
		return m_camera.fieldOfView;
	}

	//Set x and y angle directly;
	public void SetRotationAngles(float _xAngle, float _yAngle)
	{
		currentXAngle = _xAngle;
		currentYAngle = _yAngle;

		UpdateRotation();
	}

	//Rotate the camera toward a rotation that points at a world position in the scene;
	public void RotateTowardPosition(Vector3 _position, float _lookSpeed)
	{
		//Calculate target look vector;
		Vector3 _direction = (_position - m_trans.position);

		RotateTowardDirection(_direction, _lookSpeed);
	}

	//Rotate the camera toward a look vector in the scene;
	public void RotateTowardDirection(Vector3 _direction, float _lookSpeed)
	{
		//Normalize direction;
		_direction.Normalize();

		//Transform target look vector to this transform's local space;
		_direction = m_trans.parent.InverseTransformDirection(_direction);

		//Calculate (local) current look vector; 
		Vector3 _currentLookVector = GetAimingDirection();
		_currentLookVector = m_trans.parent.InverseTransformDirection(_currentLookVector);

		//Calculate x angle difference;
		float _xAngleDifference = VectorMath.GetAngle(new Vector3(0f, _currentLookVector.y, 1f), new Vector3(0f, _direction.y, 1f), Vector3.right);

		//Calculate y angle difference;
		_currentLookVector.y = 0f;
		_direction.y = 0f;
		float _yAngleDifference = VectorMath.GetAngle(_currentLookVector, _direction, Vector3.up);

		//Turn angle values into Vector2 variables for better clamping;
		Vector2 _currentAngles = new Vector2(currentXAngle, currentYAngle);
		Vector2 _angleDifference = new Vector2(_xAngleDifference, _yAngleDifference);

		//Calculate normalized direction;
		float _angleDifferenceMagnitude = _angleDifference.magnitude;
		if(_angleDifferenceMagnitude == 0f)
			return;
		Vector2 _angleDifferenceDirection = _angleDifference/_angleDifferenceMagnitude;

		//Check for overshooting;
		if(_lookSpeed * Time.deltaTime > _angleDifferenceMagnitude)
		{
			_currentAngles += _angleDifferenceDirection * _angleDifferenceMagnitude;
		}
		else
			_currentAngles += _angleDifferenceDirection * _lookSpeed * Time.deltaTime;

		//Set new angles;
		currentYAngle = _currentAngles.y;
		//Clamp vertical rotation;
		currentXAngle = Mathf.Clamp(_currentAngles.x, -upperVerticalLimit, lowerVerticalLimit);
		
		UpdateRotation();
	}

	public float GetCurrentXAngle()
	{
		return currentXAngle;
	}

	public float GetCurrentYAngle()
	{
		return currentYAngle;
	}

	//Returns the direction the camera is facing, without any vertical rotation;
	//This vector should be used for movement-related purposes (e.g., moving forward);
	public Vector3 GetFacingDirection ()
	{
		return facingDirection;
	}

	//Returns the 'forward' vector of this gameobject;
	//This vector points in the direction the camera is "aiming" and could be used for instantiating projectiles or raycasts.
	public Vector3 GetAimingDirection ()
	{
		return m_trans.forward;
	}

	// Returns the 'right' vector of this gameobject;
	public Vector3 GetStrafeDirection ()
	{
		return m_trans.right;
	}

	// Returns the 'up' vector of this gameobject;
	public Vector3 GetUpDirection ()
	{
		return upwardsDirection;
	}
	
	public void ChangeCameraFov(float newFov, float timeToChangeFov, AnimationCurve changeFovCurve)
	{
		if (m_fovChangerCorout != null)
		{
			StopCoroutine(m_fovChangerCorout);
		}
		m_fovChangerCorout = FovChangerCorout(newFov, timeToChangeFov, changeFovCurve);
		StartCoroutine(m_fovChangerCorout);
	}
#endregion

}
