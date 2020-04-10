using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerDelayScene : MonoBehaviour
{

    [SerializeField] bool m_startWithSceneLoader = true;
    [SerializeField] AudioMixer m_mainMixer;
    [SerializeField] ChangeImageValues m_startSceneScreen;
    [SerializeField] float m_waitTimeToActivatePlayer = 1;

    void Awake()
    {
        if (m_startWithSceneLoader)
        {
            m_startSceneScreen.OverrideStartType(ChangeValues.StartType.StartWithFromValue);
            // PlayerController.
            // WeaponPlayerBehaviour.
            // BPMSystem.
        }
        else
        {
            m_startSceneScreen.OverrideStartType(ChangeValues.StartType.StartWithToValue);
        }
    }
    void Start()
    {
        if (m_startWithSceneLoader)
        {
            SetMixerVolume(-80);
            m_startSceneScreen.SwitchValue();
            StartCoroutine(WaitToActivatePlayer());
        }
        else
        {
            SetMixerVolume(0);
        }
    }

    IEnumerator WaitToActivatePlayer()
    {
        yield return new WaitForSeconds(m_waitTimeToActivatePlayer);
        SetMixerVolume(0);
    }

    void SetMixerVolume(float volume)
    {
        m_mainMixer.SetFloat("MainMixerVolume", volume);
    }

}
