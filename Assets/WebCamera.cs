using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Runtime.InteropServices;

public class WebCamera : MonoBehaviour
{
    public string deviceName;
    
    //接收返回的图片数据  
    WebCamTexture tex;
    void OnGUI()
    {
       
        if (GUI.Button(new Rect(10, 20, 100, 100), "开启摄像头"))
        {
            // 调用摄像头  
            StartCoroutine(start());
        }
        if (GUI.Button(new Rect(10, 130, 100, 100), "捕获照片"))
        {
            //捕获照片  
            tex.Pause();
            StartCoroutine(getTexture());
        }
        if (GUI.Button(new Rect(10, 240, 100, 100), "再次捕获"))
        {
            //重新开始  
            //tex.Play();
            takePhoto("test.jpg");
        }
        if (GUI.Button(new Rect(120, 20, 80, 100), "录像"))
        {
            //录像  
            StartCoroutine(SeriousPhotoes());
        }
        if (GUI.Button(new Rect(10, 350, 100, 100), "停止"))
        {
            //停止捕获镜头  
            tex.Stop();
            StopAllCoroutines();
        }
        if (tex != null)
        {
            // 捕获截图大小               —距X左屏距离   |   距Y上屏距离    
            //GUI.DrawTexture(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 190, 280, 200), tex);
        }
    }
    /// <summary>  
    /// 捕获窗口位置  
    /// </summary>  
    public IEnumerator start()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            deviceName = devices[1].name;
            tex = new WebCamTexture(deviceName, 640, 480, 12);
            RawImage ri = GetComponent<RawImage>();
            ri.texture = tex;
            tex.Play();
           /* tex.Play();
            Texture2D t = new Texture2D(400, 300);
            t.ReadPixels(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 50, 360, 300), 0, 0, false);
            //距X左的距离        距Y屏上的距离  
            // t.ReadPixels(new Rect(220, 180, 200, 180), 0, 0, false);  
            t.Apply();
            byte[] byt = t.EncodeToPNG();
            File.WriteAllBytes(Application.persistentDataPath + "/Photoes/" + Time.time + ".jpg", byt);
            tex.Stop();
            StopAllCoroutines();*/

        }
    }

    public static byte[] StructToBytes(object structObj, int size)
    {
        byte[] bytes = new byte[size];
        IntPtr structPtr = Marshal.AllocHGlobal(size);
        //将结构体拷到分配好的内存空间
        Marshal.StructureToPtr(structObj, structPtr, false);
        //从内存空间拷贝到byte 数组
        Marshal.Copy(structPtr, bytes, 0, size);
        //释放内存空间
        Marshal.FreeHGlobal(structPtr);
        return bytes;

    }


    public static object ByteToStruct(byte[] bytes, Type type)
    {
        int size = Marshal.SizeOf(type);
        if (size > bytes.Length)
        {
            return null;
        }
        //分配结构体内存空间
        IntPtr structPtr = Marshal.AllocHGlobal(size);
        //将byte数组拷贝到分配好的内存空间
        Marshal.Copy(bytes, 0, structPtr, size);
        //将内存空间转换为目标结构体
        object obj = Marshal.PtrToStructure(structPtr, type);
        //释放内存空间
        Marshal.FreeHGlobal(structPtr);
        return obj;
    }


    /// <summary>  
    /// 获取截图  
    /// </summary>  
    /// <returns>The texture.</returns>  
    public IEnumerator getTexture()
    {
        yield return new WaitForEndOfFrame();
        //Texture2D t = new Texture2D(400, 300);
        //t.ReadPixels(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 50, 360, 300), 0, 0, false);
        RawImage ri = GetComponent<RawImage>();
        WebCamTexture wt = (WebCamTexture)ri.texture;
        
        int width = ri.texture.width;
        int height = ri.texture.height;
        Texture2D t = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // RenderTexture.active = ri.texture;
        Color[] colors = wt.GetPixels();
        byte[] colorbytes = StructToBytes(colors, colors.Length * 4);
        //Array colorarray = new Array();
       /* using (ZipOutputStream s = new ZipOutputStream(File.Create(Application.persistentDataPath + "/Photoes/" + "1.zip")))
        {
            s.SetLevel(5);
            s.Password = "1q2w3e";
            ZipEntry entry = new ZipEntry(Path.GetFileName("block1"));
            entry.DateTime = DateTime.Now;
            s.PutNextEntry(entry);
            s.Write(colorbytes, 0, colorbytes.Length);
            s.Finish();
            s.Close();
        } */
		
        t.SetPixels(wt.GetPixels());

        //距X左的距离        距Y屏上的距离  
        // t.ReadPixels(new Rect(220, 180, 200, 180), 0, 0, false);  
        //t.Apply();
        byte[] byt = t.EncodeToJPG();
        File.WriteAllBytes(Application.persistentDataPath + "/Photoes/" + Time.time + ".jpg", byt);
        tex.Play();
    }
    /// <summary>  
    /// 连续捕获照片  
    /// </summary>  
    /// <returns>The photoes.</returns>  
    public IEnumerator SeriousPhotoes()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            Texture2D t = new Texture2D(400, 300, TextureFormat.RGB24, true);
            t.ReadPixels(new Rect(Screen.width / 2 - 180, Screen.height / 2 - 50, 360, 300), 0, 0, false);
            t.Apply();
            print(t);
            byte[] byt = t.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/MulPhotoes/" + Time.time.ToString().Split('.')[0] + "_" + Time.time.ToString().Split('.')[1] + ".png", byt);
            Thread.Sleep(300);
        }
    }

    private void takePhoto(object photoname)
    {


#if UNITY_ANDROID
        //Init AndroidJavaClass
        AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); ;
        AndroidJavaClass Intent = new AndroidJavaClass("android.content.Intent");
        AndroidJavaClass MediaStore = new AndroidJavaClass("android.provider.MediaStore");
        AndroidJavaClass Uri = new AndroidJavaClass("android.net.Uri");
        AndroidJavaClass Environment = new AndroidJavaClass("android.os.Environment");
        //获取当前Activity
        AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        //相当于Intent intent=new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", MediaStore.GetStatic<AndroidJavaObject>("ACTION_IMAGE_CAPTURE"));

        //获取sd卡路径，相当于String sdPath= Environment.getExternalStorageDirectory().getAbsolutePath();
        AndroidJavaObject sdPath = new AndroidJavaObject("java.lang.String", Environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<AndroidJavaObject>("getAbsolutePath"));
        //将路径转化为java String
        AndroidJavaObject img_path = new AndroidJavaObject("java.lang.String", "/Android/data/" + Application.persistentDataPath + "/files/" + photoname.ToString() + ".jpg");
        //相当于img_path=sdPath+img_path；
        img_path = sdPath.Call<AndroidJavaObject>("concat", img_path);
        //相当于File targetImgFile=new File(img_path);
        AndroidJavaObject targetImgFile = new AndroidJavaObject("java.io.File", img_path);
        //相当于intent.putExtra(MediaStore.EXTRA_OUTPUT,Uri.fromFile(targetImgFile));
        intent.Call<AndroidJavaObject>("putExtra", MediaStore.GetStatic<AndroidJavaObject>("EXTRA_OUTPUT"), Uri.CallStatic<AndroidJavaObject>("fromFile", targetImgFile));
        currentActivity.Call("startActivity", intent);



#endif
}
}
