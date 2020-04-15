using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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


    #region get set
    public List<GameObject> AllUsedCover { get => allUsedCover; set => allUsedCover = value; }
    public List<WaveScreenController> WaveScreenControllers { get => waveScreenControllers; set => waveScreenControllers = value; }
    public int CurrentScore { get => _currentScore; set => _currentScore = value; }

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


    public void AddScore(int scoreAdded)
    {
        _currentScore += scoreAdded;
        if (waveScreenControllers.Count > 0)
        {
            for (int i = 0, l = waveScreenControllers.Count; i < l; ++i)
            {
                waveScreenControllers[i].AddScore(scoreAdded, _currentScore);
            }
        }

    }
}
