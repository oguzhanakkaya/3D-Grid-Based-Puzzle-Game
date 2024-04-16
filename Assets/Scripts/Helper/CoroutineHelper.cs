using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoroutineHelper : MonoBehaviourSingleton<CoroutineHelper>
{
	private List<Run> m_OnGUIObjects = new List<Run>();
	public int ScheduledOnGUIItems
	{
		get {return m_OnGUIObjects.Count;}
	}
}
