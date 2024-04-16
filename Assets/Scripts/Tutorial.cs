using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEvents;
using DG.Tweening;

public class Tutorial : MonoBehaviour
{
    private EventBus _eventBus;

    public static Tutorial Instance;

    public GameObject handImage;
    public GameObject staticBusPanel;

    public Sequence sequence;

    public Coroutine coroutine;

    public void Initialize()
    {

        _eventBus = ServiceLocator.Instance.Resolve<EventBus>();
        _eventBus.Subscribe<GameEvents.OnLevelLoaded>(OnLevelLoaded);
        _eventBus.Subscribe<GameEvents.OnItemUnclicked>(HideHandIcon);

        Instance = this;

        sequence = DOTween.Sequence();

       
    }
    public void OnLevelLoaded()
    {
        Debug.LogError(LevelManager.Instance.level);
        if (LevelManager.Instance.level==0)
        {
            ShowHandIcon();
        }
        else if (LevelManager.Instance.level == 6)
        {
            ShowStaticBusPanel();
        }
    }
    public void ShowStaticBusPanel()
    {
        staticBusPanel.SetActive(true);
    }
    public void ShowHandIcon()
    {
        handImage.SetActive(true);

        coroutine = StartCoroutine(HandAnimCoroutine());
     

    }
    public void HideHandIcon()
    {
        StopCoroutine(coroutine);
        DOTween.Kill(handImage.transform);

        handImage.SetActive(false);
    }
    public IEnumerator HandAnimCoroutine()
    {
        Vector3 firstPoint = Camera.main.WorldToScreenPoint(GridManager.GetNode(2, 5).worldPosition);

        Vector3 firstGridPoint = Camera.main.WorldToScreenPoint(GridManager.GetNode(0, 5).worldPosition);
        Vector3 secondGridPoint = Camera.main.WorldToScreenPoint(GridManager.GetNode(0, 1).worldPosition);

        handImage.transform.position = firstPoint;


        while (true)
        {
            handImage.transform.DOMoveX(firstGridPoint.x, .5f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(.5f);
            handImage.transform.DOMove(secondGridPoint, .5f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(.5f);
            handImage.transform.position = firstPoint;
        }
    }
    public void ShowStaticBusUI()
    {

    }
}
