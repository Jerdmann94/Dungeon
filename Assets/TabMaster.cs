using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TabMaster : MonoBehaviour
{
    public GameObject playTab;
    public GameObject stashTab;
    public GameObject tradeTab;
    public GameObject lobbyTab;
    public GameObject loadingTab;
    public GameObject stashUIBox;

    private void Start()
    {
        Debug.Log("Start");
    }

    private void OnEnable()
    {
        Debug.Log("Enable");
    }

    private void Awake()
    {
        Debug.Log("Awake");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
    {
        Debug.Log("Pressing play from inside on scene load");
        Debug.Log(arg0.name);
        if (arg0.name == "mainMenu")
        {
            var tb = GameObject.FindWithTag("TabMaster").GetComponent<TabMaster>();
            tb.log();
            tb.PressedPlay();
        }
        
    }
    public void log()
    {
        Debug.Log(playTab);
        Debug.Log(stashTab);
        Debug.Log(tradeTab);
        Debug.Log(lobbyTab);
        Debug.Log(loadingTab);
    }
    public void PressedPlay()
    {
        playTab.SetActive(true);
        stashTab.SetActive(false);
        tradeTab.SetActive(false);
        lobbyTab.SetActive(false);
        loadingTab.SetActive(false);
        stashUIBox.SetActive(false);
    }

    public void PressedStash()
    {
        playTab.SetActive(false);
        stashTab.SetActive(true);
        tradeTab.SetActive(false);
        lobbyTab.SetActive(false);
        loadingTab.SetActive(false);
        stashUIBox.SetActive(true);
    }
    public void PressedTrade()
    {
        playTab.SetActive(false);
        stashTab.SetActive(false);
        tradeTab.SetActive(true);
        lobbyTab.SetActive(false);
        loadingTab.SetActive(false);
        stashUIBox.SetActive(true);
    }
    public void PressedLobby()
    {
        playTab.SetActive(false);
        stashTab.SetActive(false);
        tradeTab.SetActive(false);
        lobbyTab.SetActive(true);
        loadingTab.SetActive(false);
        stashUIBox.SetActive(false);
    }

    public void MakeSureLoadingIsOn()
    {
        playTab.SetActive(true);
        stashTab.SetActive(true);
        tradeTab.SetActive(true);
        lobbyTab.SetActive(true);
        loadingTab.SetActive(true);
        stashUIBox.SetActive(true);
    }
}
