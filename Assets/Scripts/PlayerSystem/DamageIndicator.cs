using UnityEngine;
using UnityEngine.UI;
using PoolTypes;

[RequireComponent(typeof(RectTransform))]
public class DamageIndicator : MonoBehaviour
{
    
    [SerializeField] float m_timeToShow = 2;

    Animator m_animator;
    RectTransform m_myTrans;
    Transform m_player;
    Transform m_target;

    bool m_isActive = false;

    void Awake()
    {
        m_myTrans = GetComponent<RectTransform>();
        m_animator = GetComponent<Animator>();
    }
    void OnEnable()
    {
        Invoke("On_StopShowIndicator", m_timeToShow);
        PlayerController player = PlayerController.s_instance;
        if (player != null)
        {
            bool onFury = player.GetComponent<BPMSystem>().IsCurrentlyOnFury;
            if (onFury)
                m_animator.SetTrigger("StartFury");
            else
                m_animator.SetTrigger("StartNoFury");
        }
    }

    public void SetupIndicator(Transform player, Transform target)
    {
        m_player = player;
        m_target = target;
        m_isActive = true;
    }
    
    void LateUpdate()
    {
        if (!m_isActive)
            return;
        
        Vector3 direction = m_player.position - m_target.position;
        Quaternion tRot = Quaternion.LookRotation(direction);
        tRot.z = -tRot.y;
        tRot.x = 0;
        tRot.y = 0;
        Vector3 nortDirection = new Vector3(0, 0, m_player.eulerAngles.y);
        m_myTrans.localRotation = tRot * Quaternion.Euler(nortDirection);
    }

    void On_StopShowIndicator()
    {
        m_isActive = false;
        ObjectPooler.Instance.ReturnObjectToPool(ObjectType.DamageIndicator, gameObject);
    }

}
