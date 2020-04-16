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
        // Scene[] scenes = SceneManager.GetAllScenes();

        // List<Scene> scenes = new List<Scene>();

        StartCoroutine(Coucou());

        // int sceneNbr = 0;
        // for (int i = 0, l = SceneManager.sceneCount; i < l; ++i)
        // {
        //     // scenes.Add(SceneManager.GetSceneAt(i));
        //     Scene scene = SceneManager.GetSceneAt(i);
        //     if (scene.name != "SceneLoader")
        //     {
        //         sceneNbr ++;
        //         SceneManager.UnloadSceneAsync(scene.buildIndex);
        //         // SceneManager.LoadScene(scene.buildIndex, LoadSceneMode.Additive);
        //     }
        // }
        // for (int i = 0, l = sceneNbr; i < l; ++i)
        // {
        //     Scene scene = SceneManager.GetSceneAt(i);
        //     if (scene.name != "SceneLoader")
        //     {
        //         SceneManager.LoadScene(scene.buildIndex, LoadSceneMode.Additive);
        //     }
        // }
    }

    IEnumerator Coucou()
    {
        SceneManager.UnloadSceneAsync("LD_Gameplay");
        SceneManager.UnloadSceneAsync("LD_Lighting");
        yield return new WaitForSeconds(3);
        SceneManager.LoadSceneAsync("LD_Gameplay", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("LD_Lighting", LoadSceneMode.Additive);
    }

}
