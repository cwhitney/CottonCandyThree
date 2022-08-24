using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTApp : MonoBehaviour
{
	public AudioManager mAudioManager;
	public InputManager mInputManager;

	OscManager mOsc;
	SugarSystem mSugarSystem = new SugarSystem();

	public float maxWindSpeedHorz = 20.0f;
	public float maxWindSpeedVert = 2.29f;

	// Start is called before the first frame update
	void Start() {
		mSugarSystem.Setup();

		mInputManager.OnChangeDir += OnStirChangeDir;
		mInputManager.OnWindSpeedChanged += OnWindSpeedUpdate;

		//Application.targetFrameRate = 30;
	}

	void OnStirChangeDir(int dir) {
		//Debug.Log("Stir changed dir: " + dir);
	}

	void OnWindSpeedUpdate(float amt) {
		//Debug.Log("Wind amount: " + amt);
		mSugarSystem.SetWind(new Vector3(amt * maxWindSpeedHorz, maxWindSpeedVert, 0.0f));   // max wind speed 20.0
	}

	// Update is called once per frame
	void Update() {
		//mSugarSystem.SetWind(new Vector3(Mathf.Sin(Time.fixedTime * 0.5f) * maxWindSpeedHorz, maxWindSpeedVert, 0.0f));	// max wind speed 20.0
		//mSugarSystem.SetWind(new Vector3(0, 2.29f, 0.0f));	// max wind speed 20.0
		mSugarSystem.Update();

		if (Input.GetKeyDown(KeyCode.G)) {
			Debug.Log("Start spawning");
			StartPressed();
		} else if (Input.GetKeyDown(KeyCode.S)) {
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
