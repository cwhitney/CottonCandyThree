using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTApp : MonoBehaviour
{
	public AudioManager mAudioManager;

	OscManager mOsc;
	SugarSystem mSugarSystem = new SugarSystem();

	// Start is called before the first frame update
	void Start() {
		mSugarSystem.Setup();
	}

	// Update is called once per frame
	void Update() {
		mSugarSystem.SetWind(new Vector3(Mathf.Sin(Time.fixedTime * 0.5f) * 20.0f, 2.29f, 0.0f));	// max wind speed 20.0
		//mSugarSystem.SetWind(new Vector3(0, 2.29f, 0.0f));	// max wind speed 20.0
		mSugarSystem.Update();

		if (Input.GetKeyDown(KeyCode.S)) {
			Debug.Log("Start spawning");
			StartPressed();
		} else if (Input.GetKeyDown(KeyCode.T)) {
			Debug.Log("Stop spawning");

			StopPressed();
		}
	}

	void StartPressed() {
		mSugarSystem.StartSpawningRamp();
	}

	void StopPressed() {
		mSugarSystem.StopSpawning();
	}
}
