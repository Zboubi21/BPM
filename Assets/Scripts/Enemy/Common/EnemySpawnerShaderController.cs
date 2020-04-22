using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerShaderController : ChangeShaderValue
{

    [SerializeField] float m_meshHeight = 2;
    [SerializeField] Material m_shaderMaterial;
    [SerializeField] Transform m_spawnShaderPos;

    MeshRenderer[] m_meshes;
    List<Material> m_lastMaterials = new List<Material>();
    Material m_shaderMaterialInstance;

    protected override void Awake()
    {
        m_meshes = GetComponentsInChildren<MeshRenderer>();
        if (m_meshes == null)
            return;
        for (int i = 0, l = m_meshes.Length; i < l; ++i)
        {
            m_lastMaterials.Add(m_meshes[i].material);

            m_meshes[i].material = m_shaderMaterial;
            m_shaderMaterialInstance = m_shaderMaterial;
        }
    }
    protected override void SetupVariables()
    {
        m_currentFromValue = m_spawnShaderPos.position.y;
        m_currentToValue = m_spawnShaderPos.position.y + m_meshHeight;

        m_distanceFromTargetedValues = GetDistanceFromValues(m_currentFromValue, m_currentToValue);
        m_speedToFadeIn = m_distanceFromTargetedValues / m_timeToFadeIn;
        m_speedToFadeOff = m_distanceFromTargetedValues / m_timeToFadeOff;
    }

    public void On_StartToSpawn()
    {
        SetupChangeValue(true);
        SetupVariables();
        SetShaderValue(m_currentFromValue);
        SwitchValue();
    }
    
    protected override float GetShaderValue()
    {
        if (m_shaderMaterialInstance == null)
            return 0;
        else
            return m_shaderMaterialInstance.GetFloat(m_shaderParameter);
    }
    protected override void SetShaderValue(float newValue)
    {
        if (m_shaderMaterialInstance == null)
            return;
        m_shaderMaterialInstance.SetFloat(m_shaderParameter, newValue);
    }

    protected override void On_ChangeShaderValueCoroutIsDone()
    {
        base.On_ChangeShaderValueCoroutIsDone();
        if (m_meshes == null)
            return;
        for (int i = 0, l = m_meshes.Length; i < l; ++i)
        {
            m_meshes[i].material = m_lastMaterials[i];
        }
    }

}
