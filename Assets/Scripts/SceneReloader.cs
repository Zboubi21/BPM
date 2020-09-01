﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    
#region Singleton
	public static SceneReloader s_instance;
	void Awake(){
		if(s_instance == null){
			s_instance = this;
            DontDestroyOnLoad(gameObject);
		}else{
			// Debug.LogError("Two instance of SceneReloader");
			gameObject.SetActive(false);
            Destroy(gameObject);
		}
    }
#endregion Singleton
    
    public void On_ResetLvl()
    {
        // StartCoroutine(LoadLvl());

        // ObjectPooler pool = ObjectPooler.Instance;
        // if (pool != null)
        //     GameObject.Destroy(pool.gameObject);
        // SceneManager.LoadScene(0);
        
        Application.Quit();
    }

    public void TryNotShotDownTheGame()
    {
        if (ObjectPooler.Instance)
            Destroy(ObjectPooler.Instance.gameObject);
        SceneManager.LoadScene(0);
    }

    IEnumerator LoadLvl()
    {
        ObjectPooler.Instance?.On_ReturnAllInPool();

        AsyncOperation loadGameplay = SceneManager.UnloadSceneAsync("LD_Gameplay");
        AsyncOperation loadLighting = SceneManager.UnloadSceneAsync("LD_Lighting");

        while(!loadGameplay.isDone && !loadLighting.isDone)
        {
            yield return null;
        }

        AsyncOperation newLoadGameplay = SceneManager.LoadSceneAsync("LD_Gameplay", LoadSceneMode.Additive);
        AsyncOperation newLoadLighting = SceneManager.LoadSceneAsync("LD_Lighting", LoadSceneMode.Additive);

        while(!newLoadGameplay.isDone && !newLoadLighting.isDone)
        {
            yield return null;
        }

        PlayerController.s_instance?.GetComponent<PlayerDelayScene>().On_StartPlayer();
    }

    public void LeaveGame()
    {
        Application.Quit();
    }

}
