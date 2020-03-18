using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFollowPlayerCam : MonoBehaviour
{
   
   [SerializeField] Transform m_target;
   [SerializeField] Vector3 m_offset;
   [SerializeField, Range(0, 1)] float m_xSpeed = 1, m_ySpeed = 1, m_zSpeed = 1;
   [SerializeField] float m_clampValue = 0.25f;

   [Space]
   [SerializeField] float m_distance = 0.25f;

	Vector3 localPositionOffset;
   Vector3 m_currentPosition;
   Vector3 m_targetPos;

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
   }
   public void UpdateScript(Vector3 moveDirection)
   {
      FollowTarget();

      // m_trans.position = m_target.position + moveDirection;
      // m_trans.forward * m_distance;
   }
   void LateUpdate()
   {
      // FollowTarget();
   }
   
   void FollowTarget()
   {
      m_targetPos = m_target.position + m_offset;
      // transform.position = Vector3.Lerp(transform.position, m_targetPos, Time.deltaTime * m_speed);

      //Smooth current position;
      Vector3 deltaValue = m_currentPosition - m_targetPos;
      // Debug.Log("m_currentPosition = " + m_currentPosition + " | m_targetPos = " + m_targetPos + " | distance = " + deltaValue);
      m_currentPosition = Smooth(m_currentPosition, m_targetPos, m_xSpeed, m_ySpeed, m_zSpeed);

      float deltaX = Mathf.Abs(m_currentPosition.x - m_targetPos.x);
      if (deltaX > m_clampValue)
      {

      }

      //Set position;
      transform.position = m_currentPosition;
   }

   Vector3 Smooth(Vector3 _start, Vector3 _target, float xSmooth, float ySmooth, float zSmooth)
	{
		//Convert local position offset to world coordinates;
		Vector3 _offset = transform.localToWorldMatrix * localPositionOffset;

		//Add local position offset to target;
		_target += _offset;

      Vector3 lerpPos;
      lerpPos.x = Mathf.Lerp(_start.x, _target.x, xSmooth /** Time.deltaTime*/);
      lerpPos.y = Mathf.Lerp(_start.y, _target.y, ySmooth /** Time.deltaTime*/);
      lerpPos.z = Mathf.Lerp(_start.z, _target.z, zSmooth /** Time.deltaTime*/);
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
