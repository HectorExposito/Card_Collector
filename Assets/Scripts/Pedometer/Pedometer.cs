using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Pedometer : MonoBehaviour
{
    public TMP_Text text;
    private void Start()
    {
        if (PlayerPrefs.HasKey("TotalSteps"))
        {
            PlayerPrefs.DeleteKey("TotalSteps");
        }
        if (!PlayerPrefs.HasKey("Steps"))
        {
            PlayerPrefs.SetInt("Steps", 0);
        }
        if (!PlayerPrefs.HasKey("Coins"))
        {
            PlayerPrefs.SetInt("Coins", 10000);
        }
        else
        {
            PlayerPrefs.SetInt("Coins",10000);
        }
    }
    void Update()
    {
        stepDetector();
    }

    private float loLim = 0.005F;
    private float Lim = 0.3F;
    private int steps = 0;
    private bool stateH = false;
    private float fHigh = 8.0F;
    private float curAcc = 0F;
    private float fLow = 0.2F;
    private float avgAcc;

    //Se encarga de comprobar si se ha dado un paso
    public void stepDetector()
    {
        curAcc = Mathf.Lerp(curAcc, Input.acceleration.magnitude, Time.deltaTime * fHigh);
        avgAcc = Mathf.Lerp(avgAcc, Input.acceleration.magnitude, Time.deltaTime * fLow);
        float delta = curAcc - avgAcc;
        if (!stateH)
        {
            if (delta > Lim)
            {
                stateH = true;
                PlayerPrefs.SetInt("Steps", PlayerPrefs.GetInt("Steps") +1);
                if (PlayerPrefs.GetInt("Steps") ==10)
                {
                    PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 1);
                    PlayerPrefs.SetInt("Steps", 0);
                }
                FindObjectOfType<QuestManager>().UpdateQuests(Quest.QuestType.STEPS);
                PlayerPrefs.Save();
                
            }
        }
        else
        {
            if (delta < loLim)
            {
                stateH = false;
            }
        }
        avgAcc = curAcc;
        text.text = PlayerPrefs.GetInt("Coins").ToString();

    }
}
