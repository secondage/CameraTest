/*
 *        This code should work on Windows Standalone/ Mac / Android /IOS
 * 		  but not WebPlayer or Windows 8 as these don't have direct access to the file system
 * 
 */


using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEngine.UI;


public class FileChooserDemo : MonoBehaviour {
	 
	public FileChooser fileChooser;
	public Text textfield;
	public Image image; 

	AudioSource source=null;

	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
	}

	void Update () {
		if(source==null || source.clip==null) return;
		if(!source.isPlaying && source.clip.isReadyToPlay)
			source.Play();	
	}

	public void LoadTextButtonClicked(){
		fileChooser.setup (FileChooser.OPENSAVE.OPEN, "txt");
		fileChooser.callbackYes = delegate(string filename, string fullname) {
			fileChooser.gameObject.SetActive (false);
			if (File.Exists(filename)) {
				//load text file
				try
				{
					using (StreamReader sr = new StreamReader(fullname))
					{
						string text = sr.ReadToEnd();
						textfield.text = text;
					}
				}
				catch (Exception e)
				{
					Debug.Log("The file could not be read: " + e.Message);
				}
			}else{
				Debug.Log("The file does not exist: "+filename);
			}

		};
		fileChooser.callbackNo = delegate() {
			fileChooser.gameObject.SetActive (false);
		};
	}

	public void SaveTextButtonClicked(){
		fileChooser.setup (FileChooser.OPENSAVE.SAVE, "txt");
		fileChooser.callbackYes = delegate(string filename, string fullname) {
			fileChooser.gameObject.SetActive (false);

			if (File.Exists(fullname)) {
				//We may choose not to overwrite an existing file or display a warning:
				Debug.Log ("Don't want to overwrite existing file: "+filename);
			}else{
				//save file
				try
				{
					using (StreamWriter sw = new StreamWriter(filename))
					{
						sw.Write ( textfield.text );
					}
				}
				catch (Exception e)
				{
					Debug.Log("The file could not be written to:" + e.Message);
				}			
			}
		};
		fileChooser.callbackNo = delegate() {
			fileChooser.gameObject.SetActive (false);
		};
	}

	public void LoadImageButtonClicked(){
		fileChooser.setup (FileChooser.OPENSAVE.OPEN, "png");
		fileChooser.callbackYes = delegate(string filename, string fullname) {
			fileChooser.gameObject.SetActive (false);
			Texture2D texture = null;
			byte[] bytes;		
			if (File.Exists(filename)) {
				bytes = File.ReadAllBytes(fullname);
				texture = new Texture2D(2, 2);
				//texture.filterMode = FilterMode.Point;
				texture.LoadImage(bytes);

				//set on image
				image.sprite = Sprite.Create (texture,new Rect(0,0,texture.width,texture.height),Vector2.zero);
			}else{
				Debug.Log("The file does not exist: "+filename);
			}
		};
		fileChooser.callbackNo = delegate() {
			fileChooser.gameObject.SetActive (false);
		};
	}

	//

	public void LoadAudioButtonClicked(){
		fileChooser.setup (FileChooser.OPENSAVE.OPEN, "wav|mp3|ogg");
		fileChooser.callbackYes = delegate(string filename, string fullname) {
			fileChooser.gameObject.SetActive (false);
			//do something with the audio here
			if (File.Exists(filename)) {
				try{
					WWW www = new WWW("file://"+ fullname);
					source.clip = www.GetAudioClip();
				}catch(Exception e){
					Debug.Log("Could not stream audio: " + e.Message);
				}
			}else{
				Debug.Log("The file does not exist: "+filename);
			}
		};
		fileChooser.callbackNo = delegate() {
			fileChooser.gameObject.SetActive (false);
		};
	}


}
