using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDataOptional
{

    public Coroutine m_animCorout;

    [Range(0, 100)] public float m_animPercent = 0;

    public Action<Vector3> onUpdateVector3 { get; set; }
    public Action<Quaternion> onUpdateQuaternion { get; set; }
    public Action<float> onUpdateFloat { get; set; }
    public Action<Color> onUpdateColor { get; set; }
	public Action onComplete { get; set; }

}
