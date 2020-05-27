using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderEnemyDeclencher : MonoBehaviour
{

    [Header("Animations")]
    [SerializeField] bool m_useAnim = false;
    [SerializeField] Animator m_animator;
    [SerializeField] Anim m_spawnAnim;
    [SerializeField] Anim m_stunAnim;
    [SerializeField] Anim m_dieAnim;
    [SerializeField] Anim m_runAnim;

    [System.Serializable] class Anim
    {
        public string m_name;
        public int m_layer = 0;
    }

    [Header("Key Code")]
    [SerializeField] KeyCode m_spawnKey = KeyCode.Alpha1;
    [SerializeField] KeyCode m_weakSpotKey = KeyCode.Alpha2;
    [SerializeField] KeyCode m_stunKey = KeyCode.Alpha3;
    [SerializeField] KeyCode m_lowLifeKey = KeyCode.Alpha4;
    [SerializeField] KeyCode m_dissolveKey = KeyCode.Alpha5;
    [SerializeField] KeyCode m_disintegrationKey = KeyCode.Alpha6;
    [SerializeField] KeyCode m_runKey = KeyCode.Alpha7;

    SimpleEnemySpawnerShaderController m_shaderController;
    bool m_showWeakSpot = false;
    bool m_isStun = false;
    bool m_isLowLife = false;

    void Start()
    {
        m_shaderController = GetComponent<SimpleEnemySpawnerShaderController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(m_spawnKey))
        {
            if (m_useAnim)
                m_animator.Play(m_spawnAnim.m_name, m_spawnAnim.m_layer);
            m_shaderController?.On_StartSpawnShader();
        }

        if (Input.GetKeyDown(m_weakSpotKey))
        {
            m_showWeakSpot = !m_showWeakSpot;
            m_shaderController?.On_ShowWeakSpot(m_showWeakSpot);
        }

        if (Input.GetKeyDown(m_stunKey))
        {
            m_isStun = !m_isStun;
            if (m_useAnim)
                m_animator.Play(m_stunAnim.m_name, m_stunAnim.m_layer);
            m_shaderController?.On_EnemyIsStun(m_isStun);
        }

        if (Input.GetKeyDown(m_lowLifeKey))
        {
            m_isLowLife = !m_isLowLife;
            m_shaderController?.On_EnemyIsLowLife(m_isLowLife);
        }

        if (Input.GetKeyDown(m_dissolveKey))
        {
            if (m_useAnim)
                m_animator.Play(m_dieAnim.m_name, m_dieAnim.m_layer);
            m_shaderController?.On_StartDissolveShader();
        }

        if (Input.GetKeyDown(m_disintegrationKey))
        {
            if (m_useAnim)
                m_animator.Play(m_dieAnim.m_name, m_dieAnim.m_layer);
            m_shaderController?.On_StartDisintegrationShader();
        }

        if (Input.GetKeyDown(m_runKey))
        {
            if (m_useAnim)
                m_animator.Play(m_runAnim.m_name, m_runAnim.m_layer);
        }
    }

}