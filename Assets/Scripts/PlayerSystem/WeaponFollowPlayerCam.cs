using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFollowPlayerCam : MonoBehaviour
{
   
   [SerializeField] Transform m_target;
   [SerializeField] Vector3 m_offset;
   [SerializeField, Range(0, 1)] float m_horizontalSpeed = 1, m_verticalSpeeed = 1;
   [SerializeField] float m_clampValue = 0.25f;

   [Space]
   [SerializeField] float m_distance = 0.25f;

	Vector3 localPositionOffset;
   Vector3 m_currentPosition;
   Vector3 m_targetedPos;

   // [Space]
   // [SerializeField] Transform m_trans;

   void OnEnable()
	{
		//Reset current position when gameobject is re-enabled to prevent unwanted interpolation from last position;
		ResetCurrentPosition();
	}
   void Start()
   {
      m_currentPosition = transform.position;

      m_lastDistance = new Vector3(m_target.position.x, 1.7f, m_target.position.y);
   }
   [SerializeField] Transform m_testDirection;
   [SerializeField] float m_dotValue = 0.5f;
   Vector3 m_lastDistance;
   public void UpdateScript(Vector3 moveDirection)
   {
      FollowTarget();
      // FollowTargetRot();   // J'ai essayer ça avant !
      // TestFollowTarget();   // J'ai essayer ça avant !
      // m_trans.position = m_target.position + moveDirection;
      // m_trans.forward * m_distance;

      // LastTest(moveDirection);
   }
   float ClampValue(float valueToClamp, float clampValue)
   {
      return Mathf.Clamp(valueToClamp, -clampValue, clampValue);
   }
   void LateUpdate()
   {
      // FollowTarget();
   }
   
   void FollowTarget()
   {
      m_targetedPos = m_target.position + m_offset;
      // transform.position = Vector3.Lerp(transform.position, m_targetPos, Time.deltaTime * m_speed);

      //Smooth current position;
      // Vector3 deltaValue = m_currentPosition - m_targetedPos;
      // Debug.Log("m_currentPosition = " + m_currentPosition + " | m_targetPos = " + m_targetPos + " | distance = " + deltaValue);
      m_currentPosition = Smooth(m_currentPosition, m_targetedPos, m_horizontalSpeed, m_verticalSpeeed);

      // float deltaX = Mathf.Abs(m_currentPosition.x - m_targetedPos.x);
      // if (deltaX > m_clampValue)
      // {

      // }

      //Set position;
      transform.position = m_currentPosition;
      // transform.position = Vector3.Lerp(transform.position, m_targetPos, Time.deltaTime * m_xSpeed);
   }

   [Space]
   [SerializeField] float m_smoothDampSpeed = 1;
   [SerializeField] float m_maxSmoothDampSpeed = Mathf.Infinity;
   [SerializeField] float m_deltaTime = 0.02f;
   Vector3 m_currentVelocityRef;
   void TestFollowTarget()
   {
      m_targetedPos = m_target.position + m_offset;

      // Vector3 lerpPos;
      // lerpPos.x = Mathf.Lerp(transform.position.x, m_targetedPos.x, m_xSpeed /** Time.deltaTime*/);
      // lerpPos.y = Mathf.Lerp(transform.position.y, m_targetedPos.y, m_ySpeed /** Time.deltaTime*/);
      // lerpPos.z = Mathf.Lerp(transform.position.z, m_targetedPos.z, m_zSpeed /** Time.deltaTime*/);

      transform.position = Vector3.SmoothDamp(transform.position, m_targetedPos, ref m_currentVelocityRef, m_smoothDampSpeed, m_maxSmoothDampSpeed, m_deltaTime);

      // transform.position = lerpPos;
   }

   [SerializeField] Transform m_targetRot;
   void FollowTargetRot()
   {
      transform.rotation = m_targetRot.rotation;
   }

   void LastTest(Vector3 moveDirection)
   {
      Vector3 clampMoveDirection = new Vector3(ClampValue(moveDirection.x, m_clampValue), moveDirection.y, ClampValue(moveDirection.z, m_clampValue));
      Vector3 targetedTestPosition = m_target.position - clampMoveDirection + new Vector3(0, 1.7f, 0);
      // transform.position = Vector3.SmoothDamp(transform.position, m_testDirection.position, ref m_currentVelocityRef, m_smoothDampSpeed, m_maxSmoothDampSpeed, m_deltaTime);
      m_testDirection.position = Vector3.SmoothDamp(transform.position, targetedTestPosition, ref m_currentVelocityRef, m_smoothDampSpeed, m_maxSmoothDampSpeed, m_deltaTime);
      // transform.position = m_testDirection.position;

      // float distance = Vector3.Distance(transform.position, m_testDirection.position);
      float distance = Vector3.Distance(new Vector3(m_target.position.x, 1.7f, m_target.position.y), m_testDirection.position);

      // Debug.Log("distance = " + distance);

      if(distance <  m_dotValue)
      {
         m_lastDistance = m_testDirection.position;
         transform.position = m_testDirection.position;
      }
      else
      {
         transform.position = m_lastDistance;
      }
   }

   Vector3 Smooth(Vector3 _start, Vector3 _target, float horizontalSpeed, float verticalSpeed)
	{
		//Convert local position offset to world coordinates;
		Vector3 _offset = transform.localToWorldMatrix * localPositionOffset;

		//Add local position offset to target;
		_target += _offset;

      Vector3 lerpPos;
      lerpPos.x = Mathf.Lerp(_start.x, _target.x, horizontalSpeed /** Time.deltaTime*/);
      lerpPos.y = Mathf.Lerp(_start.y, _target.y, verticalSpeed /** Time.deltaTime*/);
      lerpPos.z = Mathf.Lerp(_start.z, _target.z, horizontalSpeed /** Time.deltaTime*/);
      return lerpPos;

      // return Vector3.Lerp (_start, _target, /*Time.deltaTime * */xSmooth);
	}

   //Reset stored position and move this gameobject directly to the target's position;
	//Call this function if the target has just been moved a larger distance and no interpolation should take place (teleporting);
	public void ResetCurrentPosition()
	{
		//Convert local position offset to world coordinates;
		Vector3 _offset = transform.localToWorldMatrix * localPositionOffset;
		//Add position offset and set current position;
		m_currentPosition = m_target.position + _offset;
	}

}
