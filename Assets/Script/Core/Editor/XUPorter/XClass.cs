using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UnityEditor.XCodeEditor
{
	public partial class XClass : System.IDisposable
	{

        private string filePath;

		public XClass(string fPath)
		{
            filePath = fPath;
			if( !System.IO.File.Exists( filePath ) ) {
					Debug.LogError( filePath +"路径下文件不存在" );
					return;
			}
		}


        public void WriteBelow(string below, string text)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();

            int beginIndex = text_all.IndexOf(below);
            if(beginIndex == -1){
                Debug.LogError(filePath +"中没有找到标致"+below);
                return; 
            }

            int endIndex = text_all.LastIndexOf("\n", beginIndex + below.Length);

            text_all = text_all.Substring(0, endIndex) + "\n"+text+"\n" + text_all.Substring(endIndex);

            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(text_all);
            streamWriter.Close();
        }

        public void Replace(string below, string newText)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();

            int beginIndex = text_all.IndexOf(below);
            if(beginIndex == -1){
                Debug.LogError(filePath +"中没有找到标致"+below);
                return; 
            }

            text_all =  text_all.Replace(below,newText);
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(text_all);
            streamWriter.Close();

        }




        public void Dispose()
        {

        }
	}
}