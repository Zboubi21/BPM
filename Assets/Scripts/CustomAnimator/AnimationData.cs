using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AnimationData
{

    AnimationDataOptional m_optional = new AnimationDataOptional();
    public AnimationDataOptional Optional { get => m_optional; }

    // public bool m_isRunning = false;

    // Vector2 m_vector2ata;
    Vector3 m_vector3Data;
    public Vector3 Vector3Data { set => m_vector3Data = value; }

    Quaternion m_quaternionData;
    public Quaternion QuaternionData { set => m_quaternionData = value; }

    float m_floatData;
    public float FloatData { set => m_floatData = value; }

    Color m_colorData;
    public Color ColorData { set => m_colorData = value; }

    float m_delay = 0;
    public float Delay { get => m_delay; }
    public AnimationData SetDelay(float delay)
    {
        m_delay = delay;
        return this;
    }

    AnimationCurve m_curve;
    public AnimationCurve Curve { get => m_curve; }
    public AnimationData SetCurve(AnimationCurve curve)
    {
        m_curve = curve;
        return this;
    }

    // On anim is update
    public AnimationData SetOnUpdate(Action<float> onUpdate)
    {
        m_optional.onUpdateFloat = onUpdate;
        return this;
    }
    public AnimationData SetOnUpdate(Action<Color> onUpdate)
    {
        m_optional.onUpdateColor = onUpdate;
        return this;
    }

    // On anim is finished
    public AnimationData SetOnComplete(Action onComplete)
    {
        m_optional.onComplete = onComplete;
        return this;
    }

}
