using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{

    private const int NUMBER_OF_QUESTS = 3;
    private Quest[] dailyQuests=new Quest[NUMBER_OF_QUESTS];
    private Quest[] weeklyQuests=new Quest[NUMBER_OF_QUESTS];
    private Quest[] monthlyQuests=new Quest[NUMBER_OF_QUESTS];

    [SerializeField]private GameObject[] questPanels;
    [SerializeField] private GameObject progressPanel;
    [SerializeField] private TMP_Text progressText;

    private string todaysDate;
    private const string MONDAY="Monday";

    private void Awake()
    {
        LoadFiles();
        Debug.Log("DESPUES DE LOAD");
        //MostrarMisiones();
    }

    private void LoadFiles()
    {
        if (File.Exists(Application.persistentDataPath + "/dailyQuests.txt"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/dailyQuests.txt", FileMode.Open);
            dailyQuests = (Quest[])bf.Deserialize(file);
            file.Close();
        }

        if (File.Exists(Application.persistentDataPath + "/weeklyQuests.txt"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/weeklyQuests.txt", FileMode.Open);
            weeklyQuests = (Quest[])bf.Deserialize(file);
            file.Close();
        }

        if (File.Exists(Application.persistentDataPath + "/monthlyQuests.txt"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/monthlyQuests.txt", FileMode.Open);
            monthlyQuests = (Quest[])bf.Deserialize(file);
            file.Close();
        }
    }

    private void Start()
    {
        todaysDate = DateTime.Today.ToLocalTime().ToString();
        Debug.Log("Fecha de hoy "+todaysDate);
        Debug.Log("Ultima fecha guardada "+ PlayerPrefs.GetString("lastDateSaved"));
        if (todaysDate!=PlayerPrefs.GetString("lastDateSaved"))
        {
            Debug.Log("Fecha distinta");
            CreateQuest();
            PlayerPrefs.SetString("lastDateSaved", todaysDate);
            PlayerPrefs.Save();
            Debug.Log("Ultima fecha guardada " + PlayerPrefs.GetString("lastDateSaved"));
            //MostrarMisiones();
        }
        SetQuestPanel();
    }

    private void SetQuestPanel()
    {
        for (int i = 0; i < questPanels.Length; i++)
        {
            TMP_Text questText=questPanels[i].transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text rewardText= questPanels[i].transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>();

            if (i < 3)
            {
                questText.text = dailyQuests[i].text;
                rewardText.text = dailyQuests[i].reward.ToString();

                Quest q = dailyQuests[i];
                questPanels[i].GetComponent<Button>().onClick.AddListener(delegate { OpenProgressPanel(q); });
                if (dailyQuests[i].completed)
                {
                    questPanels[i].transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                }
            }
            else if (i < 6)
            {
                questText.text = weeklyQuests[i % 3].text;
                rewardText.text = weeklyQuests[i % 3].reward.ToString();

                Quest q = weeklyQuests[i % 3];
                questPanels[i].GetComponent<Button>().onClick.AddListener(delegate { OpenProgressPanel(q); });
                if (weeklyQuests[i % 3].completed)
                {
                    questPanels[i].transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                }
            }
            else
            {
                questText.text = monthlyQuests[i % 3].text;
                rewardText.text = monthlyQuests[i % 3].reward.ToString();

                Quest q = monthlyQuests[i % 3];
                questPanels[i].GetComponent<Button>().onClick.AddListener(delegate { OpenProgressPanel(q); });
                if (monthlyQuests[i % 3].completed)
                {
                    questPanels[i].transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                }
            }
            
        }
    }

    private void MostrarMisiones()
    {
        Debug.Log("MISIONES DIARIAS");
        for (int i = 0; i < dailyQuests.Length; i++)
        {
            if (dailyQuests[i] != null)
            {
                Debug.Log(dailyQuests[i].ToString());
            }
            
        }
        Debug.Log("MISIONES SEMANALES");
        for (int i = 0; i < weeklyQuests.Length; i++)
        {
            if (weeklyQuests[i] != null)
            {
                Debug.Log(weeklyQuests[i].ToString());
            }
        }
        Debug.Log("MISIONES MENSUALES");
        for (int i = 0; i < monthlyQuests.Length; i++)
        {
            if (monthlyQuests[i] != null)
            {
                Debug.Log(monthlyQuests[i].ToString());
            }
        }
    }

    private void CreateQuest()
    {
        for (int i = 0; i < 3; i++)
        {
            if (PlayerPrefs.HasKey("lastDateSaved"))
            {
                if (i == 0)
                {
                    SetQuestValues(i);
                }

                if (i == 1 && CheckIfDifferentWeek())
                {
                    SetQuestValues(i);
                }

                if (i == 2 && CheckIfDifferentMonth())
                {
                    SetQuestValues(i);
                }
            }
            else
            {
                SetQuestValues(i);
            }
        }
        SaveOnFile("dailyQuests");
        SaveOnFile("weeklyQuests");
        SaveOnFile("monthlyQuests");
    }

    private bool CheckIfDifferentWeek()
    {

        string thisMonday = "";
        switch (DateTime.Today.DayOfWeek)
        {
            case DayOfWeek.Monday:
                thisMonday = DateTime.Today.ToLocalTime().ToString();
                break;
            case DayOfWeek.Tuesday:
                thisMonday = DateTime.Today.AddDays(-1).ToLocalTime().ToString();
                break;
            case DayOfWeek.Wednesday:
                thisMonday = DateTime.Today.AddDays(-2).ToLocalTime().ToString();
                break;
            case DayOfWeek.Thursday:
                thisMonday = DateTime.Today.AddDays(-3).ToLocalTime().ToString();
                break;
            case DayOfWeek.Friday:
                thisMonday = DateTime.Today.AddDays(-4).ToLocalTime().ToString();
                break;
            case DayOfWeek.Saturday:
                thisMonday = DateTime.Today.AddDays(-5).ToLocalTime().ToString();
                break;
            case DayOfWeek.Sunday:
                thisMonday = DateTime.Today.AddDays(-6).ToLocalTime().ToString();
                break;
        }
        Debug.Log("este lunes " + thisMonday);
        if (PlayerPrefs.HasKey("lastMondaySaved"))
        {
            if (PlayerPrefs.GetString("lastMondaySaved").Equals(thisMonday))
            {
                return false;
            }
        }
        PlayerPrefs.SetString("lastMondaySaved", thisMonday);
        return true;
    }

    private void SaveOnFile(string fileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/"+fileName+".txt");
        switch (fileName)
        {
            case "dailyQuests":
                bf.Serialize(file, dailyQuests);
                break;
            case "weeklyQuests":
                bf.Serialize(file, weeklyQuests);
                break;
            case "monthlyQuests":
                bf.Serialize(file, monthlyQuests);
                break;
        }
        file.Close();
    }

    private bool CheckIfDifferentMonth()
    {
        string todaysMonth = todaysDate.Split("/")[1];
        string lastDateSavedMonth = PlayerPrefs.GetString("lastDateSaved").Split("/")[1];

        if (todaysMonth == lastDateSavedMonth)
        {
            return false;
        }
        return true;
    }

    private void SetQuestValues(int i)
    {
        bool correctQuestType = false;
        List<int> questTypes = new List<int>();
        int num;
        for (int j = 0; j < NUMBER_OF_QUESTS; j++)
        {
            do
            {
                correctQuestType = false;
                num = new System.Random().Next(0, 5);
                if (!questTypes.Contains(num))
                {
                    questTypes.Add(num);
                    correctQuestType = true;
                }
            } while (!correctQuestType);

            Quest.QuestType type = Quest.QuestType.CARD;

            switch (num)
            {
                case 0:
                    type = Quest.QuestType.CARD;
                    break;
                case 1:
                    type = Quest.QuestType.FUSION;
                    break;
                case 2:
                    type = Quest.QuestType.PACK;
                    break;
                case 3:
                    type = Quest.QuestType.STEPS;
                    break;
                case 4:
                    type = Quest.QuestType.TRADE;
                    break;
            }

            Quest.QuestDifficulty difficulty = Quest.QuestDifficulty.EASY;

            switch (j)
            {
                case 0:
                    difficulty = Quest.QuestDifficulty.EASY;
                    break;
                case 1:
                    difficulty = Quest.QuestDifficulty.MEDIUM;
                    break;
                case 2:
                    difficulty = Quest.QuestDifficulty.HARD;
                    break;
            }

            switch (i)
            {
                case 0:
                    dailyQuests[j] = new Quest(difficulty, type, Quest.QuestTime.DAY);
                    Debug.Log("Devuelve "+j+i);
                    //questPanels[j+i].transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                    break;
                case 1:
                    weeklyQuests[j] = new Quest(difficulty, type, Quest.QuestTime.WEEK);
                    //questPanels[j + i].transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                    break;
                case 2:
                    monthlyQuests[j] = new Quest(difficulty, type, Quest.QuestTime.MONTH);
                    //questPanels[j + i].transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void UpdateQuests(Quest.QuestType type)
    {
        int numQuest = 0;
        foreach (Quest quest in dailyQuests)
        {
            if (quest.type == type)
            {
                if (!quest.completed)
                {
                    if (quest.UpdateActualQuantity())
                    {
                        Debug.Log("Quest "+numQuest+" "+quest.text);
                        questPanels[numQuest].transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + quest.reward);
                    }
                    SaveOnFile("dailyQuests");
                }
            }
            numQuest++;
        }

        foreach (Quest quest in weeklyQuests)
        {
            if (quest.type == type)
            {
                if (!quest.completed)
                {
                    if (quest.UpdateActualQuantity())
                    {
                        questPanels[numQuest].transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + quest.reward);
                    }
                }
                SaveOnFile("weeklyQuests");

            }
            numQuest++;
        }

        foreach (Quest quest in monthlyQuests)
        {
            if (quest.type == type)
            {
                if (!quest.completed)
                {
                    if (quest.UpdateActualQuantity())
                    {
                        questPanels[numQuest].transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + quest.reward);
                    }
                }
                SaveOnFile("monthlyQuests");
            }
            numQuest++;
        }
    }

    public void OpenProgressPanel(Quest quest)
    {
        progressPanel.SetActive(true);
        SetProgressText(quest);
    }

    private void SetProgressText(Quest quest)
    {
        switch (quest.type)
        {
            case Quest.QuestType.TRADE:
                progressText.text = "INTERCAMBIOS REALIZADOS:\n";
                break;
            case Quest.QuestType.CARD:
                progressText.text = "CARTAS RARAS O ULTRA RARAS OBTENIDAS:\n";
                break;
            case Quest.QuestType.FUSION:
                progressText.text = "FUSIONES REALIZADAS:\n";
                break;
            case Quest.QuestType.PACK:
                progressText.text = "SOBRES ABIERTOS:\n";
                break;
            case Quest.QuestType.STEPS:
                progressText.text = "PASOS CAMINADOS:\n";
                break;
        }
        progressText.text += quest.actualQuantity + "/" + quest.quantity;
    }
}
