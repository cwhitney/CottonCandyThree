using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundClip {
	public float volume = 1.0f;
	public AudioSource source;
	private string name;

	public SoundClip(AudioSource s) {
		source = s;
		name = s.clip.name;
	}

	public void Play(bool loop=false, float seek=0.0f) {
		Debug.Log("Playing audio "+name);
		source.loop = loop;
		source.time = seek;
		source.Play();
	}

	public void Stop() {
		source.Stop();
	}

	public string GetName() {
		return name;
	}

	public IEnumerator Crossfade(float from, float to, float duration, System.Action<int> callback=null) {
		float start = Time.fixedTime;
		bool exit = false;

		while (exit == false) {
			float pct = (Time.fixedTime - start) / duration;
			pct = Mathf.Clamp01(pct);
			
			source.volume = Mathf.Lerp(from, to, pct); 
			if(pct >= 1.0f) {
				exit = true;
			}

			yield return null;
		}

		if(callback != null) {
			callback(0);
		}
	}
}

public class AudioManager : MonoBehaviour
{
	public AudioClip[] audioList;
	public AudioClip[] shepardTones;

	private ArrayList soundList;
	private ArrayList shepardSoundList;

	// Start is called before the first frame update
	void Awake() {
		soundList = new ArrayList();
		shepardSoundList = new ArrayList();

		foreach (AudioClip a in audioList) {
			AudioSource asrc = gameObject.AddComponent<AudioSource>();
			asrc.clip = a;

			soundList.Add( new SoundClip(asrc) );
		}

		foreach (AudioClip a in shepardTones) {
			AudioSource asrc = gameObject.AddComponent<AudioSource>();
			asrc.clip = a;

			shepardSoundList.Add(new SoundClip(asrc));
		}
		Debug.Log("Clips loaded");
	}

	public void Play(string name, bool crossfade=false, bool loop=false, float seek=0.0f) {
		foreach(SoundClip s in soundList) {
			if(s.GetName() == name) {
				s.Play(loop, seek);
				if (crossfade) {
					StartCoroutine(s.Crossfade(0.0f, 1.0f, 1.0f));
				}
			}
		}
	}

	public void PlayShepardTone(int toneNum) {
		SoundClip s = shepardSoundList[toneNum] as SoundClip;
		s.Play();
	}

	public void Stop(string name, bool crossfade = false) {
		foreach (SoundClip s in soundList) {
			if (s.GetName() == name) {

				if (crossfade) {
					StartCoroutine(s.Crossfade(1.0f, 0.0f, 1.0f, (int a) => {
						s.Stop();
					}));
				}
			}
		}
	}

	public void StopAll() {
		foreach (SoundClip s in soundList) {
			s.Stop();
			//StartCoroutine(s.Crossfade(1.0f, 0.0f, 1.0f));
		}
	}
}
