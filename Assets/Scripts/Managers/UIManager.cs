using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEvents;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public EventBus _eventBus;

    public LevelCompletedUI levelCompletedUI;
    public LevelFailedUI levelFailedUI;
    public GameUI gameUI;
    public Store store;
    public Tutorial tutorial;


    [SerializeField] private GameObject storePanel;

    private bool isGame;


    public List<TextMeshProUGUI> coinTextList = new List<TextMeshProUGUI>();

    public void Initialize()
    {
        Instance = this;

        _eventBus = ServiceLocator.Instance.Resolve<EventBus>();
        _eventBus.Subscribe<GameEvents.OnCoinChanged>(OnCoinChanged);
        _eventBus.Subscribe<GameEvents.OnLevelStarted>(OnLevelStarted);

        levelCompletedUI.Initialize();
        levelFailedUI.Initialize();
        gameUI.Initialize();
        store.Initialize();
        tutorial.Initialize();

    }
    private void OnCoinChanged(GameEvents.OnCoinChanged p)
    {
        SetCoinText(p.coin);
    }
    public void SetCoinText(int i)
    {
        foreach (var item in coinTextList)
        {
            item.text = i.ToString();
        }
    }
    private void OnLevelStarted(GameEvents.OnLevelStarted p)
    {
        SetCoinText(GameManager.Instance.coin);
    }
    public void OpenStore(bool isGame=false)
    {
        storePanel.gameObject.SetActive(true);
        GameManager.Instance.SetInputManager(false);

        this.isGame = isGame;

        if (isGame)
            GameUI.Instance.StopTimer();
    }
    public void CloseStore(bool isGame)
    {
        storePanel.gameObject.SetActive(false);
        GameManager.Instance.SetInputManager(true);

        if (this.isGame)
            GameUI.Instance.StartTimer();
    }
}
