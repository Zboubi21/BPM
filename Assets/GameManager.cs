using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Two instance of GameManager");
        }
    }
    #endregion

    [SerializeField] bool m_showRespawnPos = true;
    [SerializeField] Transform m_respawnPos;

    #region get set
    public List<GameObject> AllUsedCover { get => allUsedCover; set => allUsedCover = value; }
    public List<WaveScreenController> WaveScreenControllers { get => waveScreenControllers; set => waveScreenControllers = value; }
    public int CurrentScore { get => _currentScore; set => _currentScore = value; }
    public Transform RespawnPos { get => m_respawnPos; }

    #endregion

    #region Enemy DataStocking
    List<GameObject> allUsedCover = new List<GameObject>();
    #endregion

    #region Wave Screen DataStocking
    List<WaveScreenController> waveScreenControllers = new List<WaveScreenController>();
    int _currentScore;
    #endregion

    public void AddScreen(WaveScreenController controller)
    {
        waveScreenControllers.Add(controller);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            AddScore(100);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            AddScore(10);
        }
#endif
    }
    public void AddScore(int scoreAdded)
    {
        _currentScore += scoreAdded;
        RefreshAllScreen(scoreAdded, _currentScore);
    }

    public void ResetScore()
    {
        _currentScore = 0;
        RefreshAllScreen(0, _currentScore);
    }

    public void RemoveScore(int scoreToRemove)
    {
        _currentScore -= scoreToRemove;
        RefreshAllScreen(scoreToRemove, _currentScore);
    }

    void RefreshAllScreen(int score, int currentScore)
    {
        if (waveScreenControllers.Count > 0)
        {
            for (int i = 0, l = waveScreenControllers.Count; i < l; ++i)
            {
                waveScreenControllers[i].RefreshScore(score, _currentScore);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (m_showRespawnPos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_respawnPos.position, 0.5f);
        }
    }

}
