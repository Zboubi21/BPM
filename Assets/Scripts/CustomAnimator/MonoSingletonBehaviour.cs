using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingletonBehaviour <T> : MonoBehaviour where T : MonoBehaviour
{
    
    public static T Instance;

    [Header("Singleton Parameters")]

    [Tooltip("If true, the game object arn't be destroyed whend scene is unloaded")]
    [SerializeField] bool m_dontDestroyOnLoad = false;

    [Tooltip("Show an error if more than 1 Instance is detected")]
    [SerializeField] bool m_showInstanceError = false;

    protected virtual void Awake()
    {
        SetupSingleton();
    }
    protected virtual void SetupSingleton()
    {
        if(Instance == null){
            Instance = this as T;
            if (m_dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
		}else{
            if (m_showInstanceError)
			    Debug.LogError("Two instance of " + this);
            Destroy(gameObject);
		}
    }
    
}
