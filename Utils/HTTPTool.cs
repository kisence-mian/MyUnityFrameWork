using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System;
using System.Net;
using System.IO;

public class HTTPTool
{
    private static readonly Encoding DEFAULTENCODE = Encoding.UTF8;
    public static string Upload_Request(string url, string file)
    {
        NameValueCollection data = new NameValueCollection();

        return Upload_Request(url, new string[] { file }, data, DEFAULTENCODE);
    }

    public static string Upload_Request(string url, string[] files, NameValueCollection data, Encoding encoding)
    {
        string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
        byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
        byte[] endbytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

        //1.HttpWebRequest
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.ContentType = "multipart/form-data; boundary=" + boundary;
        request.Method = "POST";
        request.KeepAlive = true;
        request.Credentials = CredentialCache.DefaultCredentials;
        request.Expect = null;

        using (Stream stream = request.GetRequestStream())
        {
            //1.1 key/value
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            if (data != null)
            {
                foreach (string key in data.Keys)
                {
                    stream.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, data[key]);
                    byte[] formitembytes = encoding.GetBytes(formitem);
                    stream.Write(formitembytes, 0, formitembytes.Length);
                }
            }

            //1.2 file
            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: text/plain\r\n\r\n";
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            for (int i = 0; i < files.Length; i++)
            {
                stream.Write(boundarybytes, 0, boundarybytes.Length);
                string header = string.Format(headerTemplate, "file", Path.GetFileName(files[i]));
                byte[] headerbytes = encoding.GetBytes(header);
                stream.Write(headerbytes, 0, headerbytes.Length);
                using (FileStream fileStream = new FileStream(files[i], FileMode.Open, FileAccess.Read))
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
            return stream.ReadToEnd();
        }
    }
}
