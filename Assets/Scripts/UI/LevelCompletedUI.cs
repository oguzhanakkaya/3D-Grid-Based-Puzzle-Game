using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static GameEvents;
using System.Linq;
using DG.Tweening;

public class LevelCompletedUI : MonoBehaviour
{
    [SerializeField]private Button continueButton,rwButton;
    [SerializeField] private TextMeshProUGUI continueButtonText,rwButtonText,multiplierResultCurrentMultpText,progressBarText;
    private int multiplier;

    public static Action RwButtonPressedEvent;
    public EventBus _eventBus;

    [SerializeField] private Image newItemIconParent, newItemIcon,progressBar;
    [SerializeField] private Transform  header,headerBottomPos;

    private float progressBarStartFill;


    public void Initialize()
    {
        _eventBus = ServiceLocator.Instance.Resolve<EventBus>();
        _eventBus.Subscribe<GameEvents.OnLevelCompleted>(LevelCompleted);

        continueButton.onClick.AddListener(ContinueButtonPressed);
    }
    private void LevelCompleted()
    {
        Run.After(2f, () =>
        {
            gameObject.SetActive(true);
            StartCoroutine(ContinueButtonActive());
        });
      
      //  SetUnlockableItem();
 
    }
    private void ContinueButtonPressed()
    {
        GameManager.Instance.OnCoinGained(5);
        LevelManager.Instance.LoadNextLevel();
        gameObject.SetActive(false);
    }
    public void IndicatorChanged(int i)
    {
        multiplier = i;

        multiplierResultCurrentMultpText.text = multiplier.ToString() + "x";
        SetRwButtonText();
    }
    public void SetRwButtonText()
    {
        rwButtonText.text = (5 * multiplier).ToString();
    }
    private IEnumerator ContinueButtonActive()
    {
        continueButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        continueButton.gameObject.SetActive(true);
    }
   
}
