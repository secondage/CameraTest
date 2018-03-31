using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;


//perhaps also store fileInput.text locally?

public class FileChooser : MonoBehaviour {

	public GameObject titleObject;

	public GameObject button;

	public GameObject fileListPanel;

	public InputField fileInput;
	public Text placeholder;
    public Text TextTopic;

	public Button openSaveButton;

#if	UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
	public static string SLASH="\\";
#else
	public static string SLASH="/";
#endif	

	void Start () {
		if(dir==""){
			SetStartingDirectory();
			showFiles ();	
		}
	}

	void SetStartingDirectory(){
		//Sets the directory to the current directory
		//dir = Directory.GetCurrentDirectory();

		//Sets the directory to the documents folder 
		dir =  Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments); 
	}


	public enum OPENSAVE { OPEN, SAVE };

	OPENSAVE openSave = OPENSAVE.OPEN;

	public void setup(OPENSAVE os, string ext){
		extensions.Clear();

		if(ext!=null && ext!="") {
			string[] exts = ext.Split ( '|' );
			for(int i=0;i<exts.Length;i++){
				extensions.Add("."+exts[i]);
			}
		}
		openSave = os;

		//set text on buttons
		if(openSave==OPENSAVE.SAVE)
			openSaveButton.transform.Find ("Text").GetComponent<Text>().text = "保存";
		if(openSave==OPENSAVE.OPEN)
			openSaveButton.transform.Find ("Text").GetComponent<Text>().text = "打开";


		placeholder.text = "";

		if(extensions.Count>0){
			string pattern="";
			for(int i=0;i<extensions.Count;i++){
				pattern+="*"+extensions[i];
				if(i<extensions.Count-1) pattern+=";";
			}
			placeholder.text = pattern;
			filter = WildCardToRegex(pattern);
		}

		Show (true);
	}

	public void Show(bool show){
		fileInput.text = "";
		if(dir=="") SetStartingDirectory();
		showFiles ();
		gameObject.SetActive(true);
	}

	string dir="";

	//list of extensions that we are looking for
	List<string> extensions = new List<string>();
	

	//convert a Wild Card pattern e.g. a??b.* to a RegEx expression:
	public static string WildCardToRegex(string pattern){
		return "^" + Regex.Escape(pattern).
			Replace (";" , "$|^").
			Replace("\\*", ".*").
			Replace("\\?", ".") + "$";
	}
	
	string filter=""; 
	string lastfilter="";

	public void EndEdit(){
		checkWildCard ();

		if( fileInput.text.Length>0 && ( fileInput.text[ fileInput.text.Length-1]=='/' 
		                                || fileInput.text[ fileInput.text.Length-1 ] =='\\' )){
			//remove last character
			fileInput.text = fileInput.text.Substring(0,fileInput.text.Length-1);
		}

		if(fileInput.text!="" && Directory.Exists ( dir+SLASH+fileInput.text ) ){
			dir = dir+SLASH+fileInput.text;
			fileInput.text="";
			showFiles();
			return;
		}
		if(filter==""){
			Yes ();
		}
		if(filter!=lastfilter) showFiles ();
		lastfilter=filter;
	}

	void checkWildCard(){
		if( fileInput.text.Contains ("?") || fileInput.text.Contains("*") ){
			filter = WildCardToRegex (fileInput.text);
		}else filter="";
	}

	void showFiles(){

		//remove all buttons 
		foreach(Transform T in fileListPanel.transform){
			Destroy(T.gameObject);
		}

		setTitle (dir+SLASH);
		string[] dirs = Directory.GetDirectories(dir);
		for(int i=0;i<dirs.Length;i++){
			GameObject newButton=(GameObject)Instantiate(button);
			newButton.transform.Find ("Text").GetComponent<Text>().text = 
				Path.GetFileName(dirs[i])+SLASH;
			newButton.transform.SetParent( fileListPanel.transform ); 
			newButton.transform.localScale = Vector3.one;


			int j=i;
			Button.ButtonClickedEvent BCE=new Button.ButtonClickedEvent();
			BCE.AddListener(  ()=>{ 
				dir = dirs[j];
				showFiles ();
			}  );
			newButton.GetComponent<Button>().onClick = BCE;
	
			
		}

		//Debug.Log ("Matching "+filter);


		string[] files = Directory.GetFiles (dir);	
		for(int i=0;i<files.Length;i++){

			string fileName = Path.GetFileName(files[i]);
            if (filter == "")
                continue;
			//skip if doesn't match pattern:
			if(filter!="" && !Regex.IsMatch (fileName,filter,RegexOptions.IgnoreCase)) continue;

			GameObject newButton=(GameObject)Instantiate(button);
			newButton.transform.Find ("Text").GetComponent<Text>().text = fileName;
			newButton.transform.SetParent ( fileListPanel.transform );
			newButton.transform.localScale = Vector3.one;

			Button.ButtonClickedEvent BCE=new Button.ButtonClickedEvent();
			BCE.AddListener(  ()=>{ 
				fileInput.text = fileName; 
				//Yes ();

			}  );
			newButton.GetComponent<Button>().onClick = BCE;

		}

	}

	public void upDirectory(){
		if(Directory.GetParent(dir)!=null){
			dir = Directory.GetParent(dir).FullName;
			showFiles ();
		}
	}

	public void setTitle(string text){
		titleObject.GetComponent<Text>().text = text;
	}

	//for load, loop through extensions. Don't return if file doesn't exist.
	//test on Mac

	public void Yes(){
		string fullFilename = dir+SLASH+fileInput.text;
		//add the first extension in the list if none specified
		if(openSave == OPENSAVE.SAVE){
			if(!fileInput.text.Contains (".") && extensions.Count>0){
				fullFilename+=extensions[0];
			}
		}else{
			//if not extension provided try all the extensions
			if(!fileInput.text.Contains (".") && extensions.Count>0){
                /*for(int i=0;i<extensions.Count;i++){
					string testFilename = fullFilename+extensions[i];
					if(File.Exists( testFilename)){
						fullFilename = testFilename;
						break;
					}
				}*/
                Debug.Log("Can't return. Directory or File does not exist: " + fullFilename);
                return;
            }
			//if file still not exist don't return
			if( !File.Exists( fullFilename) ) {
                if (!Directory.Exists(fullFilename))
                {
                    Debug.Log("Can't return. Directory or File does not exist: " + fullFilename);
                    return;
                }
           }
		}

		if(callbackYes!=null)
			callbackYes(fileInput.text, fullFilename);
	}
	
	public void No(){
		if(callbackNo!=null)
			callbackNo();
	}
	
	public delegate void Del1();
	public delegate void Del2(string s, string f);
	
	public Del2 callbackYes;
	public Del1 callbackNo;

}
