using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderSuicidalEnemyDeclencher : ShaderEnemyDeclencher
{
    
    [Space]
    [Header("Suicidal Enemy")]
    [SerializeField] Anim m_goingToExplodAnim;
    [SerializeField] KeyCode m_goingToExplodKey = KeyCode.Alpha8;

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(m_goingToExplodKey))
        {
            if (m_useAnim)
                m_animator.Play(m_goingToExplodAnim.m_name, m_goingToExplodAnim.m_layer);
            // m_shaderController?.On_();
        }
    }

}
