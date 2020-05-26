using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class DisappearObjectController : DestroyableObjectController
{
    
    [SerializeField] float m_waitTimeToDestroy = 0.25f;
    [SerializeField] float m_ySpawnOffset = 0;
    [SerializeField] float m_gizmosRadius = 0.125f;
    [SerializeField] bool m_affVFX = true;
    [SerializeField] bool m_affSFX = true;
    [SerializeField] FxType m_fxType = FxType.DestroyableObjectSmall;
    [SerializeField] Sounds m_impactSounds;

    Renderer m_renderer;

    void Awake()
    {
        m_renderer = GetComponent<Renderer>();
    }

    protected override void On_ObjectIsBreak()
    {
        GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.destroyEnvironements.destroyThirdCategorie);
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + m_ySpawnOffset, transform.position.z);

        if (m_affVFX)
            ObjectPooler.Instance?.SpawnFXFromPool(m_fxType, spawnPos, transform.rotation);
        if (m_affSFX)
            StartSoundFromArray(m_impactSounds.m_audioSource, m_impactSounds.m_sounds, m_impactSounds.m_volume, m_impactSounds.m_volumeRandomizer, m_impactSounds.m_pitch, m_impactSounds.m_pitchRandomizer);

        StartCoroutine(WaitTimeToDestroy());
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(new Vector3 (transform.position.x, transform.position.y + m_ySpawnOffset, transform.position.z), m_gizmosRadius);
    }
    
    IEnumerator WaitTimeToDestroy()
    {
        if (m_renderer != null)
            m_renderer.enabled = false;
        yield return new WaitForSeconds(m_waitTimeToDestroy);
        Destroy(gameObject);
    }

}
