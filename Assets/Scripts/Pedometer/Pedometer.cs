
using UnityEngine;
using TMPro;

public class Pedometer : MonoBehaviour
{
    public TMP_Text text;
    private float lowLimit = 0.005F;
    private float limit = 0.3F;
    private bool isHigh = false;
    private float frequencyHigh = 8.0F;
    private float currentAcceleration = 0F;
    private float frequencyLow = 0.2F;
    private float previousAcceleration;
    private const int NUMBER_OF_STEPS= 1;

    private void Start()
    {
        
        if (!PlayerPrefs.HasKey("TotalSteps"))
        {
            PlayerPrefs.SetInt("TotalSteps", 0);
        }
        if (!PlayerPrefs.HasKey("Steps"))
        {
            PlayerPrefs.SetInt("Steps", 0);
        }
        if (!PlayerPrefs.HasKey("Coins"))
        {
            PlayerPrefs.SetInt("Coins", 10000);
        }
    }
    void Update()
    {
        StepDetector();
    }

    //Se encarga de comprobar si se ha dado un paso
    public void StepDetector()
    {
        currentAcceleration = Mathf.Lerp(currentAcceleration, Input.acceleration.magnitude, Time.deltaTime * frequencyHigh);
        previousAcceleration = Mathf.Lerp(previousAcceleration, Input.acceleration.magnitude, Time.deltaTime * frequencyLow);
        float delta = currentAcceleration - previousAcceleration;
        if (!isHigh)
        {
            if (delta > limit)
            {
                isHigh = true;
                PlayerPrefs.SetInt("Steps", PlayerPrefs.GetInt("Steps") +1);
                PlayerPrefs.SetInt("TotalSteps", PlayerPrefs.GetInt("TotalSteps") + 1);
                if (PlayerPrefs.GetInt("Steps") == NUMBER_OF_STEPS)
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
            if (delta < lowLimit)
            {
                isHigh = false;
            }
        }
        previousAcceleration = currentAcceleration;
        text.text = PlayerPrefs.GetInt("Coins").ToString();

    }
}
