using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveScreenController :  MonoBehaviour
{
    #region Enumerateur
    public enum ScreenChannel
    {
        WaveCountChannel,
        EnemyCountChannel,
        ScoreCountChannel,
        CocoChannel
    }
    #endregion

    public ScreenChannel _screenChannel;
    [Space]
    public GameObject[] allChannel;
    WaveController waveController;

    private void Start()
    {
        ChangeInformationDisplayed();
    }

    public void OnChangeDisplayInfo(WaveController control)
    {
        if(waveController != control)
        {
            waveController = control;
        }
        ChangeInformationDisplayed();
    }


    #region Set Screen Var
    void ChangeInformationDisplayed()
    {
        switch (_screenChannel)         //Get the right waveScreenReference depending on the type of channel this should display
        {
            case ScreenChannel.EnemyCountChannel:

                if (allChannel[0] != null)
                {
                    ChangeInfoOnScreen(allChannel[0].GetComponent<WaveScreenReference>());
                    SwitchChannel(1);
                }

                break;
            case ScreenChannel.WaveCountChannel:

                if (allChannel[1] != null)
                {
                    ChangeInfoOnScreen(allChannel[1].GetComponent<WaveScreenReference>());
                    SwitchChannel(2);
                }

                break;
            case ScreenChannel.ScoreCountChannel:

                if (allChannel[2] != null)
                {
                    ChangeInfoOnScreen(allChannel[2].GetComponent<WaveScreenReference>());
                    SwitchChannel(3);
                }

                break;
            case ScreenChannel.CocoChannel:

                if(allChannel[3] != null)
                {
                    ChangeInfoOnScreen(allChannel[3].GetComponent<WaveScreenReference>());
                    SwitchChannel(4);
                }
                break;
        }
    }

    void ChangeInfoOnScreen(WaveScreenReference screenRef) //Set the variable to be change whithin the appropriate function
    {
        if(waveController != null)
        {
            if (screenRef.backGround != null)
                UpdateBackground(screenRef);

            if (screenRef.changingInfo != null)
            {
                if(_screenChannel == ScreenChannel.EnemyCountChannel)
                {
                    UpdateChangingInfo(screenRef, waveController.NbrOfEnemy - waveController.NbrOfDeadEnemy);
                }
                else if(_screenChannel == ScreenChannel.ScoreCountChannel)
                {
                    UpdateChangingInfo(screenRef, 99599f);
                }
                else if (_screenChannel == ScreenChannel.WaveCountChannel)
                {
                    UpdateChangingInfo(screenRef, waveController.NbrOfWave, 10);
                }
            }

            if (screenRef.staticInfos.Length > 0)
                UpdateStaticInfos(screenRef);

            if (screenRef.decorativeImages.Length > 0)
                UpdateDecorativeInfos(screenRef);
        }
    }
    #endregion


    #region Update Screen Var
    void UpdateBackground(WaveScreenReference screenRef)
    {
        
    }

    void UpdateChangingInfo(WaveScreenReference screenRef, int current, int max)
    {
        screenRef.changingInfo.text = string.Format("{0}/{1}", current, max);
    }
    void UpdateChangingInfo(WaveScreenReference screenRef, int current)
    {
        screenRef.changingInfo.text = string.Format("{0}x", current);
    }
    void UpdateChangingInfo(WaveScreenReference screenRef, float current)
    {
        screenRef.changingInfo.text = string.Format("{0}", current);
    }

    void UpdateStaticInfos(WaveScreenReference screenRef)
    {

    }

    void UpdateDecorativeInfos(WaveScreenReference screenRef)
    {

    }
    #endregion

    #region Easter Egg
    public void SwitchChannel(int channel)
    {
        for (int i = 0, l = allChannel.Length; i < l; ++i)
        {
            if(channel -1 == i)
            {
                allChannel[i].SetActive(true);
            }
            else
            {
                allChannel[i].SetActive(false);
            }
        }
    }
    public void SwitchChannel()
    {
        if(waveController != null)
        {
            for (int i = 0, l = allChannel.Length; i < l; ++i)
            {
                if (i == 3)
                {
                    allChannel[i].SetActive(true);
                    _screenChannel = ScreenChannel.CocoChannel;
                    waveController.AddCocoScreen();
                }
                else
                {
                    allChannel[i].SetActive(false);
                }
            }
        }
    }
    #endregion
}