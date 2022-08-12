using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExplodeEventRelay : MonoBehaviour
{
	public UnityAction ExplodeCompleted;

	public void OnExplodeComplete() {
		ExplodeCompleted.Invoke();
	}
}
