using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameEvents;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public EventBus _eventBus;

    public int coin;

    public InputManager inputManager;

    private Coroutine carCompletedCoroutine;

    public void Initialize()
    {
        Instance = this;

        _eventBus = ServiceLocator.Instance.Resolve<EventBus>();

        _eventBus.Subscribe<GameEvents.OnLevelLoaded>(OnLevelLoaded);
        _eventBus.Subscribe<GameEvents.OnLevelCompleted>(OnLevelCompleted);

        Application.targetFrameRate = 60;

        coin = PlayerPrefs.GetInt("Coin", 0);

        Run.After(.25f, () =>
        {
            _eventBus.Fire(new GameEvents.OnCoinChanged(coin));
        });
    }
    private void OnLevelLoaded()
    {
        SetInputManager(true);

        if (carCompletedCoroutine!=null)
            StopCoroutine(carCompletedCoroutine);

    }
    private void OnLevelCompleted()
    {
        carCompletedCoroutine=StartCoroutine(LevelManager.Instance.LevelCompletedCarEffect());
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 0) + 1);
    }
    public void OnCoinGained(int i)
    {
        coin += i;
        UIManager.Instance.SetCoinText(coin);
        PlayerPrefs.SetInt("Coin",coin);
        _eventBus.Fire(new GameEvents.OnCoinChanged(coin));
    }
    public void OnCoinSpent(int i)
    {
        if (coin - i >= 0)
        {
            coin -= i;
            UIManager.Instance.SetCoinText(coin);
            PlayerPrefs.SetInt("Coin", coin);
            _eventBus.Fire(new GameEvents.OnCoinChanged(coin));
        }
    }
    public bool CheckHaveCoin(int i)
    {
        return coin >= i;
    }

    public void SetInputManager(bool boolean)
    {
        inputManager.enabled = boolean;
    }

    public void GiveExtraTime(int i)
    {
        GameUI.Instance.time = i;
        GameUI.Instance.StartTimer();
        //GameUI.Instance.SetTimeText(GameUI.Instance.time);
        SetInputManager(true);
    }
    

}
