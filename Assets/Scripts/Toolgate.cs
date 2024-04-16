using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameEvents;
using DG.Tweening;

public class Toolgate : MonoBehaviour
{
    public EventBus _eventBus;

    public Transform barrier;

    public Vector3 rotation;

    void Awake()
    {
        _eventBus = ServiceLocator.Instance.Resolve<EventBus>();
        _eventBus.Subscribe<GameEvents.OnLevelCompleted>(OnLevelCompleted);
        _eventBus.Subscribe<GameEvents.OnLevelCompleted>(OnLevelLoaded);

        rotation = transform.localEulerAngles;
    }
    private void OnDestroy()
    {
        _eventBus.Unsubscribe<GameEvents.OnLevelCompleted>(OnLevelCompleted);
    }
    public void OnLevelLoaded()
    {
        GetComponent<Collider>().enabled = true;

        barrier.DOLocalRotate(rotation, 0f, RotateMode.Fast);
    }

    public void OnLevelCompleted()
    {
        GetComponent<Collider>().enabled = false;

        barrier.DOLocalRotate(new Vector3(0,90,-180),.2f,RotateMode.Fast);
    }
}
