using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsAutoCloser : MonoBehaviour
{
     public GraphicRaycaster m_Raycaster;
    public PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    private List<RaycastResult> results = new List<RaycastResult>();

    public void Start()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
    }


    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_PointerEventData.position = Input.mousePosition;
            results.Clear();
            // Raycast
            m_Raycaster.Raycast(m_PointerEventData, results);

            // Check if any UI elements were hit
            if (results.Count > 0)
            {
                foreach (var item in results)
                {
                    if (item.gameObject.CompareTag("Settings"))
                    {
                        return;
                    }
                }
               // Settings.Instance.SettingsButtonPressed(this.gameObject);
            }
            else
            {
              //  Settings.Instance.SettingsButtonPressed(this.gameObject);
            }
        }
    }
}
