using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
namespace Assets.HellionCat.SimpleExport
{
    public class SimpleExport_ScoreCSV
    {
        private static List<String> paramName;
        static SimpleExport_ScoreCSV()        {
            paramName = new List<string>();
            paramName.Add("Score");
            paramName.Add("Email");
            
        }
        public static Boolean ExportCSV(Int32 pScore,String pEmail,String pExportPath="./Assets/SimpleExport/Export/ScoreCSV")
        {
            pExportPath=String.Format("{0}.csv",pExportPath);
            if (File.Exists(pExportPath))
            {
            	using (StreamWriter stream = File.AppendText(pExportPath))
            	{	
            		stream.Write(String.Format("\n"));
            		stream.Write(String.Format("{0}{1}",0>0?";":"", pScore));
            		stream.Write(String.Format("{0}{1}",1>0?";":"", pEmail));
            		
            	}
            }
            else
            {
            	String directorPath = new FileInfo(pExportPath).Directory.FullName;
            	if (!Directory.Exists(directorPath))
            	{
            		Directory.CreateDirectory(directorPath);
            	}
            	using (StreamWriter stream = File.CreateText(pExportPath))
            	{
            		stream.Write(String.Format("{0};", paramName[0], pScore));
            		stream.Write(String.Format("{0};", paramName[1], pEmail));
            		
            		stream.Write(String.Format("\n"));
            		stream.Write(String.Format("{0}{1}",0>0?";":"", pScore));
            		stream.Write(String.Format("{0}{1}",1>0?";":"", pEmail));
            				
            	}
            }
            return true;
        }        
    }
}
