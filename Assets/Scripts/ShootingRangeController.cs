using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShootingRangeController : MonoBehaviour
{
    public TMP_Text minutesAndSeconds;

    public TMP_Text scoreText;
    public static ShootingRangeController s_instance;

    int score;
    public float shootingRangeTime;
    List<GameObject> shootingRangePoutches = new List<GameObject>();
    float _currentTime;
    bool shootingRangeHasStarted;
    float minutes;
    float secondes;

    public List<GameObject> ShootingRangePoutches { get => shootingRangePoutches; set => shootingRangePoutches = value; }

    private void Awake()
    {
        shootingRangeHasStarted = true;
        SetupSingleton();
    }
    void SetupSingleton()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Debug.LogError("Two instance of PlayerController");
        }
    }

    private void Update()
    {
        if(_currentTime != 0)
        {
            _currentTime -= Time.deltaTime;
            if(_currentTime <= 0)
            {
                _currentTime = 0;
            }

            minutes = Mathf.FloorToInt(_currentTime / 60);
            secondes = Mathf.FloorToInt((_currentTime % 60));

            if (minutes < 10)
            {
                if (secondes < 10)
                {
                    minutesAndSeconds.text = string.Format("0{0} : 0{1}", minutes, secondes);
                }
                else
                {
                    minutesAndSeconds.text = string.Format("0{0} : {1}", minutes, secondes);
                }
            }
            else
            {
                if (secondes < 10)
                {
                    minutesAndSeconds.text = string.Format("{0} : 0{1}", minutes, secondes);
                }
                else
                {
                    minutesAndSeconds.text = string.Format("{0} : {1}", minutes, secondes);
                }
            }
        }
    }

    public void AddScore(int addedScore, GameObject gameObject)
    {
        score += addedScore;
        scoreText.text = string.Format("{0}", score);
        ShootingRangePoutches.Remove(gameObject);
        if (shootingRangeHasStarted)
        {
            _currentTime = shootingRangeTime;
            shootingRangeHasStarted = false;
            StartCoroutine(ShootingRangeTime());
            for (int i = 0, l = shootingRangePoutches.Count; i < l; ++i)
            {
                if (shootingRangePoutches[i].GetComponent<PoutchChara>() != null)
                {
                    shootingRangePoutches[i].GetComponent<PoutchChara>().OnInstantiateNewPoutch();
                }
            }
            score = 0;
            scoreText.text = string.Format("{0}", score);
        }
    }

    IEnumerator ShootingRangeTime()
    {
        yield return new WaitForSeconds(shootingRangeTime);
        for (int i = 0, l = shootingRangePoutches.Count; i < l; ++i)
        {
            if (shootingRangePoutches[i].GetComponent<PoutchChara>() != null)
            {
                shootingRangePoutches[i].GetComponent<PoutchChara>().StartCoroutine(shootingRangePoutches[i].GetComponent<PoutchChara>().JusteDie());
            }
        }
        yield return new WaitForSeconds(5f);
        shootingRangeHasStarted = true;
        

    }
}
