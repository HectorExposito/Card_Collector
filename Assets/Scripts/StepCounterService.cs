using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StepCounterService : MonoBehaviour
{
    public TMP_Text tecto;
    private AndroidJavaObject m_Service;
    void Start()
    {
        //Replace with your full package name
        startService("com.example.cardcollectorservice.StatusCheckStarter");
    }

    void startService(string packageName)
    {
        AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass customClass = new AndroidJavaClass(packageName);
        if (customClass != null)
        {
            tecto.text = ":D";
        }
        else
        {
            tecto.text = ":(((((((((";
        }
        customClass.CallStatic("StartCheckerService", unityActivity);
    }
}
