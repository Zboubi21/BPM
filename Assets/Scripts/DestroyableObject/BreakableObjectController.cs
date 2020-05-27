using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class BreakableObjectController : DestroyableObjectController
{

    [SerializeField] GameObject m_baseMesh;
    // [SerializeField] GameObject m_breakMesh;
    [SerializeField] DestroyableObjectController[] m_linkedDestroyableObjects;
    [SerializeField] bool m_justeUseForwardForce = false;
    [SerializeField] bool m_useControllerForward = true;
    [SerializeField] float m_breakForce = 3;
    [SerializeField] float m_upForce = 3;

    [SerializeField] FxType m_fxToSpawn;
    [SerializeField] Transform m_customSpawn;

    [SerializeField] Probabilities[] m_breakMesh;
    [System.Serializable] class Probabilities
    {
        public GameObject m_mesh;
        [Range(0, 100)] public int m_prob;
    }

    [SerializeField] Sounds m_impactSounds;
    
    protected override void On_ObjectIsBreak()
    {
        On_BreakObject();
        Rigidbody[] rbody = GetComponentsInChildren<Rigidbody>();
        GameManager.Instance.AddScore(GameManager.Instance.scoreSystem.destroyEnvironements.destroySecondCategorie);

        for (int i = 0, l = rbody.Length; i < l; ++i)
        {
            // rbody[i].AddForce(rbody[i].transform.position + (-rbody[i].transform.up * m_breakForce));
            // rbody[i].AddForce(rbody[i].transform.position + (rbody[i].transform.up * m_breakForce));

            Vector3 direction = m_useControllerForward ? rbody[i].transform.forward : rbody[i].transform.right;
            Vector3 force = m_justeUseForwardForce ? direction * m_breakForce : ((rbody[i].transform.position - transform.position).normalized * m_breakForce) + (rbody[i].transform.forward * m_upForce);
            rbody[i].AddForce(force);

            // int alea = Random.Range(0, 100);
            // if (alea <= 50)
            //     rbody[i].AddForce(rbody[i].transform.position + (transform.forward * m_breakForce));
            // if (alea > 50)
            //     rbody[i].AddForce(rbody[i].transform.position + (-transform.forward * m_breakForce));
        }
    }

    void On_BreakObject()
    {
        StartSoundFromArray(m_impactSounds.m_audioSource, m_impactSounds.m_sounds, m_impactSounds.m_volume, m_impactSounds.m_volumeRandomizer, m_impactSounds.m_pitch, m_impactSounds.m_pitchRandomizer);
        // m_baseMesh.SetActive(false);
        Destroy(m_baseMesh);
        // m_breakMesh.SetActive(true);
        int meshChosen = ChoseMesh();
        m_breakMesh[meshChosen].m_mesh.SetActive(true);
        for (int i = 0, l = m_breakMesh.Length; i < l; ++i)
        {
            if (i != meshChosen)
            {
                Destroy(m_breakMesh[i].m_mesh);
            }
        }

        if (m_linkedDestroyableObjects != null)
            if (m_linkedDestroyableObjects.Length != 0)
                for (int i = 0, l = m_linkedDestroyableObjects.Length; i < l; ++i)
                {
                    if (m_linkedDestroyableObjects[i] != null)
                        m_linkedDestroyableObjects[i].TakeDamage(10);
                }

        Vector3 pos = m_customSpawn == null ? transform.position : m_customSpawn.position;
        Quaternion rot = m_customSpawn == null ? Quaternion.identity : m_customSpawn.rotation;
        ObjectPooler.Instance?.SpawnFXFromPool(m_fxToSpawn, pos, rot);
    }

    int ChoseMesh()
    {
        int[] probs = new int[m_breakMesh.Length];
        for (int i = 0, l = probs.Length; i < l; ++i) {
            probs[i] = m_breakMesh[i].m_prob;
        }

        int total = 0;

        foreach (int elem in probs) {
            total += elem;
        }

        float randomPoint = UnityEngine.Random.value * total;

        for (int i= 0; i < probs.Length; i++) {
            if (randomPoint < probs[i]) {
                return i;
            }
            else {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }

}
