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
	}
}
