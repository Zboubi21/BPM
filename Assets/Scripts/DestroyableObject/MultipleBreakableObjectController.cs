using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleBreakableObjectController : DestroyableObjectController
{
    
    [SerializeField] GameObject m_breakableObjects;
    [SerializeField] int m_nbrOfBreakableObject = 1;
    [SerializeField] int m_newColliderLayerNbr = 16;

    List<Collider> m_colliders = new List<Collider>();
    int m_currentBreakObjectNbr = 0;

    protected override void Start()
    {
        base.Start();
        Collider[] col = m_breakableObjects.GetComponentsInChildren<Collider>();
        for (int i = 0, l = col.Length; i < l; ++i)
        {
            m_colliders.Add(col[i]);
        }
    }

    protected override void On_ObjectTakeDamage()
    {
        if (m_colliders.Count == 0 || m_currentBreakObjectNbr >= m_nbrOfBreakableObject)
            return;

        m_currentBreakObjectNbr ++;

        int alea = Random.Range(0, m_colliders.Count);
        m_colliders[alea].gameObject.layer = m_newColliderLayerNbr;
        m_colliders[alea].enabled = true;
        m_colliders[alea].GetComponent<Rigidbody>().isKinematic = false;
        m_colliders.RemoveAt(alea);
    }

    protected override void On_ObjectIsBreak()
    {
        base.On_ObjectIsBreak();
        GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.destroyEnvironements.destroyFourthCategorie);
    }

}
