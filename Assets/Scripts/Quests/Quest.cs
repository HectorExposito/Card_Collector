using System;

[Serializable]
public class Quest
{
    public string text;
    private QuestDifficulty difficulty;
    public QuestType type;
    private QuestTime time;
    int quantity;
    public int reward;
    int actualQuantity;
    public bool completed;
    

    public Quest(QuestDifficulty difficulty,QuestType type,QuestTime time)
    {
        this.difficulty = difficulty;
        this.type = type;
        this.time = time;

        actualQuantity = 0;
        completed = false;
        CreateQuest();
    }

    public String ToString()
    {
        return text + " " + reward;
    }

    private void CreateQuest()
    {
        switch (type)
        {
            case QuestType.TRADE:
                CreateTradeQuest();
                break;
            case QuestType.PACK:
                CreatePackQuest();
                break;
            case QuestType.CARD:
                CreateCardQuest();
                break;
            case QuestType.STEPS:
                CreateStepsQuest();
                break;
            case QuestType.FUSION:
                CreateFusionQuest();
                break;
        }

        SetQuantityAndRewardBasedOnTime();
        SetText();
    }

    public bool UpdateActualQuantity()
    {
        actualQuantity++;
        if(actualQuantity>=quantity && completed == false)
        {
            completed = true;
            return true;
        }
        return false;
    }

    private void SetText()
    {
        switch (type)
        {
            case QuestType.TRADE:
                text = "Realiza " + quantity + " intercambios.";
                break;
            case QuestType.PACK:
                text = "Abre " + quantity + " sobres.";
                break;
            case QuestType.CARD:
                text = "Consigue " + quantity + " cartas raras o ultra raras en sobres.";
                break;
            case QuestType.STEPS:
                text = "Camina " + quantity + " pasos.";
                break;
            case QuestType.FUSION:
                text = "Realiza " + quantity + " fusiones.";
                break;
        }
    }

    private void SetQuantityAndRewardBasedOnTime()
    {
        switch (time)
        {
            case QuestTime.DAY:
                break;
            case QuestTime.WEEK:
                quantity = quantity * 7;
                reward = reward * 7;
                break;
            case QuestTime.MONTH:
                quantity = quantity * 30;
                reward = reward * 30;
                break;
        }
    }

    private void CreateFusionQuest()
    {
        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                quantity = 10;
                reward = 500;
                break;
            case QuestDifficulty.MEDIUM:
                quantity = 20;
                reward = 1000;
                break;
            case QuestDifficulty.HARD:
                quantity = 30;
                reward = 2000;
                break;
        }
    }

    private void CreateStepsQuest()
    {
        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                quantity = 1000;
                reward = 200;
                break;
            case QuestDifficulty.MEDIUM:
                quantity = 4000;
                reward = 600;
                break;
            case QuestDifficulty.HARD:
                quantity = 6000;
                reward = 1000;
                break;
        }
    }

    private void CreateCardQuest()
    {
        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                quantity = 1;
                reward = 500;
                break;
            case QuestDifficulty.MEDIUM:
                quantity = 3;
                reward = 2000;
                break;
            case QuestDifficulty.HARD:
                quantity = 5;
                reward = 3000;
                break;
        }
    }

    private void CreatePackQuest()
    {
        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                quantity = 5;
                reward = 1100;
                break;
            case QuestDifficulty.MEDIUM:
                quantity = 10;
                reward = 2400;
                break;
            case QuestDifficulty.HARD:
                quantity = 25;
                reward = 6000;
                break;
        }
    }

    private void CreateTradeQuest()
    {
        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                quantity = 1;
                reward = 200;
                break;
            case QuestDifficulty.MEDIUM:
                quantity = 5;
                reward = 800;
                break;
            case QuestDifficulty.HARD:
                quantity = 10;
                reward = 1600;
                break;
        }
    }


    public enum QuestType
    {
        TRADE,PACK,CARD,STEPS,FUSION
    }

    public enum QuestDifficulty
    {
        EASY,MEDIUM,HARD
    }

    public enum QuestTime
    {
        DAY,WEEK,MONTH
    }
}
