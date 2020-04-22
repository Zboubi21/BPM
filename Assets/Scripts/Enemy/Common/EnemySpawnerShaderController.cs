using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerShaderController : ChangeShaderValue
{

    [Space]
    [Space]
    [Header("USE THIS PARAMETERS!")]

    [SerializeField] Spawn m_spawn;
    [Serializable] class Spawn
    {
        public string m_shaderParameter;
        public float m_meshHeight = 2.5f;
        public float m_timeToFadeShader = 1;
        public AnimationCurve m_curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public Material m_shaderMaterial;
        public Transform m_shaderPos;
    }
    
    [SerializeField] BaseShaderModifs m_disintegration;
    [SerializeField] BaseShaderModifs m_dissolve;
    [Serializable] class BaseShaderModifs
    {
        public string m_shaderParameter;
        public float m_fromValue = 0, m_toValue = 1;
        public float m_timeToFadeShader = 1;
        public AnimationCurve m_curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public Material m_shaderMaterial;
        public ParticleSystem m_particles;
    }

    MeshRenderer[] m_meshes;
    List<Material> m_startMaterials = new List<Material>();
    Material m_spawnShaderMaterialInstance;
    Material m_disintegrationShaderMaterialInstance;
    Material m_dissolveShaderMaterialInstance;

    ShaderState m_currentShaderState = ShaderState.Spawn;
    enum ShaderState
    {
        Spawn,
        Disintegration,
        Dissolve,
    }

    protected override void Awake()
    {
        m_spawnShaderMaterialInstance = m_spawn.m_shaderMaterial;
        m_disintegrationShaderMaterialInstance = m_disintegration.m_shaderMaterial;
        m_dissolveShaderMaterialInstance = m_dissolve.m_shaderMaterial;

        m_meshes = GetComponentsInChildren<MeshRenderer>();
        if (m_meshes == null)
            return;
        for (int i = 0, l = m_meshes.Length; i < l; ++i)
        {
            m_startMaterials.Add(m_meshes[i].material);

            m_meshes[i].material = m_spawnShaderMaterialInstance;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            On_StartDisintegrationShader();
        if (Input.GetKeyDown(KeyCode.B))
            On_StartDissolveShader();
    }
    protected override void SetupVariables()
    {
    }

    public void On_StartSpawnShader()
    {
        m_currentShaderState = ShaderState.Spawn;
        SetupChangeValue(true);

        // Setup Variables
        m_currentFromValue = m_spawn.m_shaderPos.position.y;
        m_currentToValue = m_spawn.m_shaderPos.position.y + m_spawn.m_meshHeight;
        m_distanceFromTargetedValues = GetDistanceFromValues(m_currentFromValue, m_currentToValue);
        m_curve = m_spawn.m_curve;
        m_speedToFadeIn = m_distanceFromTargetedValues / m_spawn.m_timeToFadeShader;

        SetShaderValue(m_currentFromValue);
        SwitchValue(true);
    }
    public void On_StartDisintegrationShader()
    {
        for (int i = 0, l = m_meshes.Length; i < l; ++i)
        {
            m_meshes[i].material = m_disintegrationShaderMaterialInstance;
        }

        m_currentShaderState = ShaderState.Disintegration;
        SetupChangeValue(true);

        // Setup Variables
        m_currentFromValue = m_disintegration.m_fromValue;
        m_currentToValue = m_disintegration.m_toValue;
        m_distanceFromTargetedValues = GetDistanceFromValues(m_currentFromValue, m_currentToValue);
        m_curve = m_disintegration.m_curve;
        m_speedToFadeIn = m_distanceFromTargetedValues / m_disintegration.m_timeToFadeShader;

        SetShaderValue(m_currentFromValue);
        SwitchValue(true);

        m_disintegration.m_particles.Play(true);
    }
    public void On_StartDissolveShader()
    {
        for (int i = 0, l = m_meshes.Length; i < l; ++i)
        {
            m_meshes[i].material = m_dissolveShaderMaterialInstance;
        }

        m_currentShaderState = ShaderState.Dissolve;
        SetupChangeValue(true);

        // Setup Variables
        m_currentFromValue = m_dissolve.m_fromValue;
        m_currentToValue = m_dissolve.m_toValue;
        m_distanceFromTargetedValues = GetDistanceFromValues(m_currentFromValue, m_currentToValue);
        m_curve = m_dissolve.m_curve;
        m_speedToFadeIn = m_distanceFromTargetedValues / m_dissolve.m_timeToFadeShader;

        SetShaderValue(m_currentFromValue);
        SwitchValue(true);

        m_dissolve.m_particles.Play(true);
    }
    
    protected override float GetShaderValue()
    {
        switch (m_currentShaderState)
        {
            case ShaderState.Spawn:
                return m_spawnShaderMaterialInstance.GetFloat(m_spawn.m_shaderParameter);
            case ShaderState.Disintegration:
                return m_disintegrationShaderMaterialInstance.GetFloat(m_disintegration.m_shaderParameter);
            case ShaderState.Dissolve:
                return m_dissolveShaderMaterialInstance.GetFloat(m_dissolve.m_shaderParameter);
        }
        return 0;
    }
    protected override void SetShaderValue(float newValue)
    {
        switch (m_currentShaderState)
        {
            case ShaderState.Spawn:
                m_spawnShaderMaterialInstance.SetFloat(m_spawn.m_shaderParameter, newValue);
            break;
            case ShaderState.Disintegration:
                m_disintegrationShaderMaterialInstance.SetFloat(m_disintegration.m_shaderParameter, newValue);
            break;
            case ShaderState.Dissolve:
                m_dissolveShaderMaterialInstance.SetFloat(m_dissolve.m_shaderParameter, newValue);
            break;
        }
    }

    protected override void On_ChangeShaderValueCoroutIsDone()
    {
        base.On_ChangeShaderValueCoroutIsDone();

        if (m_currentShaderState == ShaderState.Spawn)
        {
            if (m_meshes == null)
                return;
            for (int i = 0, l = m_meshes.Length; i < l; ++i)
            {
                m_meshes[i].material = m_startMaterials[i];
            }
        }
    }

}
