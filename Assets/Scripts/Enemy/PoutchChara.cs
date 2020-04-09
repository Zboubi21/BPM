using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PoutchChara : EnemyCara
{
    
    [Header("Poutch Variables")]
    [SerializeField] bool m_canvasFollowMainCam = true;
    [SerializeField] Transform m_canvas;
    [SerializeField] TextMeshProUGUI m_lifepoint;
    [SerializeField] float m_timeToDie = 1;
    [SerializeField] bool m_dieAt0Lifepoint = false;
    
    Transform m_mainCamera;
    Animator m_animator;

    protected override void Start()
    {
        base.Start();
        m_mainCamera = Camera.main.GetComponent<Transform>();
        m_animator = GetComponent<Animator>();
        UpdateLifebar();
    }
    void LateUpdate()
    {
        if (m_mainCamera != null && m_canvasFollowMainCam)
            m_canvas.transform.LookAt(m_mainCamera);
    }

    public override void TakeDamage(float damage, int i, bool hasToBeElectricalStun, float timeForElectricalStun)
    {
        if (IsDead)
            return;
        base.TakeDamage(damage, i, hasToBeElectricalStun, timeForElectricalStun);
        UpdateLifebar();
        CheckIfDead();
    }

    void UpdateLifebar()
    {
        if (m_lifepoint != null)
            m_lifepoint.text =  _currentLife.ToString();
    }

    protected override void CheckIfDead()
    {
        bool needToDie = false;

        if (m_dieAt0Lifepoint && _currentLife <= 0)
            needToDie = true;

        if (!m_dieAt0Lifepoint && _currentLife < 0)
            needToDie = true;

        if (needToDie)
        {
            IsDead = true;
            StartCoroutine(Die());
        }
    }

    public void KillPoutch()
    {
        _currentLife = 0;
        CheckIfDead();
    }

    IEnumerator Die()
    {
        m_animator.SetBool("isDead", IsDead);
        GameObject.Instantiate(gameObject, transform.position, transform.rotation);
        yield return new WaitForSeconds(m_timeToDie);
        Destroy(gameObject);
    }
    
}
