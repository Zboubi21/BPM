using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class SimpleEnemySpawnerShaderController : ChangeShaderValue
{

    [Space]
    [Space]
    [Header("USE THIS PARAMETERS!")]
    [SerializeField] Renderer[] m_renderersToChangeMat;

    [SerializeField] Spawn m_spawn;
    [Serializable] class Spawn
    {
        public string m_shaderParameter;
        public float m_meshHeight = 2.5f;
        public float m_timeToFadeShader = 1;
        public AnimationCurve m_curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public Material[] m_shaderMaterial;
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
        public Material[] m_shaderMaterial;
        public ParticleSystem m_ps;
    }

    [SerializeField] WeakSpots m_weakSpots;
    [Serializable] class WeakSpots
    {
        public int[] m_renderersIndex;
        public Material[] m_shaderMaterial;
    }

    [SerializeField] Material[] m_stunShaderMaterial;

    List<Material> m_startMaterials = new List<Material>();
    Material[] m_spawnShaderMaterialInstance;
    Material[] m_disintegrationShaderMaterialInstance;
    Material[] m_dissolveShaderMaterialInstance;

    Material[] m_weakSpotMaterialInstance;
    Material[] m_stunShaderMaterialInstance;

    ShaderState m_currentShaderState = ShaderState.Spawn;
    enum ShaderState
    {
        Spawn,
        Disintegration,
        Dissolve,
    }

    protected override void Awake()
    {
        m_spawnShaderMaterialInstance = new Material[m_spawn.m_shaderMaterial.Length];
        SetupShaderInstance(m_spawnShaderMaterialInstance, m_spawn.m_shaderMaterial);

        m_disintegrationShaderMaterialInstance = new Material[m_disintegration.m_shaderMaterial.Length];
        SetupShaderInstance(m_disintegrationShaderMaterialInstance, m_disintegration.m_shaderMaterial);

        m_dissolveShaderMaterialInstance = new Material[m_dissolve.m_shaderMaterial.Length];
        SetupShaderInstance(m_dissolveShaderMaterialInstance, m_dissolve.m_shaderMaterial);

        m_weakSpotMaterialInstance = new Material[m_weakSpots.m_shaderMaterial.Length];
        SetupShaderInstance(m_weakSpotMaterialInstance, m_weakSpots.m_shaderMaterial);

        m_stunShaderMaterialInstance = new Material[m_stunShaderMaterial.Length];
        SetupShaderInstance(m_stunShaderMaterialInstance, m_stunShaderMaterial);
    }
    protected override void Start()
    {
        if (m_renderersToChangeMat != null)
        {
            for (int i = 0, l = m_renderersToChangeMat.Length; i < l; ++i)
            {
                m_startMaterials.Add(m_renderersToChangeMat[i].material);

                m_renderersToChangeMat[i].material = m_spawnShaderMaterialInstance[i];
            }
        }
    }
    void SetupShaderInstance(Material[] shaderInstance, Material[] shader)
    {
        for (int i = 0, l = shader.Length; i < l; ++i)
        {
            shaderInstance[i] = new Material(shader[i]);
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

        if (m_renderersToChangeMat != null)
        {
            for (int i = 0, l = m_renderersToChangeMat.Length; i < l; ++i)
            {
                m_renderersToChangeMat[i].material = m_spawnShaderMaterialInstance[i];
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
        if (m_renderersToChangeMat != null)
        {
            for (int i = 0, l = m_renderersToChangeMat.Length; i < l; ++i)
            {
                m_renderersToChangeMat[i].material = m_disintegrationShaderMaterialInstance[i];
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

        // ObjectPooler.Instance.SpawnFXFromPool(m_disintegration.m_fx, m_disintegration.m_spawnFxTrans.position, m_disintegration.m_spawnFxTrans.rotation);
        m_disintegration.m_ps?.Play(true);
    }
    public void On_StartDissolveShader()
    {
        StartCoroutine(WaitTimeToStartDissolveShader());
    }
    IEnumerator WaitTimeToStartDissolveShader()
    {
        yield return new WaitForSeconds(m_dissolve.m_waitTimeToDissolve);
        
        if (m_renderersToChangeMat != null)
        {
            for (int i = 0, l = m_renderersToChangeMat.Length; i < l; ++i)
            {
                m_renderersToChangeMat[i].material = m_dissolveShaderMaterialInstance[i];
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

        // ObjectPooler.Instance.SpawnFXFromPool(m_dissolve.m_fx, m_dissolve.m_spawnFxTrans.position, m_dissolve.m_spawnFxTrans.rotation);
        m_dissolve.m_ps?.Play(true);
    }

    public void On_ShowWeakSpot(bool show)
    {
        if (show)
        {
            if (m_weakSpots.m_renderersIndex != null)
                for (int i = 0, l = m_weakSpots.m_renderersIndex.Length; i < l; ++i)
                    m_renderersToChangeMat[m_weakSpots.m_renderersIndex[i]].material = m_weakSpotMaterialInstance[i];
        }
        else
        {
            if (m_weakSpots.m_renderersIndex != null)
                for (int i = 0, l = m_weakSpots.m_renderersIndex.Length; i < l; ++i)
                    m_renderersToChangeMat[m_weakSpots.m_renderersIndex[i]].material = m_startMaterials[m_weakSpots.m_renderersIndex[i]];
        }
    }

    public void On_EnemyIsStun(bool isStun)
    {
        if (isStun)
        {
            if (m_renderersToChangeMat != null)
                for (int i = 0, l = m_renderersToChangeMat.Length; i < l; ++i)
                    m_renderersToChangeMat[i].material = m_stunShaderMaterialInstance[i];
        }
        else
        {
            if (m_renderersToChangeMat != null)
                for (int i = 0, l = m_renderersToChangeMat.Length; i < l; ++i)
                    m_renderersToChangeMat[i].material = m_startMaterials[i];
        }
    }
    
    protected override float GetShaderValue()
    {
        switch (m_currentShaderState)
        {
            case ShaderState.Spawn:
                return m_spawnShaderMaterialInstance[0].GetFloat(m_spawn.m_shaderParameter);
            case ShaderState.Disintegration:
                return m_disintegrationShaderMaterialInstance[0].GetFloat(m_disintegration.m_shaderParameter);
            case ShaderState.Dissolve:
                return m_dissolveShaderMaterialInstance[0].GetFloat(m_dissolve.m_shaderParameter);
        }
        return 0;
    }
    protected override void SetShaderValue(float newValue)
    {
        switch (m_currentShaderState)
        {
            case ShaderState.Spawn:
                SetShadersValue(m_spawnShaderMaterialInstance, m_spawn.m_shaderParameter, newValue);
            break;
            case ShaderState.Disintegration:
                SetShadersValue(m_disintegrationShaderMaterialInstance, m_disintegration.m_shaderParameter, newValue);
            break;
            case ShaderState.Dissolve:
                SetShadersValue(m_dissolveShaderMaterialInstance, m_dissolve.m_shaderParameter, newValue);
            break;
        }
    }
    void SetShadersValue(Material[] shader, string parameters, float value)
    {
        for (int i = 0, l = shader.Length; i < l; ++i)
        {
            shader[i].SetFloat(parameters, value);
        }
    }

    protected override void On_ChangeShaderValueCoroutIsDone()
    {
        base.On_ChangeShaderValueCoroutIsDone();

        if (m_currentShaderState == ShaderState.Spawn)
        {
            if (m_renderersToChangeMat != null)
            {
                for (int i = 0, l = m_renderersToChangeMat.Length; i < l; ++i)
                {
                    m_renderersToChangeMat[i].material = m_startMaterials[i];
                }
            }
        }
    }

}
