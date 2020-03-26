using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class ObjectPooler : MonoBehaviour {

#region Singleton

	public static ObjectPooler Instance;

	void Awake(){
		if(Instance == null){
			Instance = this;
            DontDestroyOnLoad(gameObject);
		}else{
			// Debug.LogError("Two instance of ObjectPooler");
			gameObject.SetActive(false);
            Destroy(gameObject);
		}
        CreateAllPools();
    }

#endregion Singleton

	[Header("Enemy pools")]
	[SerializeField] List<EnemyPool> m_enemyPools;
	[System.Serializable] public class EnemyPool {
        public string m_name;
        public EnemyType m_enemyType;
        public GameObject m_prefab;
		public int m_size;
    }

	[Header("Projectile pools")]
	[SerializeField] List<ProjectilPool> m_projectilPools;
	[System.Serializable] public class ProjectilPool {
        public string m_name;
        public ProjectileType m_projectileType;
        public GameObject m_prefab;
		public int m_size;
    }
    [Header("Fx pools")]
    [SerializeField] List<FXPool> m_FXPools;
    [System.Serializable]
    public class FXPool
    {
        public string m_name;
        public FxType m_fxType;
        public GameObject m_prefab;
        public int m_size;
    }

    [Header("Object pools")]
	[SerializeField] List<ObjectPool> m_objectPools;
	[System.Serializable] public class ObjectPool {
        public string m_name;
        public ObjectType m_objectType;
        public GameObject m_prefab;
		public int m_size;
    }

	[Space]

	[Header("Pool test")]
	[SerializeField] bool m_usePoolTest = true;
	[SerializeField] Transform m_spawnPool;
	[SerializeField] PoolTest[] m_poolTest;
	[System.Serializable] public class PoolTest{
        public KeyCode m_input;
        public EnemyType m_objectToSpawn;
    }

	[SerializeField] ObjectPoolTest[] m_objectPoolTest;
	[System.Serializable] public class ObjectPoolTest{
        public KeyCode m_input;
        public ObjectType m_objectToSpawn;
    }

	Dictionary<EnemyType, Queue<GameObject>> m_enemyPoolDictionary;
	Dictionary<ProjectileType, Queue<GameObject>> m_projectilePoolDictionary;
	Dictionary<FxType, Queue<GameObject>> m_FXPoolDictionary;
	Dictionary<ObjectType, Queue<GameObject>> m_objectPoolDictionary;

	Queue<PoolTracker> m_trackedObject = new Queue<PoolTracker>();

	void CreateAllPools(){
		m_enemyPoolDictionary = new Dictionary<EnemyType, Queue<GameObject>>();
		foreach(EnemyPool pool in m_enemyPools){
			Queue<GameObject> objectPool = new Queue<GameObject>();
			for(int i = 0, l = pool.m_size; i < l; ++i){
				GameObject obj = Instantiate(pool.m_prefab, transform, this);
				obj.SetActive(false);
				obj.name = obj.name + "_" + i;
				objectPool.Enqueue(obj);
			}
			m_enemyPoolDictionary.Add(pool.m_enemyType, objectPool);
		}

		m_projectilePoolDictionary = new Dictionary<ProjectileType, Queue<GameObject>>();
		foreach(ProjectilPool pool in m_projectilPools){
			Queue<GameObject> objectPool = new Queue<GameObject>();
			for(int i = 0, l = pool.m_size; i < l; ++i){
				GameObject obj = Instantiate(pool.m_prefab, transform, this);
				obj.SetActive(false);
				obj.name = obj.name + "_" + i;
				objectPool.Enqueue(obj);
			}
			m_projectilePoolDictionary.Add(pool.m_projectileType, objectPool);
		}

        m_FXPoolDictionary = new Dictionary<FxType, Queue<GameObject>>();
        foreach (FXPool pool in m_FXPools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0, l = pool.m_size; i < l; ++i)
            {
                GameObject obj = Instantiate(pool.m_prefab, transform, this);
                obj.SetActive(false);
                obj.name = obj.name + "_" + i;
                objectPool.Enqueue(obj);
            }
            m_FXPoolDictionary.Add(pool.m_fxType, objectPool);
        }

        m_objectPoolDictionary = new Dictionary<ObjectType, Queue<GameObject>>();
		foreach(ObjectPool pool in m_objectPools){
			Queue<GameObject> objectPool = new Queue<GameObject>();
			for(int i = 0, l = pool.m_size; i < l; ++i){
				GameObject obj = Instantiate(pool.m_prefab, transform, this);
				obj.SetActive(false);
				obj.name = obj.name + "_" + i;
				objectPool.Enqueue(obj);
			}
			m_objectPoolDictionary.Add(pool.m_objectType, objectPool);
		}
	}

	void Update(){
		if(m_usePoolTest){
			for (int i = 0, l = m_poolTest.Length; i < l; ++i){
				if(Input.GetKeyDown(m_poolTest[i].m_input)){
					if(m_spawnPool != null){
						SpawnEnemyFromPool(m_poolTest[i].m_objectToSpawn, m_spawnPool.position, m_spawnPool.rotation);
					}else{
						SpawnEnemyFromPool(m_poolTest[i].m_objectToSpawn, Vector3.zero, Quaternion.identity);
					}
				}
			}
			for (int i = 0, l = m_objectPoolTest.Length; i < l; ++i){
				if(Input.GetKeyDown(m_objectPoolTest[i].m_input)){
					if(m_spawnPool != null){
						SpawnObjectFromPool(m_objectPoolTest[i].m_objectToSpawn, m_spawnPool.position, m_spawnPool.rotation);
					}else{
						SpawnObjectFromPool(m_objectPoolTest[i].m_objectToSpawn, Vector3.zero, Quaternion.identity);
					}
				}
			}
		}
    }

	public GameObject SpawnEnemyFromPool(EnemyType enemyType, Vector3 position, Quaternion rotation){

		if(!m_enemyPoolDictionary.ContainsKey(enemyType)){
			Debug.LogWarning("Pool of " + enemyType + " dosen't exist.");
			return null;
		}

		if(m_enemyPoolDictionary[enemyType].Count == 0){
			Debug.LogError(enemyType.ToString() + " pool is empty!");
			return null;
		}

		GameObject objectToSpawn = m_enemyPoolDictionary[enemyType].Dequeue();

		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;
		objectToSpawn.SetActive(true);

		PoolTracker poolTracker = AddPoolTrackerComponent(objectToSpawn, PoolType.EnemyType);
		poolTracker.EnemyType = enemyType;
		m_trackedObject.Enqueue(poolTracker);

		return objectToSpawn;
	}
	public void ReturnEnemyToPool(EnemyType enemyType, GameObject objectToReturn){
		CheckPoolTrackerOnResetObject(objectToReturn);
		objectToReturn.SetActive(false);
		m_enemyPoolDictionary[enemyType].Enqueue(objectToReturn);
	}


	public GameObject SpawnProjectileFromPool(ProjectileType projectileType, Vector3 position, Quaternion rotation){

		if(!m_projectilePoolDictionary.ContainsKey(projectileType)){
			Debug.LogError("Pool of " + projectileType + " dosen't exist.");
			return null;
		}

		if(m_projectilePoolDictionary[projectileType].Count == 0){
			Debug.LogError(projectileType.ToString() + " pool is empty!");
			return null;
		}

		GameObject objectToSpawn = m_projectilePoolDictionary[projectileType].Dequeue();

		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;
		objectToSpawn.SetActive(true);

		PoolTracker poolTracker = AddPoolTrackerComponent(objectToSpawn, PoolType.ProjectileType);
		poolTracker.ProjectileType = projectileType;
		m_trackedObject.Enqueue(poolTracker);

		return objectToSpawn;
	}
	public void ReturnProjectileToPool(ProjectileType objectType, GameObject objectToReturn){
		CheckPoolTrackerOnResetObject(objectToReturn);
		objectToReturn.SetActive(false);
		m_projectilePoolDictionary[objectType].Enqueue(objectToReturn);
	}

    public GameObject SpawnFXFromPool(FxType FXType, Vector3 position, Quaternion rotation)
    {

        if (!m_FXPoolDictionary.ContainsKey(FXType))
        {
            Debug.LogError("Pool of " + FXType + " dosen't exist.");
            return null;
        }

        if (m_FXPoolDictionary[FXType].Count == 0)
        {
            Debug.LogError(FXType.ToString() + " pool is empty!");
            return null;
        }

        GameObject objectToSpawn = m_FXPoolDictionary[FXType].Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        PoolTracker poolTracker = AddPoolTrackerComponent(objectToSpawn, PoolType.FxType);
        poolTracker.FxType = FXType;
        m_trackedObject.Enqueue(poolTracker);

        return objectToSpawn;
    }
    public void ReturnFXToPool(FxType objectType, GameObject objectToReturn)
    {
        CheckPoolTrackerOnResetObject(objectToReturn);
        objectToReturn.SetActive(false);
        m_FXPoolDictionary[objectType].Enqueue(objectToReturn);
    }

    public GameObject SpawnObjectFromPool(ObjectType objectType, Vector3 position, Quaternion rotation){

		if(!m_objectPoolDictionary.ContainsKey(objectType)){
			Debug.LogError("Pool of " + objectType + " dosen't exist.");
			return null;
		}

		if(m_objectPoolDictionary[objectType].Count == 0){
			Debug.LogError(objectType.ToString() + " pool is empty!");
			return null;
		}

		GameObject objectToSpawn = m_objectPoolDictionary[objectType].Dequeue();

		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;
		objectToSpawn.SetActive(true);

		PoolTracker poolTracker = AddPoolTrackerComponent(objectToSpawn, PoolType.ObjectType);
		poolTracker.ObjectType = objectType;
		m_trackedObject.Enqueue(poolTracker);

		return objectToSpawn;
	}
	public void ReturnObjectToPool(ObjectType objectType, GameObject objectToReturn){
		CheckPoolTrackerOnResetObject(objectToReturn);
		objectToReturn.SetActive(false);
		m_objectPoolDictionary[objectType].Enqueue(objectToReturn);
	}

	PoolTracker AddPoolTrackerComponent(GameObject objectToSpawn, PoolType poolType){
		// PoolTracker poolTracker = objectToSpawn.GetComponent<PoolTracker>();
		// if(poolTracker == null){
			PoolTracker poolTracker = objectToSpawn.AddComponent<PoolTracker>().GetComponent<PoolTracker>();
		// }
		poolTracker.PoolType = poolType;
		return poolTracker;
	}
	
	public void On_ReturnAllInPool(){
		for (int i = 0, l = m_trackedObject.Count; i < l; ++i) {
			PoolTracker poolTracker = m_trackedObject.Dequeue();
			if(poolTracker != null){
				poolTracker.ResetTrackedObject();
			}
		}
	}

	void CheckPoolTrackerOnResetObject(GameObject objectToReturn){
		PoolTracker poolTracker = objectToReturn.GetComponent<PoolTracker>();
        if(poolTracker != null){
            Destroy(poolTracker);
        }
	}

}
