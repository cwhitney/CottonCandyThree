using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarParticle : MonoBehaviour {

	public Texture2D[] textureList;
	public bool isAlive = false;
	public GameObject particlePlane;
	public GameObject explodeSpriteGo;

	Vector3		pos = new Vector3(0,0);
	float		scale = 1.0f;
	float		rotation = 0.0f;

	Vector3		velocity;
	Vector3		accel;
	float		mass = 1.0f;
	bool		bPersistent = true;
	float		rotationalVel = 0.01f;
	float		explodeTime = 0.0f;

	const float edgeBuf = 100f;

	float minX = 1920f * -0.5f - edgeBuf;
	float maxX = 1920f * 0.5f + edgeBuf;
	float minY = 1080f * -0.5f - edgeBuf;
	float maxY = 1080f * 0.5f + edgeBuf;

	const float minMass = 5.0f;
	const float maxMass = 10.0f;

	// Start is called before the first frame update
	void Awake() {
		this.gameObject.SetActive(false);
		this.explodeSpriteGo.SetActive(false);

		// Listen to when the explode animation completes. Because of how Animator's work, we needed to relay the event from an Action
		explodeSpriteGo.GetComponent<ExplodeEventRelay>().ExplodeCompleted += OnExplodeFinish;
	}

	public void Spawn(bool isPersistent = false) {
		isAlive = true;
		mass = Random.Range(minMass, maxMass);
		accel = new Vector3(0.0f, Random.Range(30, 65), 0.0f) / mass;
		velocity = accel;

		bPersistent = isPersistent;
		rotationalVel = Random.Range(-0.7f, 0.7f);

		if (bPersistent) {
			pos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0f);
		} else {
			pos = new Vector3(Random.Range(minX, maxX), minY+1, 0f);    // start offscreen
			explodeTime = Random.Range(2.0f, 4.0f);
		}

		scale = mass * 1.0f;
		rotation = Random.Range(0.0f, Mathf.PI * 2.0f);

		int rnd = Random.Range(0, textureList.Length);
		particlePlane.GetComponent<Renderer>().material.SetTexture("_MainTex", textureList[rnd]);

		this.transform.localScale = new Vector3(scale, scale, scale);
		this.transform.position = pos;
		//particlePlane.GetComponent<Renderer>().enabled = true;

		particlePlane.SetActive(true);
		explodeSpriteGo.SetActive(false);
		this.gameObject.SetActive(true);

		if(this.bPersistent == false) {
			StartCoroutine(WaitExplode(explodeTime));
		}
	}

	public IEnumerator WaitExplode(float secsUntilExplode) {
		yield return new WaitForSeconds(secsUntilExplode);
		Explode();
	}

	void Explode() {
		particlePlane.SetActive(false);
		explodeSpriteGo.SetActive(true);

		explodeSpriteGo.GetComponent<Animator>().Play("SugarExplodeAnim");
	}

	public void OnExplodeFinish() {
		//Debug.Log("Explode finish!");

		particlePlane.SetActive(false);
		explodeSpriteGo.SetActive(false);
		this.isAlive = false;
	}

	public void UpdatePos(Vector3 wind) {
		if (!isAlive) {
			return;
		}


		accel = wind / mass;
		pos += (velocity + accel) * 100.0f * Time.deltaTime;
		velocity *= (1.0f - Time.deltaTime);
		//velocity *= 0.97f * Time.deltaTime;
		rotation += rotationalVel * Time.deltaTime;

		


		// Wrap persistent particles
		if (bPersistent) {
			if (pos.x > maxX) {
				pos.x = minX;
			} else if (pos.x < minX) {
				pos.x = maxX;
			}

			if (pos.y > maxY) {
				pos.y = minY;
			} else if (pos.y < minY) {
				pos.y = maxY;
			}
		} else {    // Despawn non-persistent
			if (pos.x > maxX ||
				pos.x < minX ||
				pos.y > maxY) {
				scale = 0.0f;
				isAlive = false;
				this.gameObject.SetActive(false);
			}
		}

		this.transform.position = pos;
		this.transform.rotation = Quaternion.Euler(0,0, rotation * 180.0f / 3.1415926f);
		//this.transform.rotation = Quaternion.Euler(0,0,0);
	}
}
