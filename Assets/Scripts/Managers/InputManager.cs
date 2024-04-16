using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using Unity.VisualScripting;
using UnityEngine;
using static GameEvents;

public class InputManager : MonoBehaviour
{
    private EventBus _eventBus;
    public BusController currentItem;

    public FakeBus currentFakeBus;

    private bool canClick;

    public void Initialize()
    {
        _eventBus = ServiceLocator.Instance.Resolve<EventBus>();
        _eventBus.Subscribe<GameEvents.OnLevelLoaded>(OnLevelLoaded);
        _eventBus.Subscribe<GameEvents.OnLevelEnded>(OnLevelEnded);
    }

    private void OnLevelLoaded() => canClick = true;  
    private void OnLevelEnded()  => canClick = false;

    void Update()
    {
        if (canClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                    if (hit.collider != null)
                        if (hit.collider.gameObject.TryGetComponent<BusController>(out BusController item) &&
                                                                                   item != null &&
                                                                                   !item.bus.isStaticBus &&
                                                                                   item.bus.canMove )
                        {
                            HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);

                            _eventBus.Fire(new GameEvents.OnItemClicked());
                            SpawnFakeBus(item);
                            item.ItemSelected();
                            currentItem = item;
                        }
            }
            if (Input.GetMouseButton(0))
            {
                if (!currentItem)
                    return;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                    if (hit.collider != null)
                        {
                        var worldPosition = hit.point;
                        worldPosition.y = currentItem.transform.position.y;
                        currentItem.rigidbody.velocity = (worldPosition - currentItem.rigidbody.position) * 35f;
                    }
            }


            if (Input.GetMouseButtonUp(0))
            {
                if (!currentItem)
                    return;

                HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
                DestroyFakeBus();
                currentItem.ItemDropped();
                currentItem = null;

                _eventBus.Fire(new GameEvents.OnItemUnclicked());

            }
        }
       
    }
    private void SpawnFakeBus(BusController busController)
    {
        if (currentFakeBus != null)
            return;


        if (busController.bus is ShortBus)
           currentFakeBus= LevelManager.Instance.SpawnFakeShortBus(busController.bus.transform.position,
                                                                   busController.bus.transform.localEulerAngles,
                                                                   busController.bus.numberOfPassanger,
                                                                   busController.bus.color);
       
        else
            currentFakeBus = LevelManager.Instance.SpawnFakeLongBus(busController.bus.transform.position,
                                                                   busController.bus.transform.localEulerAngles,
                                                                   busController.bus.numberOfPassanger,
                                                                   busController.bus.color);
    }
    private void DestroyFakeBus()
    {
        if (currentFakeBus)
            Destroy(currentFakeBus.gameObject);
    }
}
