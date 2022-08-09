using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTApp : MonoBehaviour
{
	OscManager mOsc;
	SugarSystem mSugarSystem = new SugarSystem();

	// Start is called before the first frame update
	void Start() {
		mSugarSystem.Setup();
	}

	// Update is called once per frame
	void Update()
	{
		mSugarSystem.Update();

		mSugarSystem.SetWind(new Vector3(Mathf.Sin(Time.fixedTime * 0.5f) * 5.5f, 2.5f, 0.0f) );

		if (Input.GetKeyDown(KeyCode.S)) {
			Debug.Log("Start spawning");
			StartPressed();
		}
	}

	void StartPressed() {
		mSugarSystem.StartSpawningRamp();
	}

	void StopPressed() {
	
	}
}
