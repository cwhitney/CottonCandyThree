using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CircularBuffer;

public class InputManager : MonoBehaviour
{
	public delegate void changeDirDelegate(int dir);
	public changeDirDelegate OnChangeDir;

	public delegate void changeWindDelegate(float speed);
	public changeWindDelegate OnWindSpeedChanged;

	public float windChangeAmount = 0.04f;

	CircularBuffer<float> mAngleBuffer;
	bool bIsCW = true;

	const float mStirConfidenceThresh = 0.0f;
	float mWindX = 0.0f;
	float mCircularDirection = 0f;
	float mPitchBend = 0.0f;

	// Start is called before the first frame update
	void Start() {
		Cursor.lockState = CursorLockMode.Confined;

		mAngleBuffer = new CircularBuffer<float>(300);
	}

	// Update is called once per frame
	void Update() {
		Vector3 mp = Input.mousePosition;
		Resolution res = UnityEngine.Screen.currentResolution;
		float w = Screen.width;
		float h = Screen.height;
		//Debug.Log(mp);

		//Press the space bar to apply no locking to the Cursor
		if (Input.GetKey(KeyCode.Space)) {
			if (Cursor.lockState == CursorLockMode.None) {
				Cursor.lockState = CursorLockMode.Confined;
			} else {
				Cursor.lockState = CursorLockMode.None;
			}
		}

		if (Input.touchCount > 0) {
			Touch t = Input.GetTouch(0);
			
			float angleRads = Mathf.Atan2(t.position.y - h*0.5f, t.position.x - w*0.5f);
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
					//mPitchBend = 0.0;
					OnChangeDir?.Invoke(1);
				} else {
					mPitchBend = Mathf.Clamp01(mPitchBend + 0.02f);
					mWindX = Mathf.Clamp(mWindX + windChangeAmount * Time.deltaTime, -1.0f, 1.0f);     // wind x
				}
				bIsCW = true;
			} else {
				//mPitchBend *= 0.93;
				//mWindX *= 0.999f;
			}

			//OnWindSpeedChanged?.Invoke(mWindX);
		} else if (mCircularDirection < 0.0) {
			confidence = (float)angleMovedNeg / (float)numSamples;

			if (confidence > mStirConfidenceThresh) {
				if (bIsCW) {
					//mPitchBend = 0.0;
					//signalChangeDir.emit(-1);
					OnChangeDir?.Invoke(-1);
				} else {
					//mPitchBend = math<float>::clamp(mPitchBend + 0.02);
					mWindX = Mathf.Clamp(mWindX - windChangeAmount * Time.deltaTime, -1.0f, 1.0f);     // wind x
				}
				bIsCW = false;
			} else {
				//mPitchBend *= 0.93;
				//mWindX *= 0.999f;
			}

			//OnWindSpeedChanged?.Invoke(mWindX);
		}

		//if (mPitchBend < 0.001) {
		//	mPitchBend = 0.0;
		//}

		//if (bIsCW) {
		//	signalWandSpeedDir.emit(mPitchBend);
		//} else {
		//	signalWandSpeedDir.emit(-mPitchBend);
		//}
		
		return 0;
	}
}
