using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sharkbox;

public class CCTApp : MonoBehaviour
{
	public AudioManager mAudioManager;
	public InputManager mInputManager;
	public OscManager mOsc;

	SugarSystem mSugarSystem = new SugarSystem();
	SerialManager mSerial = new sharkbox.SerialManager();

	public float maxWindSpeedHorz = 20.0f;
	public float maxWindSpeedVert = 2.29f;

	// Start is called before the first frame update
	void Start() {
		Config.Instance.Load();
		UpdateSettings();

		mSugarSystem.Setup();
		mOsc.Open();

		mInputManager.OnChangeDir += OnStirChangeDir;
		mInputManager.OnWindSpeedChanged += OnWindSpeedUpdate;

		List<PortInfo> portList  = mSerial.GetPortsDetail(); ;
		foreach( PortInfo p in portList) {
			Debug.Log(p.Name + " / " + p.Description);

			if(p.Description.Contains("Arduino")) {
				Debug.Log("Found the arduino");
				mSerial.Connect(p.Name, 9600);
			}
		}
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

		while(mSerial.LinesAvailable > 0) {
			string serialMsg = mSerial.ReadLine();
			Debug.Log("Message from Arduino: " + serialMsg);
			if(serialMsg == "g") {
				StartPressed();
			}else if(serialMsg == "s") {
				StopPressed();
			}
		}

		//mSugarSystem.SetWind(new Vector3(Mathf.Sin(Time.fixedTime * 0.5f) * maxWindSpeedHorz, maxWindSpeedVert, 0.0f));	// max wind speed 20.0
		mSugarSystem.Update();

		if (Input.GetKeyDown(KeyCode.G)) {
			Debug.Log("Start spawning");
			StartPressed();
		} else if (Input.GetKeyDown(KeyCode.S)) {
			Debug.Log("Stop spawning");
			StopPressed();
		} else if (Input.GetKeyDown(KeyCode.D)) {
			if (Config.Instance.isActive) {
				Config.Instance.Hide();
				UpdateSettings();
			} else {
				Config.Instance.Show();
			}
		}
	}

	void UpdateSettings() {
		mSugarSystem.MAX_PARTICLES = Config.Instance.maxParticles;
		mSugarSystem.INITIAL_PARTICLES = Config.Instance.ambientParticleCount;
		maxWindSpeedHorz = Config.Instance.maxWindX;
		maxWindSpeedVert = Config.Instance.windY;
		mInputManager.windChangeAmount = Config.Instance.stirInertia;
		mInputManager.showDebugMouse = Config.Instance.useMouseDebug;
		mOsc.host = Config.Instance.oscClient;
		mOsc.port = Config.Instance.oscPort;
	}

	void StartPressed() {
		mSugarSystem.StartSpawningRamp();
		mOsc.SendStart();
	}

	void StopPressed() {
		mSugarSystem.StopSpawning();
		mOsc.SendStop();
	}
}
