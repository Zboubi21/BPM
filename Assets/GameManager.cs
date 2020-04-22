using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
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

    public ScoreSystem scoreSystem;


    [Serializable] public class ScoreSystem
    {
        public KillSomething killSomething;
        public HitSomething hitSomething;
        public OverAdrenaline overAdrenaline;
        public DoAMultiKill multiKill;
        public DestroyEnvironements destroyEnvironements;
        public EndOfLevel endOfLevel;
    }
    //public Text killCount;
    //public Image mutliKillTimerUnderFive;
    //public Image mutliKillTimerOverFive;

    float _currentTimeBetweenKillUnderFive;
    float _currentTimeBetweenKillOverFive;
    float _currentTimeBetweenKillOnFury;
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

    private void Start()
    {
        _currentTimeBetweenKillUnderFive = scoreSystem.multiKill.timeBetweenEachKillUnderFive;
        _currentTimeBetweenKillOverFive = scoreSystem.multiKill.timeBetweenEachKillOverFive;
        _currentTimeBetweenKillOnFury = scoreSystem.overAdrenaline.timeBetweenEachKillOnFury;
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
        if(_currentTimeBetweenKillOverFive != scoreSystem.multiKill.timeBetweenEachKillOverFive)
        {
            if(_currentTimeBetweenKillOverFive <= scoreSystem.multiKill.timeBetweenEachKillOverFive)
            {
                _currentTimeBetweenKillOverFive += Time.deltaTime;
                isOnTime = true;
            }
            else if(_currentTimeBetweenKillUnderFive == scoreSystem.multiKill.timeBetweenEachKillUnderFive)
            {
                _currentTimeBetweenKillOverFive = scoreSystem.multiKill.timeBetweenEachKillOverFive;
                isOnTime = false;
            }
        }

        if (_currentTimeBetweenKillUnderFive != scoreSystem.multiKill.timeBetweenEachKillUnderFive)
        {
            if (_currentTimeBetweenKillUnderFive <= scoreSystem.multiKill.timeBetweenEachKillUnderFive)
            {
                _currentTimeBetweenKillUnderFive += Time.deltaTime;
                isOnTime = true;
            }
            else if(_currentTimeBetweenKillOverFive == scoreSystem.multiKill.timeBetweenEachKillOverFive)
            {
                _currentTimeBetweenKillUnderFive = scoreSystem.multiKill.timeBetweenEachKillUnderFive;
                isOnTime = false;
            }
        }

        if (_currentTimeBetweenKillOnFury != scoreSystem.overAdrenaline.timeBetweenEachKillOnFury)
        {
            if (_currentTimeBetweenKillOnFury <= scoreSystem.overAdrenaline.timeBetweenEachKillOnFury)
            {
                _currentTimeBetweenKillOnFury += Time.deltaTime;
                isOnTimeOnFury = true;
            }
            else if (_currentTimeBetweenKillOnFury == scoreSystem.overAdrenaline.timeBetweenEachKillOnFury)
            {
                _currentTimeBetweenKillOnFury = scoreSystem.overAdrenaline.timeBetweenEachKillOnFury;
                isOnTimeOnFury = false;
            }
        }
        // mutliKillTimerUnderFive.fillAmount = Mathf.InverseLerp(0, scoreSystem.multiKill.timeBetweenEachKillUnderFive, _currentTimeBetweenKillUnderFive);
        // mutliKillTimerOverFive.fillAmount = Mathf.InverseLerp(0, scoreSystem.multiKill.timeBetweenEachKillOverFive, _currentTimeBetweenKillOverFive);
    }

    bool isOnTime;
    bool isOnTimeOnFury;
    int _currentKillCount;

    public void CountTheKill()
    {
        if (!PlayerController.s_instance.PlayerWeapon.BPMSystem.IsCurrentlyOnFury)
        {
            if (isOnTime)
            {
                _currentKillCount++;
                switch (_currentKillCount)
                {
                    case 2:
                        AddScore(scoreSystem.multiKill.doubleKill);
                       // mutliKillTimerUnderFive.gameObject.SetActive(true);
                       // mutliKillTimerOverFive.gameObject.SetActive(false);
                        //Debug.Log("DoubleKill");
                        break;
                    case 3:
                        AddScore(scoreSystem.multiKill.tripleKill);
                        //mutliKillTimerUnderFive.gameObject.SetActive(true);
                        //mutliKillTimerOverFive.gameObject.SetActive(false);
                        //Debug.Log("TripleKill");
                        break;
                    case 5:
                        AddScore(scoreSystem.multiKill.pentaKill);
                        //Debug.Log("PentaKill");
                        //mutliKillTimerUnderFive.gameObject.SetActive(false);
                        //mutliKillTimerOverFive.gameObject.SetActive(true);
                        break;
                    case 10:
                        AddScore(scoreSystem.multiKill.decaKill);
                        //Debug.Log("DecaKill");
                        _currentKillCount = 0;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                _currentKillCount = 0;
                _currentKillCount++;
            }
            if(_currentKillCount <= 5)
            {
                _currentTimeBetweenKillUnderFive = 0;
            }
            else
            {
                _currentTimeBetweenKillOverFive = 0;
            }
        }
        else
        {
            if (isOnTimeOnFury)
            {
                _currentKillCount++;
                switch (_currentKillCount)
                {
                    case 5:
                        AddScore(scoreSystem.overAdrenaline.furyPentaKill);
                        //mutliKillTimerUnderFive.gameObject.SetActive(false);
                        //mutliKillTimerOverFive.gameObject.SetActive(true);
                        break;
                    case 10:
                        AddScore(scoreSystem.overAdrenaline.furyDecaKill);
                        _currentKillCount = 0;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                _currentKillCount = 0;
                _currentKillCount++;
            }
            _currentTimeBetweenKillOnFury = 0;
        }

        //killCount.text = string.Format("{0}", _currentKillCount);
        
    }
    int _currentNbrOfElectricalyStunEnemy;
    public void CountElectricalyStunEnemy()
    {
        _currentNbrOfElectricalyStunEnemy++;
        if (_currentNbrOfElectricalyStunEnemy == 5)
        {
            AddScore(scoreSystem.overAdrenaline.furyStunEnnemies);
            _currentNbrOfElectricalyStunEnemy = 0;
        }
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


[Serializable]
public class KillSomething
{
    [Header("Score by killing an enemy")]
    public int killAnEnemy;
    [Header("Score by killing a suicidalBot before it initates self destruct")]
    public int beforeSelfDestructKill;
    [Header("Kill a SuicidalBot in the weakspot")]
    public int suicidalWeakSpotKill;
    [Header("Kill a Rusher only in weakspot")]
    public int rusherWeakSpotKill;
    [Header("Kill a sniper only in his chest (firezone)")]
    public int sniperChestKill;
    [Header("Kill an enemy in midair")]
    public int midAirKill;
    [Header("Kill an enemy before he even shoots")]
    public int killedBeforeItShots;
    [Header("Kill an enemy when it rushs the player")]
    public int killWhenRushing;
}
[Serializable]
public class HitSomething
{
    [Header("Hit Weak Spot")]
    public int weakSpotHits;
    [Header("Hit No Spot")]
    public int noSpotHits;
}
[Serializable]
public class OverAdrenaline
{
    public float timeBetweenEachKillOnFury;
    [Space]
    [Header("Use Fury")]
    public int onUsingFury;
    [Header("Stun 5 ennemies with overdrenaline")]
    public int furyStunEnnemies;
    [Header("Kill 5 ennemies with overdrenaline")]
    public int furyPentaKill;
    [Header("Kill 10 ennemies with overdrenaline(Max 1sec between each)")]
    public int furyDecaKill;
}
[Serializable]
public class DoAMultiKill
{
    public float timeBetweenEachKillUnderFive;
    public float timeBetweenEachKillOverFive;
    [Space]
    [Header("Make a Double Kill (1,5seconds between Each)")]
    public int doubleKill;
    [Header("Make a Triple Kill (1,5seconds between Each)")]
    public int tripleKill;
    [Header("Make a Penta Kill (1,5seconds between Each)")]
    public int pentaKill;
    [Header("Make a Deca Kill (1,5seconds between Each, 1sec after the Fifth)")]
    public int decaKill;
}
[Serializable]
public class DestroyEnvironements
{
    [Header("Destroy categorie 1")]
    public int destroyFirstCategorie;
    [Header("Destroy categorie 2")]
    public int destroySecondCategorie;
    [Header("Destroy categorie 3")]
    public int destroyThirdCategorie;
    [Header("Destroy categorie 4")]
    public int destroyFourthCategorie;
}
[Serializable]
public class EndOfLevel
{
    [Header("Dont go lower than level 2 for at least 2/3 of the level")]
    public int upperLevel2;
    [Header("Dont go lower than level 3 for at least 1/2 of the level")]
    public int upperLevel3;
    [Header("Don't die for the entire level")]
    public int unKillable;
}