using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System;
using System.Net;
using System.IO;
using System.Threading;

public class HTTPTool
{
    private static readonly Encoding DEFAULTENCODE = Encoding.UTF8;
    public static void Upload_Request_Thread(string url, string file)
    {
        NameValueCollection data = new NameValueCollection();

        s_url = url;
        s_files = new string[] { file };
        s_data = data;
        s_encoding = DEFAULTENCODE;

        Thread th = new Thread(Upload_Request);
        th.Start();
    }

    static string s_url;
    static string[] s_files;
    static NameValueCollection s_data;
    static Encoding s_encoding;

    public static void Upload_Request()
    {
        Debug.Log("Upload_Request " + s_url);

        try
        {



            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endbytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            //1.HttpWebRequest
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(s_url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Expect = null;

            using (Stream stream = request.GetRequestStream())
            {
                //1.1 key/value
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                if (s_data != null)
                {
                    foreach (string key in s_data.Keys)
                    {
                        stream.Write(boundarybytes, 0, boundarybytes.Length);
                        string formitem = string.Format(formdataTemplate, key, s_data[key]);
                        byte[] formitembytes = s_encoding.GetBytes(formitem);
                        stream.Write(formitembytes, 0, formitembytes.Length);
                    }
                }

                //1.2 file
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: text/plain\r\n\r\n";
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                for (int i = 0; i < s_files.Length; i++)
                {
                    stream.Write(boundarybytes, 0, boundarybytes.Length);
                    string header = string.Format(headerTemplate, "file", Path.GetFileName(s_files[i]));
                    byte[] headerbytes = s_encoding.GetBytes(header);
                    stream.Write(headerbytes, 0, headerbytes.Length);
                    using (FileStream fileStream = new FileStream(s_files[i], FileMode.Open, FileAccess.Read))
                    {
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            stream.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                //1.3 form end
                stream.Write(endbytes, 0, endbytes.Length);
            }
            //2.WebResponse
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                string result = stream.ReadToEnd();
                if(result == "ok")
                {
                    Debug.Log("上传完成 " + result);
                }
                else
                {
                    Debug.Log("上传结束 " + result);
                }

                return;
                //return stream.ReadToEnd();
            }
        }
        catch(Exception e)
        {
            Debug.Log("上传失败 " + e.ToString());
        }
    }
}
