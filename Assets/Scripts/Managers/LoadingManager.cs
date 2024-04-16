using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    public InputManager inputManager;
    public LevelManager levelManager;
    public UIManager uiManager;
    public GameManager gameManager;

    private void Awake()
    {
        Instance = this;
        SplashScreenCoroutine();
    }
    private void SplashScreenCoroutine()
    {
        Container.Initialize();
        InitiliazeManagers();
        ServiceLocator.Instance.Resolve<EventBus>().Fire(new GameEvents.OnSplashScreenFinished()); ;
    }
    private void InitiliazeManagers()
    {
        gameManager.Initialize();
        inputManager.Initialize();
        levelManager.Initialize();
        uiManager.Initialize();
    }
}
