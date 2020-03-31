using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ScreenTypes;
public class WaveScreenController :  MonoBehaviour
{
    public ScreenChannel _screenChannel;


    [Space]
    WaveScreenReference[] allWaveScreen;
    WaveController waveController;
    [Space]
    public AnimationCurve scoreCurve;
    public float timeOfScoreAnimation;
    float currentFontSize;
    float maxFontSize = 0.35f;

    private void Start()
    {
        allWaveScreen = GetComponentsInChildren<WaveScreenReference>();
        //ChangeInformationDisplayed(_screenChannel);

        if(allWaveScreen.Length > 0)
        {
            for (int i = 0, l = allWaveScreen.Length; i < l; ++i)
            {
                if ((int)_screenChannel == i)
                {
                    allWaveScreen[i].gameObject.SetActive(true);
                }
                else
                {
                    allWaveScreen[i].gameObject.SetActive(false);
                }
                currentFontSize = allWaveScreen[(int)ScreenChannel.ScoreCountChannel].changingTexts[0].fontSize;
            }
        }
    }
    int scoreGain;
    int currentScore = 200;
    private void Update()
    {
#if UNITY_EDITOR



        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            scoreGain = 100;
            currentScore += scoreGain;
            StartCoroutine(UpdateScore(allWaveScreen[3], currentScore, scoreGain, true));


        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            scoreGain = 10;
            currentScore += scoreGain;
            StartCoroutine(UpdateScore(allWaveScreen[3], currentScore, scoreGain, true));

        }

#endif
    }

    public void SetWaveController(WaveController control)
    {
        if (waveController != control || waveController == null)
        {
            waveController = control;
        }
    }

    public void OnChangeDisplayInfo(ScreenChannel channel)
    {
        if(channel == _screenChannel)
        {
            ChangeInformationDisplayed(channel, true);
        }
    }


#region Set Screen Var
    void ChangeInformationDisplayed(ScreenChannel channel, bool hasToAnimate)
    {
        if (allWaveScreen[(int)channel] != null)
        {
            ChangeInfoOnScreen(allWaveScreen[(int)channel], hasToAnimate);
            //SwitchChannel((int)channel);
        }
    }
    

    void ChangeInfoOnScreen(WaveScreenReference screenRef, bool hasToAnimate) //Set the variable to be change whithin the appropriate function
    {
        if(waveController != null)
        {
            if (screenRef.backGround != null)
            {
                switch (_screenChannel)
                {
                    case ScreenChannel.WaveCountChannel:
                        StartCoroutine(UpdateWaveBackground(screenRef, hasToAnimate));
                        break;
                    case ScreenChannel.EnemyCountChannel:
                        StartCoroutine(UpdateEnemyBackground(screenRef, hasToAnimate));
                        break;
                    case ScreenChannel.ScoreCountChannel:
                        StartCoroutine(UpdateScoreBackground(screenRef, hasToAnimate));
                        break;
                    default:
                        break;
                }
            }

            if (screenRef.changingTexts.Length > 0)
            {
                switch (_screenChannel)
                {
                    case ScreenChannel.WaveCountChannel:
                        StartCoroutine(UpdateWave(screenRef, waveController.NbrOfWave, hasToAnimate, waveController.maxWave));
                        break;
                    case ScreenChannel.EnemyCountChannel:
                        StartCoroutine(UpdateEnemy(screenRef, waveController.NbrOfEnemy, hasToAnimate));
                        break;
                    case ScreenChannel.ScoreCountChannel:
                        StartCoroutine(UpdateScore(screenRef, 99599, scoreGain, hasToAnimate));
                        break;
                    default:
                        break;
                }
            }

            if (screenRef.staticTexts.Length > 0)
            {
                switch (_screenChannel)
                {
                    case ScreenChannel.WaveCountChannel:
                        StartCoroutine(UpdateWaveStaticInfo(screenRef, hasToAnimate));
                        break;
                    case ScreenChannel.EnemyCountChannel:
                        StartCoroutine(UpdateEnemyStaticInfo(screenRef, hasToAnimate));
                        break;
                    case ScreenChannel.ScoreCountChannel:
                        StartCoroutine(UpdateScoreStaticInfo(screenRef, hasToAnimate));
                        break;
                    default:
                        break;
                }
            }

            if (screenRef.decorativeImages.Length > 0)
            {
                switch (_screenChannel)
                {
                    case ScreenChannel.WaveCountChannel:
                        StartCoroutine(UpdateWaveDecorativeInfos(screenRef, hasToAnimate));
                        break;
                    case ScreenChannel.EnemyCountChannel:
                        StartCoroutine(UpdateEnemyDecorativeInfos(screenRef, hasToAnimate));
                        break;
                    case ScreenChannel.ScoreCountChannel:
                        StartCoroutine(UpdateScoreDecorativeInfos(screenRef, hasToAnimate));
                        break;
                    default:
                        break;
                }
            }
        }
    }

#endregion


#region Update Screen Var

#region UpdateWaveScreen
    IEnumerator UpdateWaveBackground(WaveScreenReference screenRef, bool hasToAnimate)
    {
        yield return null;

    }
    IEnumerator UpdateWave(WaveScreenReference screenRef, int current, bool hasToAnimate, int max)
    {
        if (hasToAnimate)
        {
            screenRef.changingTexts[2].text = string.Format("{0}", max);
            screenRef.changingTexts[1].text = string.Format("{0}", current + 1);
            screenRef.animator.SetTrigger("FadeIn");
            yield return new WaitForSeconds(0.5f);
            screenRef.changingTexts[0].text = string.Format("{0}", current+1); // pour éviter d'avoir une "wave 0" on met current +1
            screenRef.animator.SetTrigger("FadeOut");
        }
        else
        {
            screenRef.changingTexts[2].text = string.Format("{0}", max);
            screenRef.changingTexts[1].text = string.Format("{0}", current + 1);
            screenRef.changingTexts[0].text = string.Format("{0}", current + 1); // pour éviter d'avoir une "wave 0" on met current +1
        }
        yield return null;
    }
    IEnumerator UpdateWaveStaticInfo(WaveScreenReference screenRef, bool hasToAnimate)
    {
        yield return null;

    }
    IEnumerator UpdateWaveDecorativeInfos(WaveScreenReference screenRef, bool hasToAnimate)
    {
        yield return null;

    }

#endregion

#region UpdateEnemyScreen
    IEnumerator UpdateEnemyBackground(WaveScreenReference screenRef, bool hasToAnimate)
    {
        yield return null;
    }
    IEnumerator UpdateEnemy(WaveScreenReference screenRef, int current, bool hasToAnimate)
    {
        if (hasToAnimate)
        {
            screenRef.animator.SetTrigger("FadeOut");
            yield return new WaitForSeconds(0.5f);
            screenRef.animator.SetTrigger("FadeIn");
            screenRef.changingTexts[0].text = string.Format("x{0}", current - waveController.NbrOfDeadEnemy);
        }
        else
        {
            screenRef.changingTexts[0].text = string.Format("x{0}", current - waveController.NbrOfDeadEnemy);
        }
        yield return null;
    }
    IEnumerator UpdateEnemyStaticInfo(WaveScreenReference screenRef, bool hasToAnimate)
    {
        yield return null;

    }
    IEnumerator UpdateEnemyDecorativeInfos(WaveScreenReference screenRef, bool hasToAnimate)
    {
        yield return null;

    }
#endregion

#region UpdateScoreScreen
    IEnumerator UpdateScoreBackground(WaveScreenReference screenRef, bool hasToAnimate)
    {
        yield return null;

    }
    IEnumerator UpdateScore(WaveScreenReference screenRef, int current, int value, bool hasToAnimate)
    {
        if (hasToAnimate)
        {
            screenRef.changingTexts[0].text = string.Format("{0}", current);

            float finalValue = Mathf.Lerp(currentFontSize, maxFontSize, Mathf.InverseLerp(0, 100f, scoreGain));
            yield return StartCoroutine(ScoreEvaluateCurve(scoreCurve, screenRef, finalValue));
        }
        else
        {
            screenRef.changingTexts[0].text = string.Format("{0}", current);
        }
        yield return null;
    }
    IEnumerator UpdateScoreStaticInfo(WaveScreenReference screenRef, bool hasToAnimate)
    {
        yield return null;

    }
    IEnumerator UpdateScoreDecorativeInfos(WaveScreenReference screenRef, bool hasToAnimate)
    {
        yield return null;

    }

    IEnumerator ScoreEvaluateCurve(AnimationCurve curve, WaveScreenReference screenRef, float value)
    {
        float _currentTimeOfAnimation = 0;
        while (_currentTimeOfAnimation / timeOfScoreAnimation <= 1)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            _currentTimeOfAnimation += Time.deltaTime;
            float size = curve.Evaluate(_currentTimeOfAnimation / timeOfScoreAnimation);
            screenRef.changingTexts[0].fontSize = Mathf.Lerp(currentFontSize, value, size);
            yield return null;
        }
    }
#endregion

#endregion

#region SwitchChanel
    public void SwitchChannel(int channel)
    {
        for (int i = 0, l = allWaveScreen.Length; i < l; ++i)
        {
            if(channel == i)
            {
                allWaveScreen[i].gameObject.SetActive(true);
                _screenChannel = (ScreenChannel)i;
                ChangeInformationDisplayed((ScreenChannel)i, false);
            }
            else
            {
                allWaveScreen[i].gameObject.SetActive(false);
            }
        }
    }
    public void SwitchChannel()
    {
        if(waveController != null)
        {
            for (int i = 0, l = allWaveScreen.Length; i < l; ++i)
            {
                if (i == (int)ScreenChannel.CocoChannel)
                {
                    allWaveScreen[i].gameObject.SetActive(true);
                    _screenChannel = ScreenChannel.CocoChannel;
                    waveController.AddCocoScreen();
                }
                else
                {
                    allWaveScreen[i].gameObject.SetActive(false);
                }
            }
        }
    }
#endregion
}
