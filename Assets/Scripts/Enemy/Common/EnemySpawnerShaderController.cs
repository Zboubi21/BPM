using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class EnemySpawnerShaderController : ChangeShaderValue
{

    [Space]
    [Space]
    [Header("USE THIS PARAMETERS!")]

    [SerializeField] MeshRenderer[] m_meshesToChangeMat;
    [SerializeField] MeshRenderer[] m_alternativeMeshesToChangeMat;
    [SerializeField] SkinnedMeshRenderer[] m_skinnedMeshesToChangeMat;

    [SerializeField] Spawn m_spawn;
    [Serializable] class Spawn
    {
        public string m_shaderParameter;
        public float m_meshHeight = 2.5f;
        public float m_timeToFadeShader = 1;
        public AnimationCurve m_curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public Material m_shaderMaterial;
        public Material m_alternativeShaderMaterial;
        public Transform m_shaderPos;
        public FxType m_fx = FxType.SuicidalEnemy_Spawn;
        public Transform m_spawnFxTrans;
    }
    
    [SerializeField] BaseShaderModifs m_disintegration;
    [SerializeField] BaseShaderModifs m_dissolve;
    [Serializable] class BaseShaderModifs
    {
        public string m_shaderParameter;
        public float m_fromValue = 0, m_toValue = 1;
        public float m_timeToFadeShader = 1;
        public float m_waitTimeToDissolve = 0.5f;
        public AnimationCurve m_curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public Material m_shaderMaterial;
        public Material m_alternativeShaderMaterial;
        public FxType m_fx = FxType.SuicidalEnemy_Spawn;
        public Transform m_spawnFxTrans;
    }

    List<Material> m_startMaterials = new List<Material>();
    List<Material> m_alternativeStartMaterials = new List<Material>();
    Material m_spawnShaderMaterialInstance;
    Material m_alternativeSpawnShaderMaterialInstance;
    Material m_disintegrationShaderMaterialInstance;
    Material m_alternativeDisintegrationShaderMaterialInstance;
    Material m_dissolveShaderMaterialInstance;
    Material m_alternativeDissolveShaderMaterialInstance;

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
        m_alternativeSpawnShaderMaterialInstance = m_spawn.m_alternativeShaderMaterial;

        m_disintegrationShaderMaterialInstance = m_disintegration.m_shaderMaterial;
        m_alternativeDisintegrationShaderMaterialInstance = m_disintegration.m_alternativeShaderMaterial;

        m_dissolveShaderMaterialInstance = m_dissolve.m_shaderMaterial;
        m_alternativeDissolveShaderMaterialInstance = m_dissolve.m_alternativeShaderMaterial;

        if (m_meshesToChangeMat != null)
        {
            for (int i = 0, l = m_meshesToChangeMat.Length; i < l; ++i)
            {
                m_startMaterials.Add(m_meshesToChangeMat[i].material);

                m_meshesToChangeMat[i].material = m_spawnShaderMaterialInstance;
            }
        }
        if (m_alternativeMeshesToChangeMat != null)
        {
            for (int i = 0, l = m_alternativeMeshesToChangeMat.Length; i < l; ++i)
            {
                m_alternativeStartMaterials.Add(m_alternativeMeshesToChangeMat[i].material);

                m_alternativeMeshesToChangeMat[i].material = m_alternativeSpawnShaderMaterialInstance;
            }
        }
        if (m_skinnedMeshesToChangeMat != null)
        {
            for (int i = 0, l = m_skinnedMeshesToChangeMat.Length; i < l; ++i)
            {
                m_startMaterials.Add(m_skinnedMeshesToChangeMat[i].material);

                m_skinnedMeshesToChangeMat[i].material = m_spawnShaderMaterialInstance;
            }
        }
    }
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.V))
        // if (Input.GetKeyDown(KeyCode.B))
    }
    protected override void SetupVariables()
    {
    }

    public void On_StartSpawnShader()
    {
        ObjectPooler.Instance.SpawnFXFromPool(m_spawn.m_fx, m_spawn.m_spawnFxTrans.position, m_spawn.m_spawnFxTrans.rotation);

        if (m_meshesToChangeMat != null)
        {
            for (int i = 0, l = m_meshesToChangeMat.Length; i < l; ++i)
            {
                m_meshesToChangeMat[i].material = m_spawnShaderMaterialInstance;
            }
        }
        if (m_alternativeMeshesToChangeMat != null)
        {
            for (int i = 0, l = m_alternativeMeshesToChangeMat.Length; i < l; ++i)
            {
                m_alternativeMeshesToChangeMat[i].material = m_alternativeSpawnShaderMaterialInstance;
            }
        }
        if (m_skinnedMeshesToChangeMat != null)
        {
            for (int i = 0, l = m_skinnedMeshesToChangeMat.Length; i < l; ++i)
            {
                m_skinnedMeshesToChangeMat[i].material = m_spawnShaderMaterialInstance;
            }
        }

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
        if (m_meshesToChangeMat != null)
        {
            for (int i = 0, l = m_meshesToChangeMat.Length; i < l; ++i)
            {
                m_meshesToChangeMat[i].material = m_disintegrationShaderMaterialInstance;
            }
        }
        if (m_alternativeMeshesToChangeMat != null)
        {
            for (int i = 0, l = m_alternativeMeshesToChangeMat.Length; i < l; ++i)
            {
                m_alternativeMeshesToChangeMat[i].material = m_alternativeDisintegrationShaderMaterialInstance;
            }
        }
        if (m_skinnedMeshesToChangeMat != null)
        {
            for (int i = 0, l = m_skinnedMeshesToChangeMat.Length; i < l; ++i)
            {
                m_skinnedMeshesToChangeMat[i].material = m_disintegrationShaderMaterialInstance;
            }
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

        ObjectPooler.Instance.SpawnFXFromPool(m_disintegration.m_fx, m_disintegration.m_spawnFxTrans.position, m_disintegration.m_spawnFxTrans.rotation);
    }
    public void On_StartDissolveShader()
    {
        StartCoroutine(WaitTimeToStartDissolveShader());
    }
    IEnumerator WaitTimeToStartDissolveShader()
    {
        yield return new WaitForSeconds(m_dissolve.m_waitTimeToDissolve);

        if (m_meshesToChangeMat != null)
        {
            for (int i = 0, l = m_meshesToChangeMat.Length; i < l; ++i)
            {
                m_meshesToChangeMat[i].material = m_dissolveShaderMaterialInstance;
            }
        }
        if (m_alternativeMeshesToChangeMat != null)
        {
            for (int i = 0, l = m_alternativeMeshesToChangeMat.Length; i < l; ++i)
            {
                m_alternativeMeshesToChangeMat[i].material = m_alternativeDissolveShaderMaterialInstance;
            }
        }
        if (m_skinnedMeshesToChangeMat != null)
        {
            for (int i = 0, l = m_skinnedMeshesToChangeMat.Length; i < l; ++i)
            {
                m_skinnedMeshesToChangeMat[i].material = m_dissolveShaderMaterialInstance;
            }
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

        ObjectPooler.Instance.SpawnFXFromPool(m_dissolve.m_fx, m_dissolve.m_spawnFxTrans.position, m_dissolve.m_spawnFxTrans.rotation);
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
                m_alternativeSpawnShaderMaterialInstance.SetFloat(m_spawn.m_shaderParameter, newValue);
            break;
            case ShaderState.Disintegration:
                m_disintegrationShaderMaterialInstance.SetFloat(m_disintegration.m_shaderParameter, newValue);
                m_alternativeDisintegrationShaderMaterialInstance.SetFloat(m_disintegration.m_shaderParameter, newValue);
            break;
            case ShaderState.Dissolve:
                m_dissolveShaderMaterialInstance.SetFloat(m_dissolve.m_shaderParameter, newValue);
                m_alternativeDissolveShaderMaterialInstance.SetFloat(m_dissolve.m_shaderParameter, newValue);
            break;
        }
    }

    protected override void On_ChangeShaderValueCoroutIsDone()
    {
        base.On_ChangeShaderValueCoroutIsDone();

        if (m_currentShaderState == ShaderState.Spawn)
        {
            if (m_meshesToChangeMat != null)
            {
                for (int i = 0, l = m_meshesToChangeMat.Length; i < l; ++i)
                {
                    m_meshesToChangeMat[i].material = m_startMaterials[i];
                }
            }
            if (m_alternativeMeshesToChangeMat != null)
            {
                for (int i = 0, l = m_alternativeMeshesToChangeMat.Length; i < l; ++i)
                {
                    m_alternativeMeshesToChangeMat[i].material = m_alternativeStartMaterials[i];
                }
            }
            if (m_skinnedMeshesToChangeMat != null)
            {
                for (int i = 0, l = m_skinnedMeshesToChangeMat.Length; i < l; ++i)
                {
                    m_skinnedMeshesToChangeMat[i].material = m_startMaterials[i];
                }
            }
        }
    }

}
