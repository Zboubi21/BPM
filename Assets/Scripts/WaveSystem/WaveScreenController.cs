using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveScreenController : MonoBehaviour
{
    public Canvas m_allScreen;
    public Image m_waveHasASlider;
    public Image m_robotImage;
    public GridLayoutGroup m_waveHasACanvasGroup;
    public Text m_displayNbrOfWave;
    public Text m_displayNbrOFEnemy;


    List<Image> allEnemyImages = new List<Image>();
    List<Image> allredCrossedEnemyImages = new List<Image>();

    public void ChangeDisplayedInfo(int nbrOfEnemyOnWave, int nbrOfEnemyAlive)  // Call this one every time an enemy dies
    {
        m_displayNbrOFEnemy.text = string.Format("{0}/{1}", nbrOfEnemyOnWave - nbrOfEnemyAlive, nbrOfEnemyOnWave);

        #region only if gridLayout c'est de la merde
        if (m_waveHasASlider != null)   // ça c'est juste que je le sent pas le GridLayoutGroup                                                   
        {
            m_waveHasASlider.fillAmount = Mathf.InverseLerp(0, nbrOfEnemyOnWave, nbrOfEnemyAlive);
        }
        #endregion

        if (m_waveHasACanvasGroup != null)
        {
            Image go = Instantiate(m_robotImage, m_waveHasACanvasGroup.transform.position, Quaternion.identity, m_waveHasACanvasGroup.transform);
            allredCrossedEnemyImages.Add(go);
        }
    }

    public void ChangeDisplayedInfo(int nbrOfEnemyOnWave, int nbrOfEnemyAlive, int nbrOfTheCurrentWave, int totalOfWave) // Call this one at each end of a wave
    {
        m_displayNbrOfWave.text = string.Format("{0}/{1}", nbrOfTheCurrentWave, totalOfWave);

        m_displayNbrOFEnemy.text = string.Format("{0}/{1}", nbrOfEnemyOnWave, nbrOfEnemyOnWave);

        #region only if gridLayout c'est de la merde
        if (m_waveHasASlider != null)   // ça c'est juste que je le sent pas le GridLayoutGroup                                                   
        {
            m_waveHasASlider.fillAmount = Mathf.InverseLerp(0, nbrOfEnemyOnWave, nbrOfEnemyAlive);
        }
        #endregion

        if (m_waveHasACanvasGroup != null)
        {
            for (int i = 0; i < allredCrossedEnemyImages.Count; i++)
            {
                Destroy(allredCrossedEnemyImages[i].gameObject);
            }
            for (int i = 0; i < allEnemyImages.Count; i++)
            {
                Destroy(allEnemyImages[i].gameObject);
            }
            allEnemyImages.Clear();
            allredCrossedEnemyImages.Clear();
            for (int i = 0; i < nbrOfEnemyOnWave; i++)
            {
                Image go = Instantiate(m_robotImage, m_waveHasACanvasGroup.transform.position, Quaternion.identity, m_waveHasACanvasGroup.transform);
                allEnemyImages.Add(go);
            }
            
        }
    }
}
