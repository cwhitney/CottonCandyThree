using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarSystem : MonoBehaviour
{
	public int MAX_PARTICLES = 1;
	GameObject[] mParticlePool;

	public void Setup() {
		mParticlePool = new GameObject[MAX_PARTICLES];

		for (int i=0; i< MAX_PARTICLES; i++) {
			mParticlePool[i] = Instantiate( Resources.Load("SugarParticle"), new Vector3(0, 0, 0), Quaternion.Euler(-90f,0,0) ) as GameObject;
		}
	}

	public void Update()
	{
		Vector3 windSpeed = new Vector3(2f, 0, 0);

		foreach(GameObject p in mParticlePool) {
			SugarParticle sp = p.GetComponent<SugarParticle>();
			sp.UpdatePos(windSpeed);
		}
	}
}
