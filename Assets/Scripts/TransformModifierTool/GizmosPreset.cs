using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewGizmosPresets", menuName = "GizmosPresets")]
public class GizmosPreset : SerializedScriptableObject
{

    [BoxGroup("Gizmos Parameters")]
    public bool m_isWire = false;

    [BoxGroup("Gizmos Parameters"), Range(0, 1)] 
    public float m_sphereRadius = 0.1f;

    [BoxGroup("Gizmos Parameters")]
    public Color m_fromColor = Color.blue;

    [BoxGroup("Gizmos Parameters")]
    public Color m_toColor = Color.red;

    [BoxGroup("Gizmos Parameters")]
    public Color m_lineColor = Color.yellow;
    
}
