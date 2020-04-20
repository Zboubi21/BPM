using System.Collections;
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
        StartCoroutine(LoadLvl());
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

        SceneManager.LoadSceneAsync("LD_Gameplay", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("LD_Lighting", LoadSceneMode.Additive);
    }

}
