using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEditor;
using UnityEngine;
using Unity.Services.Economy;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using System.Threading.Tasks;
using Unity.Services.Economy.Model;
using System;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject LastUpdateWindow;
    [SerializeField] TMPro.TMP_Text CoinCounter;

    public async void Awake()
    {
        VersionCheck();
        //LastUpdateWindow.SetActive(true);
        GetBalancesResult res = null;
        try
        {
            res = await GetEconomyBalances();
            foreach (var balance in res.Balances)
            {
                if (balance.CurrencyId == "COINS")
                {
                    CoinCounter.text = balance.Balance.ToString();
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Problem getting Economy currency balances:");
            Debug.LogException(e);
        }
    }
    void VersionCheck()
    {
        if (PlayerPrefs.HasKey("LastVersion"))
        {
            if (PlayerPrefs.GetString("LastVersion") == Application.version) return;
        }
        PlayerPrefs.SetString("LastVersion", Application.version);
    }
    public void PlayGame()
    {
        //SceneManager.LoadScene(1);
    }
    public async void QuitGame()
    {
        Application.Quit();
    }
    static public async void IncrementCoins()
    {
        int coins = WorldManager.Instance.RewardManager.CoinsInRun;
        PlayerBalance balance = null;
        try
        {
            balance = await IncrementPlayerBalance(coins);
            
        }
        catch (Exception e)
        {
            Debug.Log("Problem incrementing coins:");
            Debug.LogException(e);
        }
    }
    static Task<PlayerBalance> IncrementPlayerBalance(int coins)
    {
        return EconomyService.Instance.PlayerBalances.IncrementBalanceAsync("COINS", coins);
    }
    static Task<GetBalancesResult> GetEconomyBalances()
    {
        return EconomyService.Instance.PlayerBalances.GetBalancesAsync();
    }

}
