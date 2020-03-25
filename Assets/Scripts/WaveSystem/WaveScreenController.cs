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
        allWaveScreen = GetComponentInChildren<Canvas>().GetComponentsInChildren<WaveScreenReference>();
        ChangeInformationDisplayed(_screenChannel);

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
                        UpdateWaveBackground(screenRef);
                        break;
                    case ScreenChannel.EnemyCountChannel:
                        UpdateEnemyBackground(screenRef);
                        break;
                    case ScreenChannel.ScoreCountChannel:
                        UpdateScoreBackground(screenRef);
                        break;
                    default:
                        break;
                }
            }

            if (screenRef.changingInfo != null)
            {
                switch (_screenChannel)
                {
                    case ScreenChannel.WaveCountChannel:
                            UpdateWave(screenRef, waveController.NbrOfWave, 10);
                        break;
                    case ScreenChannel.EnemyCountChannel:
                            UpdateEnemy(screenRef, waveController.NbrOfEnemy - waveController.NbrOfDeadEnemy);
                        break;
                    case ScreenChannel.ScoreCountChannel:
                            UpdateScore(screenRef, 99599);
                        break;
                    default:
                        break;
                }
            }

            if (screenRef.staticInfos.Length > 0)
            {
                switch (_screenChannel)
                {
                    case ScreenChannel.WaveCountChannel:
                        UpdateWaveStaticInfo(screenRef);
                        break;
                    case ScreenChannel.EnemyCountChannel:
                        UpdateEnemyStaticInfo(screenRef);
                        break;
                    case ScreenChannel.ScoreCountChannel:
                        UpdateScoreStaticInfo(screenRef);
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
                        UpdateWaveDecorativeInfos(screenRef);
                        break;
                    case ScreenChannel.EnemyCountChannel:
                        UpdateEnemyDecorativeInfos(screenRef);
                        break;
                    case ScreenChannel.ScoreCountChannel:
                        UpdateScoreDecorativeInfos(screenRef);
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
    void UpdateWaveBackground(WaveScreenReference screenRef)
    {
        
    }
    void UpdateWave(WaveScreenReference screenRef, int current, int max)
    {
        screenRef.changingInfo.text = string.Format("{0}/{1}", current+1, max); // pour éviter d'avoir une "wave 0" on met current +1
    }
    void UpdateWaveStaticInfo(WaveScreenReference screenRef)
    {

    }
    void UpdateWaveDecorativeInfos(WaveScreenReference screenRef)
    {

    }

    #endregion

    #region UpdateEnemyScreen
    void UpdateEnemyBackground(WaveScreenReference screenRef)
    {
        
    }
    void UpdateEnemy(WaveScreenReference screenRef, int current)
    {
        screenRef.changingInfo.text = string.Format("{0}x", current);
    }
    void UpdateEnemyStaticInfo(WaveScreenReference screenRef)
    {

    }
    void UpdateEnemyDecorativeInfos(WaveScreenReference screenRef)
    {

    }
    #endregion

    #region UpdateScoreScreen
    void UpdateScoreBackground(WaveScreenReference screenRef)
    {
        
    }
    void UpdateScore(WaveScreenReference screenRef, int current)
    {
        screenRef.changingInfo.text = string.Format("{0}", current);
    }
    void UpdateScoreStaticInfo(WaveScreenReference screenRef)
    {

    }
    void UpdateScoreDecorativeInfos(WaveScreenReference screenRef)
    {

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
