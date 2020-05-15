using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class DisappearObjectController : DestroyableObjectController
{
    
    [SerializeField] float m_waitTimeToDestroy = 0.25f;
    [SerializeField] Transform m_spawnTrans;
    [SerializeField] FxType m_fxType = FxType.DestroyableObjectSmall;

    protected override void On_ObjectIsBreak()
    {
        GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.destroyEnvironements.destroyThirdCategorie);
        Transform spawnTrans = m_spawnTrans != null ? m_spawnTrans : transform;
        ObjectPooler.Instance?.SpawnFXFromPool(m_fxType, spawnTrans.position, spawnTrans.rotation);
        StartCoroutine(WaitTimeToDestroy());
    }
    
    IEnumerator WaitTimeToDestroy()
    {
        yield return new WaitForSeconds(m_waitTimeToDestroy);
        Destroy(gameObject);
    }

}
