using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndGameScoreCount : MonoBehaviour
{

    public int countingSpeed;
    TMP_Text text;
    int fakeScore;
    int countingMutlipicator;

    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public IEnumerator StartCoutningScore()
    {
        float count = GameManager.Instance.CurrentScore;
        while (count >= 10)
        {
            count = count / 10f;
            countingMutlipicator++;
        }

        while (fakeScore < GameManager.Instance.CurrentScore)
        {
            
            yield return new WaitForSeconds(Time.fixedDeltaTime/(10000 * (1 + countingMutlipicator) * (1 + countingSpeed)));
            if(fakeScore < 10)
            {
                fakeScore+= (1 + countingMutlipicator);
            }
            else if (fakeScore >= 10 && fakeScore < 100)
            {
                fakeScore += (1 + countingMutlipicator) * 1;
            }
            else if (fakeScore >= 100 && fakeScore < 1000)
            {
                fakeScore += (1 + countingMutlipicator) * 11;
            }
            else if(fakeScore >= 1000 && fakeScore < 10000)
            {
                fakeScore += (1 + countingMutlipicator) * 111;
            }
            else
            {
                fakeScore += (1 + countingMutlipicator) * 1111;
            }
            if (fakeScore < GameManager.Instance.CurrentScore)
            {
                text.text = string.Format("{0}", fakeScore);
            }
            else
            {
                text.text = string.Format("{0}" ,GameManager.Instance.CurrentScore);
            }
        }

    }
}
