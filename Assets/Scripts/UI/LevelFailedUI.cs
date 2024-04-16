using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameEvents;
using System;

public class LevelFailedUI : MonoBehaviour
{
    [SerializeField] private Button notThanksButton,continueButton;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI infoText;

    public EventBus _eventBus;


    public void Initialize()
    {
        notThanksButton.onClick.AddListener(RetryButtonPressed);
        continueButton.onClick.AddListener(ContinueButtonPressed);

        _eventBus = ServiceLocator.Instance.Resolve<EventBus>();
        _eventBus.Subscribe<GameEvents.OnLevelFailed>(OnLevelFailed);

    }
    private void OnLevelFailed()
    {
        gameObject.SetActive(true);
    }
    private void RetryButtonPressed()
    {
        gameObject.SetActive(false);
        LevelManager.Instance.LoadNextLevel();
        _eventBus.Fire(new GameEvents.OnLevelGiveUp());
    }
    private void ContinueButtonPressed()
    {
        if (GameManager.Instance.coin>= 100)
        {
            GameManager.Instance.OnCoinSpent(100);
            gameObject.SetActive(false);
            GameManager.Instance.GiveExtraTime(RemoteManager.instance.levelFailedAddExtraTime);
        }
        else
        {
            UIManager.Instance.OpenStore();

        }
    
      
        
    }
}
