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
		mAudioManager.Play("CCT_idle", true, true);
	}

	// Update is called once per frame
	void Update() {
		mSugarSystem.SetWind(new Vector3(Mathf.Sin(Time.fixedTime * 0.5f) * 5.0f, 2.29f, 0.0f));	// max wind speed 20.0
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
		mAudioManager.Play("CCT_song", true);
		mAudioManager.Stop("CCT_idle", true);
	}

	void StopPressed() {
		mSugarSystem.StopSpawning();

		mAudioManager.Stop("CCT_song", true);
		mAudioManager.Play("CCT_idle", true, true);
	}
}
