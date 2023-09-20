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
        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log(UnityServices.State);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    static public void SendAnalytics()
    {
        try
        {
            Debug.Log(UnityServices.State);
            var lol = UnityEngine.Analytics.Analytics.CustomEvent("Game Started",eventData: new Dictionary<string, object>(1) { { "try Version", 1 } });
            Debug.Log(lol);

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
