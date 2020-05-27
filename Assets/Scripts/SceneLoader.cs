 using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    
    [SerializeField] float m_waitTimeToLoadScenes = 3;
    [SerializeField] string[] m_scenesToLoad;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(WaitToLoadScenes());
    }

    AsyncOperation[] m_operations;
    bool[] m_operationsDone;
    IEnumerator WaitToLoadScenes()
    {
        yield return new WaitForSeconds(m_waitTimeToLoadScenes);
        // LoadScenes();
        if (m_scenesToLoad != null)
        {
            m_operations = new AsyncOperation[m_scenesToLoad.Length];
            m_operationsDone = new bool[m_scenesToLoad.Length];
            for (int i = 0, l = m_scenesToLoad.Length; i < l; ++i)
            {
                if (m_scenesToLoad[i] != null)
                    m_operations[i] = SceneManager.LoadSceneAsync(m_scenesToLoad[i], LoadSceneMode.Additive);
            }
            while (!OperationDone())
            {
                yield return null;
            }
            PlayerController.s_instance?.GetComponent<PlayerDelayScene>().On_StartPlayer();
            // SceneManager.UnloadSceneAsync("SceneLoader");
        }
    }

    bool OperationDone()
    {
        bool isDone = true;
        for (int i = 0, l = m_operations.Length; i < l; ++i)
        {
            if (!m_operations[i].isDone)
                isDone = false;
        }
        return isDone;
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
            PlayerController.s_instance?.GetComponent<PlayerDelayScene>().On_StartPlayer();
            SceneManager.UnloadSceneAsync("SceneLoader");
        }
    }
    
}
