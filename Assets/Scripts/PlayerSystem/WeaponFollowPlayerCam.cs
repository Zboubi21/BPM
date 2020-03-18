using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFollowPlayerCam : MonoBehaviour
{
   
   [SerializeField] Transform m_target;
   [SerializeField] Vector3 m_offset;
   [SerializeField, Range(0, 1)] float m_xSpeed = 1, m_ySpeed = 1, m_zSpeed = 1;

	Vector3 localPositionOffset;
   Vector3 m_currentPosition;
   Vector3 m_targetPos;

   void OnEnable()
	{
		//Reset current position when gameobject is re-enabled to prevent unwanted interpolation from last position;
		ResetCurrentPosition();
	}
   void Start()
   {
      m_currentPosition = transform.position;
   }
   public void UpdateScript()
   {
      m_targetPos = m_target.position + m_offset;
      // transform.position = Vector3.Lerp(transform.position, m_targetPos, Time.deltaTime * m_speed);

      //Smooth current position;
      m_currentPosition = Smooth(m_currentPosition, m_targetPos, m_xSpeed, m_ySpeed, m_zSpeed);

      //Set position;
      transform.position = m_currentPosition;
   }
   // void LateUpdate()
   // {
   //    m_targetPos = m_target.position + m_offset;
   //    // transform.position = Vector3.Lerp(transform.position, m_targetPos, Time.deltaTime * m_speed);

   //    //Smooth current position;
   //    m_currentPosition = Smooth(m_currentPosition, m_targetPos, m_speed);

   //    //Set position;
   //    transform.position = m_currentPosition;
   // }
   
   Vector3 Smooth(Vector3 _start, Vector3 _target, float xSmooth, float ySmooth, float zSmooth)
	{
		//Convert local position offset to world coordinates;
		Vector3 _offset = transform.localToWorldMatrix * localPositionOffset;

		//Add local position offset to target;
		_target += _offset;

      Vector3 lerpPos;
      lerpPos.x = Mathf.Lerp(_start.x, _target.x, xSmooth);
      lerpPos.y = Mathf.Lerp(_start.y, _target.y, ySmooth);
      lerpPos.z = Mathf.Lerp(_start.z, _target.z, zSmooth);
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
