using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarSystem : MonoBehaviour
{
	GameObject[] mParticlePool;

	public int mNumToSpawn = 0;

	public int MAX_PARTICLES = 5000;
	public int INITIAL_PARTICLES = 80;
	public const float STIR_SPAWN_SPEED = 0.1f;

	AudioManager mAudioManager;

	float mRampStartTime = 0.0f;
	bool bRampingUp = false;
	bool bExplodeOnly = false;
	float mSpawnEvery = 0.0f;
	float mSpawnNext = 0.0f;

	Vector3 windSpeed = new Vector3(2f, 2.5f, 0);

	public void Setup() {
		mParticlePool = new GameObject[MAX_PARTICLES];
		mAudioManager = GameObject.FindObjectOfType<AudioManager>();
		mAudioManager.Play("CCT_idle", true, true);

		// Make a pool of all particles
		for (int i=0; i< MAX_PARTICLES; i++) {
			mParticlePool[i] = Instantiate( Resources.Load("SugarParticle"), new Vector3(0, 0, 0), Quaternion.Euler(0,0,0) ) as GameObject;
		}

		// Spawn all the ambient particles
		for(int i=0; i<INITIAL_PARTICLES; i++) {
			mParticlePool[i].GetComponent<SugarParticle>().Spawn(true);
		}
	}

	public void Update()
	{
		if (bRampingUp) {
			float elapsed = Time.fixedTime - mRampStartTime;
			float norm = Mathf.Clamp((float)elapsed / 30.0f, 0f, 1f);
			float normE = Mathf.Pow(norm, 0.3f);
			//        console() << "Ramp :: " << (normE * mRampMax) << " / " << norm * 100.0 << endl;
			mSpawnEvery = lmap(normE, 0.0f, 1.0f, STIR_SPAWN_SPEED, 0.03f);
		}

		// Update Live Particles
		foreach (GameObject p in mParticlePool) {
			SugarParticle sp = p.GetComponent<SugarParticle>();

			if (sp.isAlive) {
				sp.UpdatePos(windSpeed);
			}
			else if(mNumToSpawn > 0) {
				sp.Spawn(false);

				mNumToSpawn--;
			}
		}

		// ----------  HANDLE SPAWN EVERY
		if (mSpawnEvery > 0.0) {
			if (Time.fixedTime > mSpawnNext) {
				++mNumToSpawn;
				mSpawnNext = Time.fixedTime + mSpawnEvery;
			}
		}
	}

	public void StartSpawningRamp() {
		mRampStartTime = Time.fixedTime;
		bRampingUp = true;
		bExplodeOnly = false;

		mAudioManager.Play("CCT_song", true);
		mAudioManager.Stop("CCT_idle", true);
	}

	public void StopSpawning() {
		mSpawnEvery = 0.0f;
		mSpawnNext = 0.0f;
		bRampingUp = false;
		bExplodeOnly = true;

		float elapsed = Time.fixedTime - mRampStartTime;
		if (elapsed < 40.0) {   // play the ending sting, then go back to idle
			mAudioManager.Stop("CCT_song", true);
			mAudioManager.Play("CCT_endingblaster", false, false);
			mAudioManager.Play("CCT_idle", true, true);
		} else {
			mAudioManager.Stop("CCT_song", true);
			mAudioManager.Play("CCT_idle", true, true);
		}
	}

	public void SetWind(Vector3 wind) {
		windSpeed = wind;
	}

	float lmap(float val, float inMin, float inMax, float outMin, float outMax) {
		return outMin + ((outMax - outMin) * (val - inMin)) / (inMax - inMin);
	}
}
