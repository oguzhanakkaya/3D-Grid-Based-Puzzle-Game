using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mai.MainMenu
{
    public class MapTouchListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public static event Action TouchStartEvent;
        public static event Action TouchEndEvent;
        public void OnPointerDown(PointerEventData eventData)
        {
            TouchStartEvent?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            TouchEndEvent?.Invoke();
        }
    }
}