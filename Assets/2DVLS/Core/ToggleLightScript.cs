using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


[RequireComponent(typeof(AudioSource))]
public class ToggleLightScript : MonoBehaviour {
	public Light2D l2d;
	string absolutePath = "./Assets/2DVLS/Samples/Sounds";
	List<AudioClip> clips = new List<AudioClip>();
	public AudioSource flashlightOn;
	public AudioSource flashlightOff;
	//compatible file extensions
	string[] fileTypes = {"ogg","wav"};
	
	FileInfo[] files;
	// Use this for initialization
	void Start () {
		if(flashlightOn == null) flashlightOn = gameObject.AddComponent<AudioSource>();
		if(flashlightOff == null) flashlightOff = gameObject.AddComponent<AudioSource>();
		reloadSounds();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyUp (KeyCode.Space)) {
			switch (l2d.LightEnabled)
			{
			case true:
				flashlightOn.clip = clips[0];
				flashlightOn.Play();
				break;
			default:
				flashlightOff.clip = clips[1];
				flashlightOff.Play();
				break;

			}
			audio.Play();
			l2d.ToggleLight();
		}
	}

	void reloadSounds() {
		DirectoryInfo info = new DirectoryInfo(absolutePath);
		files = info.GetFiles();
		
		//check if the file is valid and load it
		foreach(FileInfo f in files) {
			if(validFileType(f.FullName)) {
				//Debug.Log("Start loading "+f.FullName);
				StartCoroutine(loadFile(f.FullName));
			}
		}
	}
	
	bool validFileType(string filename) {
		foreach(string ext in fileTypes) {
			if(filename.IndexOf(ext) > -1) return true;
		}
		return false;
	}
	
	IEnumerator loadFile(string path) {
		WWW www = new WWW("file://"+path);
		
		AudioClip myAudioClip = www.audioClip;
		while (!myAudioClip.isReadyToPlay)
			yield return www;
		
		AudioClip clip = www.GetAudioClip(false);
		string[] parts = path.Split('\\');
		clip.name = parts[parts.Length - 1];
		clips.Add(clip);
	}
}
