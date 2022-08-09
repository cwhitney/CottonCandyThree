using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarParticle : MonoBehaviour {

	public Texture2D[] textureList;

	Vector3		pos = new Vector3(0,0);
	float		scale = 1.0f;
	float		rotation = 0.0f;

	Vector3		velocity;
	Vector3		accel;
	float		mass = 5.0f;
	bool		bPersistent = true;
	float		rotationalVel = 0.01f;
	float		explodeTime = 0.0f;

	// Start is called before the first frame update
	void Start() {
		int rnd = Random.Range(0, textureList.Length);
		this.GetComponent<Renderer>().material.SetTexture("_MainTex", textureList[rnd]);
	}

	public void UpdatePos(Vector3 wind) {
		accel = wind / mass;
		pos += velocity + accel;
		velocity *= 0.97f;

		if (bPersistent) {
			if(pos.x > 1920.0/2.0) {
				pos.x = 0;
			}
		}

		this.transform.position = pos;
	}
}
