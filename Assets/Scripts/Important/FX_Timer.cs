using System.Collections;
using UnityEngine;
using PoolTypes;

public class FX_Timer : MonoBehaviour
{
    
    [SerializeField] float m_waitTimeToReset = 1;

    IEnumerator m_waitTimeToResetCorout;

    void OnEnable()
    {
        StartWaitTimeToReset(true);
    }
    void OnDisable()
    {
        StartWaitTimeToReset(false);
    }

    void StartWaitTimeToReset(bool start)
    {
        if (m_waitTimeToResetCorout != null)
            StopCoroutine(m_waitTimeToResetCorout);

        if (start)
        {
            m_waitTimeToResetCorout = WaitTimeToReset();
            StartCoroutine(m_waitTimeToResetCorout);
        }   
    }

    IEnumerator WaitTimeToReset()
    {
        yield return new WaitForSeconds(m_waitTimeToReset);
        On_TimerIsReached();
    }
    
    void On_TimerIsReached()
    {
        PoolTracker poolTracker = GetComponent<PoolTracker>();
        if (poolTracker != null)
        {
            ObjectPooler.Instance.ReturnFXToPool(poolTracker.FxType, gameObject);
        }
    }

}
