using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderEnemyDeclencher : MonoBehaviour
{

    [Header("Animations")]
    [SerializeField] protected bool m_useAnim = false;
    [SerializeField] protected Animator m_animator;
    [SerializeField] Anim m_spawnAnim;
    [SerializeField] Anim m_stunAnim;
    [SerializeField] Anim m_dieAnim;
    [SerializeField] Anim m_runAnim;

    [System.Serializable] protected class Anim
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

    protected SimpleEnemySpawnerShaderController m_shaderController;
    protected EnemySpawnerShaderController m_suicidalShaderController;
    bool m_showWeakSpot = false;
    bool m_isStun = false;
    bool m_isLowLife = false;

    protected void Start()
    {
        m_shaderController = GetComponent<SimpleEnemySpawnerShaderController>();
        m_suicidalShaderController = GetComponent<EnemySpawnerShaderController>();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(m_spawnKey))
        {
            if (m_useAnim)
                m_animator.Play(m_spawnAnim.m_name, m_spawnAnim.m_layer);
            m_shaderController?.On_StartSpawnShader();
            m_suicidalShaderController?.On_StartSpawnShader();
        }

        if (Input.GetKeyDown(m_weakSpotKey))
        {
            m_showWeakSpot = !m_showWeakSpot;
            m_shaderController?.On_ShowWeakSpot(m_showWeakSpot);
            // m_suicidalShaderController?.On_(m_showWeakSpot);
        }

        if (Input.GetKeyDown(m_stunKey))
        {
            m_isStun = !m_isStun;
            if (m_useAnim)
                m_animator.Play(m_stunAnim.m_name, m_stunAnim.m_layer);
            m_shaderController?.On_EnemyIsStun(m_isStun);
            m_suicidalShaderController?.On_EnemyIsStun(m_isStun);
        }

        if (Input.GetKeyDown(m_lowLifeKey))
        {
            m_isLowLife = !m_isLowLife;
            m_shaderController?.On_EnemyIsLowLife(m_isLowLife);
            m_suicidalShaderController?.On_EnemyIsLowLife(m_isLowLife);
        }

        if (Input.GetKeyDown(m_dissolveKey))
        {
            if (m_useAnim)
                m_animator.Play(m_dieAnim.m_name, m_dieAnim.m_layer);
            m_shaderController?.On_StartDissolveShader();
            m_suicidalShaderController?.On_StartDissolveShader();
        }

        if (Input.GetKeyDown(m_disintegrationKey))
        {
            if (m_useAnim)
                m_animator.Play(m_dieAnim.m_name, m_dieAnim.m_layer);
            m_shaderController?.On_StartDisintegrationShader();
            m_suicidalShaderController?.On_StartDisintegrationShader();
        }

        if (Input.GetKeyDown(m_runKey))
        {
            if (m_useAnim)
                m_animator.Play(m_runAnim.m_name, m_runAnim.m_layer);
        }
    }

}