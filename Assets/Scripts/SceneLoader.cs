 using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    
    [SerializeField] float m_waitTimeToLoadScenes = 3;
    [SerializeField] string[] m_scenesToLoad;

    void Start()
    {
        StartCoroutine(WaitToLoadScenes());
    }

    IEnumerator WaitToLoadScenes()
    {
        yield return new WaitForSeconds(m_waitTimeToLoadScenes);
        LoadScenes();
    }

    void LoadScenes()
    {
        if (m_scenesToLoad != null)
        {
            for (int i = 0, l = m_scenesToLoad.Length; i < l; ++i)
            {
                if (m_scenesToLoad[i] != null)
                    // SceneManager.LoadScene(m_scenesToLoad[i], LoadSceneMode.Additive);
                    SceneManager.LoadSceneAsync(m_scenesToLoad[i], LoadSceneMode.Additive);
            }
        }
    }
    
}
