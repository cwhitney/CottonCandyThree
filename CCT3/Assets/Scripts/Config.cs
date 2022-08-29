using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Config : MonoBehaviour
{

	public float maxWindX = 20.0f;
	public float windY = 2.29f;
	public float stirInertia = 0.7f;
	public string oscClient = "127.0.0.1";
	public int oscPort = 8000;
	public int ambientParticleCount = 80;
	public int maxParticles = 5000;
	public bool useMouseDebug = false;

	public bool isActive = false;

	private static Config _instance;

	public GameObject mGui;

	public static Config Instance {
		get {
			if (_instance == null) {
				throw new System.InvalidOperationException("Config was accessed before it was instantiated.");
			}
			return _instance;
		}
	}

	void Awake() {
		_instance = this;
		mGui.SetActive(false);
	}

	private float lmap(float val, float inMin, float inMax, float outMin, float outMax) {
		return outMin + ((outMax - outMin) * (val - inMin)) / (inMax - inMin);
	}

	public void Save() {
		Debug.Log("Save Settings");

		maxWindX = float.Parse(mGui.transform.Find("maxWindX").gameObject.GetComponent<TMP_InputField>().text);
		windY = float.Parse(mGui.transform.Find("windY").gameObject.GetComponent<TMP_InputField>().text);
		stirInertia = float.Parse(mGui.transform.Find("stirInertia").gameObject.GetComponent<TMP_InputField>().text);
		oscClient = mGui.transform.Find("oscClient").gameObject.GetComponent<TMP_InputField>().text;
		oscPort = int.Parse(mGui.transform.Find("oscPort").gameObject.GetComponent<TMP_InputField>().text);
		ambientParticleCount = int.Parse(mGui.transform.Find("ambientParticleCount").gameObject.GetComponent<TMP_InputField>().text);
		maxParticles = int.Parse(mGui.transform.Find("maxParticles").gameObject.GetComponent<TMP_InputField>().text);
		useMouseDebug = mGui.transform.Find("useMouseDebug").gameObject.GetComponent<Toggle>().isOn;

		PlayerPrefs.SetFloat("maxWindX", maxWindX);
		PlayerPrefs.SetFloat("windY", windY);
		PlayerPrefs.SetFloat("stirInertia", stirInertia);
		PlayerPrefs.SetString("oscClient", oscClient);
		PlayerPrefs.SetInt("oscPort", oscPort);
		PlayerPrefs.SetInt("ambientParticleCount", ambientParticleCount);
		PlayerPrefs.SetInt("maxParticles", maxParticles);
		PlayerPrefs.SetInt("useMouseDebug", useMouseDebug ? 1 : 0);

		PlayerPrefs.Save();
	}

	public void Load() {
		maxWindX = PlayerPrefs.GetFloat("maxWindX");
		windY = PlayerPrefs.GetFloat("windY");
		stirInertia = PlayerPrefs.GetFloat("stirInertia");
		oscClient = PlayerPrefs.GetString("oscClient");
		oscPort = PlayerPrefs.GetInt("oscPort");
		ambientParticleCount = PlayerPrefs.GetInt("ambientParticleCount");
		maxParticles = PlayerPrefs.GetInt("maxParticles");
		useMouseDebug = PlayerPrefs.GetInt("useMouseDebug") == 1;
	}

	public void Show() {
		isActive = true;
		mGui.SetActive(true);

		mGui.transform.Find("maxWindX").gameObject.GetComponent<TMP_InputField>().text = maxWindX.ToString("#.00");
		mGui.transform.Find("windY").gameObject.GetComponent<TMP_InputField>().text = windY.ToString("#.00");
		mGui.transform.Find("stirInertia").gameObject.GetComponent<TMP_InputField>().text = stirInertia.ToString("#.00");
		mGui.transform.Find("oscClient").gameObject.GetComponent<TMP_InputField>().text = oscClient.ToString();
		mGui.transform.Find("oscPort").gameObject.GetComponent<TMP_InputField>().text = oscPort.ToString();
		mGui.transform.Find("ambientParticleCount").gameObject.GetComponent<TMP_InputField>().text = ambientParticleCount.ToString();
		mGui.transform.Find("maxParticles").gameObject.GetComponent<TMP_InputField>().text = maxParticles.ToString();
		mGui.transform.Find("useMouseDebug").gameObject.GetComponent<Toggle>().isOn = useMouseDebug;
	}

	public void Hide() {
		Save();

		mGui.SetActive(false);
		isActive = false;
	}
}
