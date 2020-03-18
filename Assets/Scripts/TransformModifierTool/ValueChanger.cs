using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ValueChanger : SerializedMonoBehaviour
{

#region SerializeField Variables
    [SerializeField] protected ChangeValue[] m_valueChanger = new ChangeValue[1];
    
    [System.Serializable] protected class ChangeValue
    {
        [Title("VALUE CHANGER", "", TitleAlignments.Centered)]
        [DisableInEditorMode()] public int m_iD = 0;

        [Title("Parameters")]
        // [FoldoutGroup("Parameters", true)]
        public bool m_activeAtStart = false;
        
        // [FoldoutGroup("Parameters", true)]
        [EnumToggleButtons]
        public ValueType m_valueType = ValueType.Position;

        [Title("Value to change")]
        [EnumToggleButtons]
        public ChangeType m_changeType = ChangeType.Local;

        [EnableIf("m_setFromValueAtStart")]
        public Vector3 m_fromValue = new Vector3(0, 0, 0);
        public Vector3 m_toValue = new Vector3(0, 0, 1);

        [HideInEditorMode]
        public Vector3 m_targetedFromPos;
        [HideInEditorMode]
        public Vector3 m_targetedToPos;

        [Tooltip("Teleport the Transform to the From Value at start")]
        public bool m_setFromValueAtStart = false;

        [Title("Speed")]
        [EnumToggleButtons]
        public SpeedType m_speedType = SpeedType.Time;
        public float m_speed = 3;

        [Title("Curve")]
        public bool m_useCurve = true;
        [ShowIf("m_useCurve")] public AnimationCurve m_curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        [Space]
        public UnityEvent m_onChangeValueIsFinished;

        [Title("Gizmos")]
        public bool m_showGizmos = false;

        [EnumToggleButtons]
        public GizmosType m_gizmosType = GizmosType.Sphere;

        [ShowIf("m_gizmosType", GizmosType.Mesh)]
        public Mesh m_gizmosMesh;

        [ShowIf("m_showGizmos")]
        [EnumToggleButtons]
        public DrawGizmosType m_drawGizmosType = DrawGizmosType.NeedSelected;
        
        public bool m_useGizmosPreset = false;

        [ShowIf("m_useGizmosPreset")]
        public GizmosPreset m_gizmosPreset;

        [HideIfGroup("m_useGizmosPreset")]
        [BoxGroup("m_useGizmosPreset/Gizmos Parameters")]
        public bool m_isWire = false;

        [BoxGroup("m_useGizmosPreset/Gizmos Parameters")]
        [Range(0, 1)] public float m_sphereRadius = 0.1f;

        [BoxGroup("m_useGizmosPreset/Gizmos Parameters")]
        public Color m_fromColor = Color.blue;

        [BoxGroup("m_useGizmosPreset/Gizmos Parameters")]
        public Color m_toColor = Color.red;

        [BoxGroup("m_useGizmosPreset/Gizmos Parameters")]
        public Color m_lineColor = Color.yellow;

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
        Speed,
        Time,
    }
    public enum DrawGizmosType
    {
        AlwaysShow = 0,
        NeedSelected = 1,
    }
    public enum GizmosType
    {
        Sphere,
        Mesh,
    }
#endregion

#region Private Vatiables
    IEnumerator m_changePositionCorout;
    IEnumerator m_changeRotationCorout;
#endregion

#region Unity Functions
    void Start()
    {
        for (int i = 0, l = m_valueChanger.Length; i < l; ++i)
        {
            if (m_valueChanger[i].m_setFromValueAtStart)
                SetValue(i, m_valueChanger[i].m_fromValue);

            if (m_valueChanger[i].m_activeAtStart)
                On_StartValueChanger(i);
        }
    }
    void OnDrawGizmos()
    {
        for (int i = 0, l = m_valueChanger.Length; i < l; ++i)
        {
            ApplyVariablesInEditor(i);

            if (m_valueChanger[i].m_showGizmos && m_valueChanger[i].m_drawGizmosType == DrawGizmosType.AlwaysShow)
                CheckDrawGizmosType(i);
        }
    }
    void OnDrawGizmosSelected()
    {
        for (int i = 0, l = m_valueChanger.Length; i < l; ++i)
        {
            // ApplyVariablesInEditor(i);

            if (m_valueChanger[i].m_showGizmos && m_valueChanger[i].m_drawGizmosType == DrawGizmosType.NeedSelected)
                CheckDrawGizmosType(i);
        }
    }
    void ApplyVariablesInEditor(int valueChangerID)
    {
        m_valueChanger[valueChangerID].m_iD = valueChangerID;

        if (m_valueChanger[valueChangerID].m_iD > 0 && m_valueChanger[valueChangerID].m_valueType == ValueType.Position)
            m_valueChanger[valueChangerID].m_setFromValueAtStart = false;

        m_valueChanger[valueChangerID].m_targetedFromPos = GetTargetedFromPos(valueChangerID);
        m_valueChanger[valueChangerID].m_targetedToPos = GetTargetedToPos(valueChangerID);
    }
    Vector3 GetTargetedFromPos(int valueChangerID)
    {
        Vector3 targetedFromPosWithPreviousValueChanger = new Vector3();
        if (m_valueChanger[valueChangerID].m_changeType == ChangeType.World)
        {
            if (m_valueChanger.Length > 1)
            {
                if (valueChangerID > 0 && m_valueChanger[valueChangerID - 1].m_valueType == ValueType.Position && !m_valueChanger[valueChangerID].m_setFromValueAtStart)
                {
                    targetedFromPosWithPreviousValueChanger = m_valueChanger[valueChangerID - 1].m_targetedToPos;
                }
                else
                {
                    targetedFromPosWithPreviousValueChanger = transform.position;
                }
            }
            else
            {
                targetedFromPosWithPreviousValueChanger = transform.position;
            }
            return m_valueChanger[valueChangerID].m_setFromValueAtStart ? m_valueChanger[valueChangerID].m_fromValue : targetedFromPosWithPreviousValueChanger;
        }
        if (m_valueChanger[valueChangerID].m_changeType == ChangeType.Local)
        {
            if (m_valueChanger.Length > 1)
            {
                if (valueChangerID > 0 && m_valueChanger[valueChangerID - 1].m_valueType == ValueType.Position && !m_valueChanger[valueChangerID].m_setFromValueAtStart)
                {
                    targetedFromPosWithPreviousValueChanger = m_valueChanger[valueChangerID - 1].m_targetedToPos;
                }
                else
                {
                    // targetedFromPosWithPreviousValueChanger = transform.position - transform.localPosition + m_valueChanger[valueChangerID].m_fromValue;
                    targetedFromPosWithPreviousValueChanger = transform.position;
                }
            }
            else
            {
                targetedFromPosWithPreviousValueChanger = transform.position;
            }
            return m_valueChanger[valueChangerID].m_setFromValueAtStart ? transform.position - transform.localPosition + m_valueChanger[valueChangerID].m_fromValue : targetedFromPosWithPreviousValueChanger;
        }
        return new Vector3(0, 0, 0);
    }
    Vector3 GetTargetedToPos(int valueChangerID)
    {
        if (m_valueChanger[valueChangerID].m_changeType == ChangeType.World)
        {
            return m_valueChanger[valueChangerID].m_toValue;
        }
        if (m_valueChanger[valueChangerID].m_changeType == ChangeType.Local)
        {
            return transform.position - transform.localPosition + m_valueChanger[valueChangerID].m_toValue;
        }
        return new Vector3(0, 0, 0);
    }
    void CheckDrawGizmosType(int valueChangerID)
    {
        if (!m_valueChanger[valueChangerID].m_showGizmos || m_valueChanger[valueChangerID].m_useGizmosPreset && m_valueChanger[valueChangerID].m_gizmosPreset == null)
            return;

        Vector3 targetedFromPos = m_valueChanger[valueChangerID].m_targetedFromPos;
        Vector3 targetedToPos = m_valueChanger[valueChangerID].m_targetedToPos;

        Color fromColor = m_valueChanger[valueChangerID].m_useGizmosPreset ? m_valueChanger[valueChangerID].m_gizmosPreset.m_fromColor : m_valueChanger[valueChangerID].m_fromColor;
        Color toColor = m_valueChanger[valueChangerID].m_useGizmosPreset ? m_valueChanger[valueChangerID].m_gizmosPreset.m_toColor : m_valueChanger[valueChangerID].m_toColor;
        Color lineColor = m_valueChanger[valueChangerID].m_useGizmosPreset ? m_valueChanger[valueChangerID].m_gizmosPreset.m_lineColor : m_valueChanger[valueChangerID].m_lineColor;

        float sphereRadius = m_valueChanger[valueChangerID].m_useGizmosPreset ? m_valueChanger[valueChangerID].m_gizmosPreset.m_sphereRadius : m_valueChanger[valueChangerID].m_sphereRadius;

        bool isWire = m_valueChanger[valueChangerID].m_useGizmosPreset ? m_valueChanger[valueChangerID].m_gizmosPreset.m_isWire : m_valueChanger[valueChangerID].m_isWire;

        DrawGizmos(valueChangerID, targetedFromPos, targetedToPos, fromColor, toColor, lineColor, sphereRadius, isWire);
    }
    void DrawGizmos(int valueChangerID, Vector3 targetedFromPos, Vector3 targetedToPos, Color fromColor, Color toColor, Color lineColor, float sphereRadius, bool isWire)
    {
        if (m_valueChanger[valueChangerID].m_gizmosType == GizmosType.Mesh && m_valueChanger[valueChangerID].m_gizmosMesh != null)
        {
            DrawGizmosMesh(targetedFromPos, m_valueChanger[valueChangerID].m_gizmosMesh, isWire, fromColor);
            DrawGizmosMesh(targetedToPos, m_valueChanger[valueChangerID].m_gizmosMesh, isWire, toColor);
        }
        else
        {
            DrawGizmosSphere(targetedFromPos, sphereRadius, isWire, fromColor);
            DrawGizmosSphere(targetedToPos, sphereRadius, isWire, toColor);
        }

        Gizmos.color = lineColor;
        Gizmos.DrawLine(targetedFromPos, targetedToPos);
    }
    void DrawGizmosSphere(Vector3 center, float radius, bool isWire, Color color)
    {
        Gizmos.color = color;
        if (isWire)
            Gizmos.DrawWireSphere(center, radius);
        if (!isWire)
            Gizmos.DrawSphere(center, radius);
    }
    void DrawGizmosMesh(Vector3 center, Mesh mesh, bool isWire, Color color)
    {
        Gizmos.color = color;
        if (isWire)
            Gizmos.DrawWireMesh(mesh, center, transform.rotation, transform.lossyScale);
        if (!isWire)
            Gizmos.DrawMesh(mesh, center, transform.rotation, transform.lossyScale);
    }
#endregion

#region Private Functions
    IEnumerator PositionChanger(int valueChangerID)
    {
        Vector3 fromValue = new Vector3();
        if (m_valueChanger[valueChangerID].m_changeType == ChangeType.Local)
            fromValue = transform.localPosition;
        if (m_valueChanger[valueChangerID].m_changeType == ChangeType.World)
            fromValue = transform.position;

        float fracJourney = 0;
        float distance = Vector3.Distance(fromValue, m_valueChanger[valueChangerID].m_toValue);
        float speed = new float();
        Vector3 actualValue = fromValue;

        if (m_valueChanger[valueChangerID].m_speedType == SpeedType.Speed)
        {
            speed = m_valueChanger[valueChangerID].m_speed;
        }
        if (m_valueChanger[valueChangerID].m_speedType == SpeedType.Time)
        {
            speed = distance / m_valueChanger[valueChangerID].m_speed;
        }

        while (actualValue != m_valueChanger[valueChangerID].m_toValue)
        {
            fracJourney += (Time.deltaTime) * speed / distance;

            if (m_valueChanger[valueChangerID].m_useCurve)
            {
                actualValue = Vector3.Lerp(fromValue, m_valueChanger[valueChangerID].m_toValue, m_valueChanger[valueChangerID].m_curve.Evaluate(fracJourney));
            }
            else
            {
                actualValue = Vector3.Lerp(fromValue, m_valueChanger[valueChangerID].m_toValue, fracJourney);
            }

            SetValue(valueChangerID, actualValue);

            yield return null;
        }
        On_ChangeValueIsFinished(valueChangerID);
    }
    IEnumerator RotationChanger(int valueChangerID)
    {
        Vector3 fromValue = new Vector3();
        if (m_valueChanger[valueChangerID].m_changeType == ChangeType.Local)
            fromValue = transform.localRotation.eulerAngles;
        if (m_valueChanger[valueChangerID].m_changeType == ChangeType.World)
            fromValue = transform.rotation.eulerAngles;

        float fracJourney = 0;
        float distance = Vector3.Distance(fromValue, m_valueChanger[valueChangerID].m_toValue);
        float speed = new float();
        Vector3 actualValue = fromValue;

        if (m_valueChanger[valueChangerID].m_speedType == SpeedType.Speed)
        {
            speed = m_valueChanger[valueChangerID].m_speed;
        }
        if (m_valueChanger[valueChangerID].m_speedType == SpeedType.Time)
        {
            speed = distance / m_valueChanger[valueChangerID].m_speed;
        }

        while (actualValue != m_valueChanger[valueChangerID].m_toValue)
        {
            fracJourney += (Time.deltaTime) * speed / distance;

            if (m_valueChanger[valueChangerID].m_useCurve)
            {
                actualValue = Vector3.Lerp(fromValue, m_valueChanger[valueChangerID].m_toValue, m_valueChanger[valueChangerID].m_curve.Evaluate(fracJourney));
            }
            else
            {
                actualValue = Vector3.Lerp(fromValue, m_valueChanger[valueChangerID].m_toValue, fracJourney);
            }

            SetValue(valueChangerID, actualValue);

            yield return null;
        }
        On_ChangeValueIsFinished(valueChangerID);
    }

    void On_StartToMove(int valueChangerID)
    {
        if(m_changePositionCorout != null)
            StopCoroutine(m_changePositionCorout);
        m_changePositionCorout = PositionChanger(valueChangerID);
        StartCoroutine(m_changePositionCorout);
    }
    void On_StartToRotate(int valueChangerID)
    {
        if(m_changeRotationCorout != null)
            StopCoroutine(m_changeRotationCorout);
        m_changeRotationCorout = RotationChanger(valueChangerID);
        StartCoroutine(m_changeRotationCorout);
    }
    
    void On_ChangeValueIsFinished(int valueChangerID)
    {
        m_valueChanger[valueChangerID].m_onChangeValueIsFinished.Invoke();
    }

    void SetValue(int valueChangerID, Vector3 newValue)
    {
        if (m_valueChanger[valueChangerID].m_valueType == ValueType.Position)
        {
            if (m_valueChanger[valueChangerID].m_changeType == ChangeType.Local)
                transform.localPosition = newValue;
            if (m_valueChanger[valueChangerID].m_changeType == ChangeType.World)
                transform.position = newValue;
        }

        if (m_valueChanger[valueChangerID].m_valueType == ValueType.Rotation)
        {
            if (m_valueChanger[valueChangerID].m_changeType == ChangeType.Local)
                transform.localEulerAngles = newValue;
            if (m_valueChanger[valueChangerID].m_changeType == ChangeType.World)
                transform.eulerAngles = newValue;
        }
    }
#endregion

#region Public Functions
    public void On_StartValueChanger(int valueChangerID)
    {
        if (m_valueChanger[valueChangerID].m_valueType == ValueType.Position)
            On_StartToMove(valueChangerID);

        if (m_valueChanger[valueChangerID].m_valueType == ValueType.Rotation)
            On_StartToRotate(valueChangerID);
    }
    public int GetValueChangerLength()
    {
        return m_valueChanger.Length;
    }
#endregion

}