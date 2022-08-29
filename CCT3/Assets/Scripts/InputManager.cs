using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using CircularBuffer;

public class InputManager : MonoBehaviour
{
	public delegate void changeDirDelegate(int dir);
	public changeDirDelegate OnChangeDir;

	public delegate void changeWindDelegate(float speed);
	public changeWindDelegate OnWindSpeedChanged;

	public bool showDebugMouse = false;

	public float windChangeAmount = 0.75f;
	public UnityEngine.UI.RawImage mouseDebugImg;

	CircularBuffer<float> mAngleBuffer;
	bool bIsCW = true;

	const float mStirConfidenceThresh = 0.0f;
	float mWindX = 0.0f;
	float mCircularDirection = 0f;
	float mPitchBend = 0.0f;
	Vector2 mp = new Vector2();

	// Start is called before the first frame update
	private void Awake() {
		mouseDebugImg.enabled = false;
	}

	void Start() {
		mAngleBuffer = new CircularBuffer<float>(300);
	}

	// Update is called once per frame
	void Update() {
		Resolution res = UnityEngine.Screen.currentResolution;
		float w = Screen.width;
		float h = Screen.height;

		if (Input.touchCount > 0 || Input.GetMouseButton(0)) {
			if(Input.touchCount > 0) {
				Touch t = Input.GetTouch(0);
				mp.Set(t.position.x, t.position.y);
			} else {
				mp.Set(Input.mousePosition.x, Input.mousePosition.y);
			}

			if (showDebugMouse) {
				mouseDebugImg.enabled = true;
				mouseDebugImg.transform.position = new Vector3(mp.x, mp.y, 0);
			} else {
				mouseDebugImg.enabled = false;
				//mouseDebugImg.transform.position = new Vector3(-9999f, -9999f, 0);
			}

			float angleRads = Mathf.Atan2(mp.y - h*0.5f, mp.x - w*0.5f);
			float ang = 180f / 3.1415926f * angleRads;
			if (ang < 0) { ang += 360; }

			mAngleBuffer.PushBack( ang );
			CalculateDirection();
		}

		mWindX *= 0.999f;
		OnWindSpeedChanged?.Invoke(mWindX);

		// get the position and normalize from -1 to 1 with 0,0 in the center
	}

	int CalculateDirection() {

		int angleMovedPos = 0;
		int angleMovedNeg = 0;
		int numSamples = 60;
		
		mCircularDirection = 0;
		for (int i = 0; i < numSamples; i++) {
			float angle = mAngleBuffer.Back(i);
			float angleP = mAngleBuffer.Back(i + 1);
			if (angle > 340 && angleP < 20) {        // ccw
				mCircularDirection += angle - (angleP + 360);
				++angleMovedNeg;
			} else if (angle < 20 && angleP > 340) {  // cw
				mCircularDirection += (angle + 360) - angleP;
				++angleMovedPos;
			} else {
				mCircularDirection += (angle - angleP);
				if (angle > angleP) {
					++angleMovedPos;
				} else { 
					++angleMovedNeg;
 				}
			}
		}

		float confidence = 0.0f;

		if (mCircularDirection > 0.0) {
			confidence = (float)angleMovedPos / (float)numSamples;

			if (confidence > mStirConfidenceThresh) {
				if (!bIsCW) {
					OnChangeDir?.Invoke(1);
				} else {
					mPitchBend = Mathf.Clamp01(mPitchBend + 0.02f);
					mWindX = Mathf.Clamp(mWindX - windChangeAmount * Time.deltaTime, -1.0f, 1.0f);     // wind x
				}
				bIsCW = true;
			}
		} else if (mCircularDirection < 0.0) {
			confidence = (float)angleMovedNeg / (float)numSamples;

			if (confidence > mStirConfidenceThresh) {
				if (bIsCW) {
					OnChangeDir?.Invoke(-1);
				} else {
					mWindX = Mathf.Clamp(mWindX + windChangeAmount * Time.deltaTime, -1.0f, 1.0f);     // wind x
				}
				bIsCW = false;
			}
		}

		return (bIsCW == true) ? -1 : 1;
	}
}
