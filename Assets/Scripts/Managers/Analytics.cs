using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Analytics;

public static class Analytics
{

    async static public void Start()
    {
        Application.targetFrameRate = 60;
        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log(UnityServices.State);
            await AnalyticsService.Instance.SetAnalyticsEnabled(true);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    static public void SendAnalytics(string eventName, Dictionary<string,object> paramss)
    {
        try
        {
            AnalyticsService.Instance.CustomData(eventName, paramss);            
        }
        catch (ServicesInitializationException e)
        {
            Debug.Log("Analytics isn't initialized");
        }
    }

    static public void PlayerDied()
    {
        SendAnalytics("PlayerDied", new() { { "userLevel",WorldManager.Instance.CurrentLevel} });
    }
    static public void PlayerFinishedRun()
    {
        SendAnalytics("PlayerFinishedRun", new() { { "TimeInSeconds", WorldManager.Instance._timeWithoutPauses } });
    }
    static public void PlayerPickedUpgrade(List<LevelUpgrades> upgrades)
    {
        SendAnalytics("PlayerPickedUpgrade", new() { { "PickedUpgrade", upgrades[0].UpgradeName }, { "UnpickedUpgradeNumber1", upgrades[1].UpgradeName }, { "UnpickedUpgradeNumber2", upgrades[2].UpgradeName } });
    }
}
