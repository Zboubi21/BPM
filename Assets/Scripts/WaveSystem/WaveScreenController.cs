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
            }
        }
        
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
            ChangeInformationDisplayed(channel);
        }
    }


    #region Set Screen Var
    void ChangeInformationDisplayed(ScreenChannel channel)
    {
        if (allWaveScreen[(int)channel] != null)
        {
            ChangeInfoOnScreen(allWaveScreen[(int)channel]);
            //SwitchChannel((int)channel);
        }
    }

    void ChangeInfoOnScreen(WaveScreenReference screenRef) //Set the variable to be change whithin the appropriate function
    {
        if(waveController != null)
        {
            if (screenRef.backGround != null)
            {
                switch (_screenChannel)
                {
                    case ScreenChannel.WaveCountChannel:
                        StartCoroutine(UpdateWaveBackground(screenRef));
                        break;
                    case ScreenChannel.EnemyCountChannel:
                        StartCoroutine(UpdateEnemyBackground(screenRef));
                        break;
                    case ScreenChannel.ScoreCountChannel:
                        StartCoroutine(UpdateScoreBackground(screenRef));
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
                        StartCoroutine(UpdateWave(screenRef, waveController.NbrOfWave, 10));
                        break;
                    case ScreenChannel.EnemyCountChannel:
                        StartCoroutine(UpdateEnemy(screenRef, waveController.NbrOfEnemy));
                        break;
                    case ScreenChannel.ScoreCountChannel:
                        StartCoroutine(UpdateScore(screenRef, 99599));
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
                        StartCoroutine(UpdateWaveStaticInfo(screenRef));
                        break;
                    case ScreenChannel.EnemyCountChannel:
                        StartCoroutine(UpdateEnemyStaticInfo(screenRef));
                        break;
                    case ScreenChannel.ScoreCountChannel:
                        StartCoroutine(UpdateScoreStaticInfo(screenRef));
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
                        StartCoroutine(UpdateWaveDecorativeInfos(screenRef));
                        break;
                    case ScreenChannel.EnemyCountChannel:
                        StartCoroutine(UpdateEnemyDecorativeInfos(screenRef));
                        break;
                    case ScreenChannel.ScoreCountChannel:
                        StartCoroutine(UpdateScoreDecorativeInfos(screenRef));
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
    IEnumerator UpdateWaveBackground(WaveScreenReference screenRef)
    {
        yield return null;

    }
    IEnumerator UpdateWave(WaveScreenReference screenRef, int current, int max)
    {
        yield return null;
        screenRef.changingTexts[0].text = string.Format("{0}", current+1); // pour éviter d'avoir une "wave 0" on met current +1
        screenRef.changingTexts[1].text = string.Format("{0}", max);
    }
    IEnumerator UpdateWaveStaticInfo(WaveScreenReference screenRef)
    {
        yield return null;

    }
    IEnumerator UpdateWaveDecorativeInfos(WaveScreenReference screenRef)
    {
        yield return null;

    }

    #endregion

    #region UpdateEnemyScreen
    IEnumerator UpdateEnemyBackground(WaveScreenReference screenRef)
    {
        yield return null;
    }
    IEnumerator UpdateEnemy(WaveScreenReference screenRef, int current)
    {
        screenRef.animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.5f);
        screenRef.animator.SetTrigger("FadeIn");
        screenRef.changingTexts[0].text = string.Format("x{0}", current - waveController.NbrOfDeadEnemy);
    }
    IEnumerator UpdateEnemyStaticInfo(WaveScreenReference screenRef)
    {
        yield return null;

    }
    IEnumerator UpdateEnemyDecorativeInfos(WaveScreenReference screenRef)
    {
        yield return null;

    }
    #endregion

    #region UpdateScoreScreen
    IEnumerator UpdateScoreBackground(WaveScreenReference screenRef)
    {
        yield return null;

    }
    IEnumerator UpdateScore(WaveScreenReference screenRef, int current)
    {
        yield return null;
        screenRef.changingTexts[0].text = string.Format("{0}", current);
    }
    IEnumerator UpdateScoreStaticInfo(WaveScreenReference screenRef)
    {
        yield return null;

    }
    IEnumerator UpdateScoreDecorativeInfos(WaveScreenReference screenRef)
    {
        yield return null;

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
                ChangeInformationDisplayed((ScreenChannel)i);
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
