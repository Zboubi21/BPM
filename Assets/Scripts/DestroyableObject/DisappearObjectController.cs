using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class DisappearObjectController : DestroyableObjectController
{
    
    [SerializeField] float m_waitTimeToDestroy = 0.25f;
    [SerializeField] float m_ySpawnOffset = 0;
    [SerializeField] float m_gizmosRadius = 0.125f;
    [SerializeField] FxType m_fxType = FxType.DestroyableObjectSmall;

    protected override void On_ObjectIsBreak()
    {
        GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.destroyEnvironements.destroyThirdCategorie);
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + m_ySpawnOffset, transform.position.z);
        ObjectPooler.Instance?.SpawnFXFromPool(m_fxType, spawnPos, transform.rotation);
        StartCoroutine(WaitTimeToDestroy());
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(new Vector3 (transform.position.x, transform.position.y + m_ySpawnOffset, transform.position.z), m_gizmosRadius);
    }
    
    IEnumerator WaitTimeToDestroy()
    {
        yield return new WaitForSeconds(m_waitTimeToDestroy);
        Destroy(gameObject);
    }

}
