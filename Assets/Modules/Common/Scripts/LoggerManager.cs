using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using Pocketboy.Common;

using System.Net.Http;

namespace Pocketboy.Logging
{
    public class LoggerManager : Singleton<LoggerManager>
    {
        

        private Stopwatch m_Stopwatch = new Stopwatch();

        private SceneLogger m_SceneLogger;

        private int m_ID = 1;

        private static string m_IDPlayerPrefsName = "StudyID";

        //private static readonly TelegramBotClient m_TelegramBot = new TelegramBotClient("645049188:AAH0yAR2hXFKfdXFbW7Iw6cnqJaYhpInHzY");


        private void Awake()
        {
            
            LoadID();
            SceneManager.sceneUnloaded += (scene) => SaveSceneStats(scene);
            SceneManager.sceneLoaded += (scene, loadMode) => ResetSceneStats();
        }

        private void OnDestroy()
        {
            SaveSceneStats(SceneManager.GetActiveScene());
        }

        public void RegisterLogger(SceneLogger logger)
        {
            m_SceneLogger = logger;
        }

        private void ResetSceneStats()
        {
            m_Stopwatch.Start();
        }

        private void SaveSceneStats(Scene scene)
        {
            m_Stopwatch.Stop();
            var time = m_Stopwatch.ElapsedMilliseconds;
            var stats = m_SceneLogger.GetStats();

            string output = string.Format("{{ID : {0}, Scene : {1},  Time : {2} seconds, Data : {3}}}", m_ID, scene.name, time / 1000f, stats);
            SaveSceneStatsToFile(scene.name, output);
        }

        private void SaveSceneStatsToFile(string scene, string stats)
        {
            string filePath = string.Format("{0}/{1}_{2}.json", Application.streamingAssetsPath, m_ID, scene);
            StreamWriter streamWriter = new StreamWriter(filePath, true);
            streamWriter.WriteLine(stats);
            streamWriter.Close();
        }

        private void LoadID()
        {
            if (PlayerPrefs.GetInt(m_IDPlayerPrefsName) == default(int))
                return;

            m_ID = PlayerPrefs.GetInt(m_IDPlayerPrefsName) + 1;
            PlayerPrefs.SetInt(m_IDPlayerPrefsName, m_ID);
        }

        private void Start()
        {
            //MailMessage mail = new MailMessage("egor_sh@live.de", "egor_sh@live.de");
            //SmtpClient client = new SmtpClient();
            //client.Port = 25;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;
            //client.Host = "smtp.outlook.com";
            //client.Credentials = new System.Net.NetworkCredential("egor_sh@live.de", "robotik1!");
            //mail.Subject = "this is a test email.";
            //mail.Body = "this is my test email body";
            //client.Send(mail);
            SendFileTelegram(Application.dataPath + "/"+ "AudioSourcesManager.cs");
        }

        public void SendMessageTelegram(string message)
        {


            string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";

            string apiToken = "645049188:AAH0yAR2hXFKfdXFbW7Iw6cnqJaYhpInHzY";
            string chatId = "@PocketBoy_Study";
            string text = message;
            urlString = String.Format(urlString, apiToken, chatId, text);
            WebRequest request = WebRequest.Create(urlString);
            Stream rs = request.GetResponse().GetResponseStream();

            StreamReader reader = new StreamReader(rs);

            string line = "";

            StringBuilder sb = new StringBuilder();
            while (line != null)
            {
                line = reader.ReadLine();
                if (line != null)
                    sb.Append(line);
            }

            string response = sb.ToString();
            print(response);
            
        }

        public void UploadMultipart(byte[] file, string filename, string contentType, string url)
        {
            var webClient = new WebClient();
            string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
            webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
            var fileData = webClient.Encoding.GetString(file);
            var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, filename, contentType, fileData);

            var nfile = webClient.Encoding.GetBytes(package);

            byte[] resp = webClient.UploadData(url, "POST", nfile);
        }

        async void Test(string url)
        {
            using (var client = new HttpClient())
            {
                var uri = new Uri(url);

                using (var multipartFormDataContent = new MultipartFormDataContent())
                {
                    var streamContent = new StreamContent(new MemoryStream(new byte[1024]));
                    streamContent.Headers.Add("Content-Type", "application/octet-stream");
                    streamContent.Headers.Add("Content-Disposition", "form-data; name=\"text\"; filename=\"Sound-1.json\"");
                    multipartFormDataContent.Add(streamContent, "file", "Sound-1.json");

                    using (var message = await client.PostAsync(uri, multipartFormDataContent))
                    {
                        var contentString = await message.Content.ReadAsStringAsync();
                    }
                }
            }
        }

        public void SendFileTelegram(string message)
        {
            

            string urlString = "https://api.telegram.org/bot{0}/sendDocument?chat_id={1}";

            string apiToken = "645049188:AAH0yAR2hXFKfdXFbW7Iw6cnqJaYhpInHzY";
            string chatId = "@PocketBoy_Study";
            string text = "@test";
            urlString = String.Format(urlString, apiToken, chatId, text);
            //Test(urlString);
            UnityEngine.Debug.Log("Sending");
            UploadMultipart(new byte[1024], "aaa", "application/octet-stream", urlString);
            return;

            WebRequest request = WebRequest.Create(urlString);
            request.ContentType = "multipart/form-data";
            Stream rs = request.GetResponse().GetResponseStream();

            //WebClient client = new WebClient();

            //var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, filename, contentType, fileData);

            //client.

            //client.UploadFile(urlString, Application.dataPath + "/AudioSourcesManager.cs");

            //Upload(urlString, Application.dataPath + "/AudioSourcesManager.cs");
        }

        public void Upload(string uri, string filePath)
        {
            string formdataTemplate = "Content-Disposition: form-data; filename=\"{0}\";\r\nContent-Type: application/octet-stream";
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ServicePoint.Expect100Continue = false;
            request.Method = "POST";
            request.ContentType = "multipart/form-data";
            //request.ContentLength = byteArray.Length;


            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, Path.GetFileName(filePath));
                    byte[] formbytes = Encoding.UTF8.GetBytes(formitem);
                    requestStream.Write(formbytes, 0, formbytes.Length);
                    byte[] buffer = new byte[1024 * 4];
                    int bytesLeft = 0;

                    while ((bytesLeft = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        requestStream.Write(buffer, 0, bytesLeft);
                    }

                }
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) { }

                Console.WriteLine("Success");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void SaveTime(float time) { }
    }
}


