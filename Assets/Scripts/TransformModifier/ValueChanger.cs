using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ValueChanger : MonoBehaviour
{

#region SerializeField Variables
    [Header("Value Changer")]
    [SerializeField] protected ChangeValue m_valueChanger;
    
    [System.Serializable] protected class ChangeValue
    {
        [Header("Parameters")]
        public bool m_activeAtStart = false;
        public ValueType m_valueType = ValueType.Position;
        public bool m_setFromValueAtStart = true;

        [Header("Value to change")]
        public ChangeType m_changeType = ChangeType.Local;
        public Vector3 m_fromValue;
        public Vector3 m_toValue;

        [Header("Speed")]
        public SpeedType m_speedType = SpeedType.TimeToChangeValue;
        public float m_speed = 1;

        [Header("Curve")]
        public bool m_useCurve = true;
        public AnimationCurve m_curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        // [Space]
        public UnityEvent m_onChangeValueIsFinished;
    }
#endregion

#region Enum
    public enum ValueType  // public ? protected ? dans un autre script ?
    {
        Position,
        Rotation,
    }
    public enum ChangeType  // public ? protected ? dans un autre script ?
    {
        Local,
        World,
    }
    public enum SpeedType  // public ? protected ? dans un autre script ?
    {
        SpeedValue,
        TimeToChangeValue,
    }
#endregion

#region Private Vatiables
    IEnumerator m_changePositionCorout;
    IEnumerator m_changeRotationCorout;
#endregion

#region Event Functions
    void Start()
    {
        if (m_valueChanger.m_setFromValueAtStart)
            SetValue(m_valueChanger.m_fromValue);

        if (m_valueChanger.m_activeAtStart)
            On_StartValueChanger();
    }
#endregion

#region Private Functions
    IEnumerator PositionChanger()
    {
        Vector3 fromValue = new Vector3();
        if (m_valueChanger.m_changeType == ChangeType.Local)
            fromValue = transform.localPosition;
        if (m_valueChanger.m_changeType == ChangeType.World)
            fromValue = transform.position;

        float fracJourney = 0;
        float distance = Vector3.Distance(fromValue, m_valueChanger.m_toValue);
        float speed = new float();
        Vector3 actualValue = fromValue;

        if (m_valueChanger.m_speedType == SpeedType.SpeedValue)
        {
            speed = m_valueChanger.m_speed;
        }
        if (m_valueChanger.m_speedType == SpeedType.TimeToChangeValue)
        {
            speed = distance / m_valueChanger.m_speed;
        }

        while (actualValue != m_valueChanger.m_toValue)
        {
            fracJourney += (Time.deltaTime) * speed / distance;

            if (m_valueChanger.m_useCurve)
            {
                actualValue = Vector3.Lerp(fromValue, m_valueChanger.m_toValue, m_valueChanger.m_curve.Evaluate(fracJourney));
            }
            else
            {
                actualValue = Vector3.Lerp(fromValue, m_valueChanger.m_toValue, fracJourney);
            }

            SetValue(actualValue);

            yield return null;
        }
        On_ChangeValueIsFinished();
    }
    IEnumerator RotationChanger()
    {
        Vector3 fromValue = new Vector3();
        if (m_valueChanger.m_changeType == ChangeType.Local)
            fromValue = transform.localRotation.eulerAngles;
        if (m_valueChanger.m_changeType == ChangeType.World)
            fromValue = transform.rotation.eulerAngles;

        float fracJourney = 0;
        float distance = Vector3.Distance(fromValue, m_valueChanger.m_toValue);
        float speed = new float();
        Vector3 actualValue = fromValue;

        if (m_valueChanger.m_speedType == SpeedType.SpeedValue)
        {
            speed = m_valueChanger.m_speed;
        }
        if (m_valueChanger.m_speedType == SpeedType.TimeToChangeValue)
        {
            speed = distance / m_valueChanger.m_speed;
        }

        while (actualValue != m_valueChanger.m_toValue)
        {
            fracJourney += (Time.deltaTime) * speed / distance;

            if (m_valueChanger.m_useCurve)
            {
                actualValue = Vector3.Lerp(fromValue, m_valueChanger.m_toValue, m_valueChanger.m_curve.Evaluate(fracJourney));
            }
            else
            {
                actualValue = Vector3.Lerp(fromValue, m_valueChanger.m_toValue, fracJourney);
            }

            SetValue(actualValue);

            yield return null;
        }
        On_ChangeValueIsFinished();
    }

    void On_StartToMove()
    {
        if(m_changePositionCorout != null)
            StopCoroutine(m_changePositionCorout);
        m_changePositionCorout = PositionChanger();
        StartCoroutine(m_changePositionCorout);
    }
    void On_StartToRotate()
    {
        if(m_changeRotationCorout != null)
            StopCoroutine(m_changeRotationCorout);
        m_changeRotationCorout = RotationChanger();
        StartCoroutine(m_changeRotationCorout);
    }
    
    void On_ChangeValueIsFinished()
    {
        m_valueChanger.m_onChangeValueIsFinished.Invoke();
    }

    void SetValue(Vector3 newValue)
    {
        if (m_valueChanger.m_valueType == ValueType.Position)
        {
            if (m_valueChanger.m_changeType == ChangeType.Local)
                transform.localPosition = newValue;
            if (m_valueChanger.m_changeType == ChangeType.World)
                transform.position = newValue;
        }

        if (m_valueChanger.m_valueType == ValueType.Rotation)
        {
            if (m_valueChanger.m_changeType == ChangeType.Local)
                transform.localEulerAngles = newValue;
            if (m_valueChanger.m_changeType == ChangeType.World)
                transform.eulerAngles = newValue;
        }
    }
#endregion

#region Public Functions
    public void On_StartValueChanger()
    {
        if (m_valueChanger.m_valueType == ValueType.Position)
            On_StartToMove();

        if (m_valueChanger.m_valueType == ValueType.Rotation)
            On_StartToRotate();
    }
#endregion

}