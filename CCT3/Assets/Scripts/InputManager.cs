using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircularBuffer;

public class InputManager : MonoBehaviour
{
	float mCircularDirection = 0f;
	float mPitchBend = 0.0f;

	CircularBuffer<float> mAngleBuffer;

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
			Debug.Log("Got some touches");
			Touch t = Input.GetTouch(0);
			Debug.Log(t.position);
		}

		// get the position and normalize from -1 to 1 with 0,0 in the center
	}

	int CalculateDirection() {

		int angleMovedPos = 0;
		int angleMovedNeg = 0;
		int numSamples = 60;
		/*
		mCircularDirection = 0;
		for (int i = 0; i < numSamples; i++) {
			

			float hh = mAngleBuffer[2];

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
				(angle > angleP) ? (++angleMovedPos) : (++angleMovedNeg);
			}
		}

		float confidence = 0.0;

		if (mCircularDirection > 0.0) {
			confidence = (float)angleMovedPos / (float)numSamples;

			if (confidence > mModel->mStirConfidenceThresh) {
				if (!bIsCW) {
					mPitchBend = 0.0;
					signalChangeDir.emit(1);
				} else {
					mPitchBend = math<float>::clamp(mPitchBend + 0.02);
					mModel->mWindX = math<float>::clamp(mModel->mWindX + 0.04, -1.0, 1.0);     // wind x
				}
				bIsCW = true;
			} else {
				mPitchBend *= 0.93;
				mModel->mWindX *= 0.999;
			}
		} else if (mCircularDirection < 0.0) {
			confidence = (float)angleMovedNeg / (float)numSamples;

			if (confidence > mModel->mStirConfidenceThresh) {
				if (bIsCW) {
					mPitchBend = 0.0;
					signalChangeDir.emit(-1);
				} else {
					mPitchBend = math<float>::clamp(mPitchBend + 0.02);
					mModel->mWindX = math<float>::clamp(mModel->mWindX - 0.04, -1.0, 1.0);     // wind x
				}
				bIsCW = false;
			} else {
				mPitchBend *= 0.93;
				mModel->mWindX *= 0.999;
			}
		}

		if (mPitchBend < 0.001) {
			mPitchBend = 0.0;
		}

		if (bIsCW) {
			signalWandSpeedDir.emit(mPitchBend);
		} else {
			signalWandSpeedDir.emit(-mPitchBend);
		}
		*/
		return 0;
	}
}
