File Chooser Dialog Box
=======================
This is a file picker dialog box made using the new GUI in Unity. It is designed to be a quick add in to your app to allow a user to select files. Or to use for rapid prototyping.

About 
=====
You can set up the dialog box with a list of extensions for example "wav|ogg|mp3" and it will filter the results accordingly.

You can also type in wildcard searches into the text box. For example you can search for all files beginning with the letter 'a' by typing in:

                             a*

You can also use the question mark '?' to fill in an unknown letter for example:

                            ?at.png

will match cat.png, bat.png and rat.png. For all files PNG files you can type in:

                             *.png
                             
You can load a file by clicking on that file and pressing enter or by typing in the filename and pressing enter. You can also choose a folder by typing in the name of the folder and pressing enter.

How to Use
==========
Create a new scene or use an existing one. Create a Canvas in your scene.
Then simply drop the FileChooser prefab onto the Canvas in the Hierachy.
You should set the Filechooser to inactive so it is invisble before it is called.

Opening the File Chooser
========================
Create a public variable in your main scene class:

	public FileChooser fileChooser;

Then you can drag the filechooser from your scene into this slot.
To open the file chooser to open files use:

	fileChooser.setup (FileChooser.OPENSAVE.OPEN, "txt");

To open the file chooser to save files use:

	fileChooser.setup (FileChooser.OPENSAVE.SAVE, "txt");

You can change "txt" to a list of other extensions for example "wav|mp3". Or to show all files use null here.

Setting the callback functions
==============================
To know what to do when the user selects a file or presses cancel you need to set the callback functions.
For when the user selects a file or clicks OK:

	fileChooser.callbackYes = delegate(string filename){
		//first hide the filechooser
		fileChooser.gameObject.setActive(false);
		//now do something with the filename such as read a file
		//**your code here***
	}

When the user clicks cancel set:

	fileChooser.callbackNo = delegate(){
		//first hide the filechooser
		fileChooser.gameObject.setActive(false);
		//now do something with the information that the user has cancelled. Perhaps do nothing.
		//**your code here***
	}


Starting Directory
==================
The starting directory is set to be the documents folder. This is the one you probably want to keep, especially on platforms where you are only allowed to save here.
You can change the starting directory by modifying the code in the function SetStartingDirectory in FileChooser.cs

File Dialog Size
================
If you want to change the size of the file dialog. Go to the FileDialog panel element in the canvas. Choose the rect transform tool and resize it. The contents will resize accordingly.

Platforms
=========
The filechooser works on PC, Mac, Android and IOS. (But not Windows 8 Store as this doesn't allow access to the file system).
Note: IOS only allows you to load and save files to certain folders such as the documents folder.




