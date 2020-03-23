using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class PoolTracker : MonoBehaviour {
    
    ObjectPooler m_objectPooler;

    PoolType m_poolType;
    public PoolType PoolType{
        get{
            return m_poolType;
        }
        set{
            m_poolType = value;
        }
    }

    EnemyType m_enemyType;
    public EnemyType EnemyType{
        get{
            return m_enemyType;
        }
        set{
            m_enemyType = value;
        }
    }

    ProjectileType m_projectileType;
    public ProjectileType ProjectileType
    {
        get{
            return m_projectileType;
        }
        set{
            m_projectileType = value;
        }
    }

    ObjectType m_objectType;
    public ObjectType ObjectType{
        get{
            return m_objectType;
        }
        set{
            m_objectType = value;
        }
    }

    void Start(){
        m_objectPooler = ObjectPooler.Instance;
    }

    public void ResetTrackedObject(){
        if(m_objectPooler == null){
            m_objectPooler = ObjectPooler.Instance;
        }
        switch (m_poolType){
            case PoolType.EnemyType:
		        m_objectPooler.ReturnEnemyToPool(m_enemyType, gameObject);
            break;
            case PoolType.ProjectileType:
		        m_objectPooler.ReturnProjectileToPool(m_projectileType, gameObject);
            break;
            case PoolType.ObjectType:
		        m_objectPooler.ReturnObjectToPool(m_objectType, gameObject);
            break;
        }
        Destroy(this);
    }
    
}
