using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System;
using System.IO;
using SimpleJSON;
using Pocketboy.Common;

namespace Pocketboy.GoogleCloud
{
    public class GoogleCloudTTS : MonoBehaviour
    {
        [SerializeField]
        private GoogleCloudTTSConfiguration Configuration;

        private static string RequestAddress = "https://texttospeech.googleapis.com/v1beta1/text:synthesize";

        private bool m_IsAPIKeyValid = false;

        public bool IsAvailable()
        {
            return Configuration != null && !string.IsNullOrEmpty(Configuration.APIKey);
        }

        public void SynthesizeText(string text, AudioSource audioSource, Action<string, bool> audioPlayingDoneCallback)
        {
            StartCoroutine(SynthesizeTextInternal(text, audioSource, audioPlayingDoneCallback));
        }

        private IEnumerator SynthesizeTextInternal(string text, AudioSource audioSource, Action<string, bool> audioPlayingDoneCallback)
        {
            UnityWebRequest request = null;
            yield return StartCoroutine(SendMessage(text, result => request = result));

            if (request.responseCode != 200)
            {
                Debug.Log("Google Cloud TTS Error occured: " + request.downloadHandler.text);
                audioPlayingDoneCallback(text, false);
                yield break;
            }

            var audioClip = GetAudioClip(request.downloadHandler.text);
            audioSource.PlayOneShot(audioClip);
            while (audioSource.isPlaying)
                yield return null;

            audioPlayingDoneCallback(text, true);
        }

        private IEnumerator SendMessage(string text, Action<UnityWebRequest> requestCallback)
        {
            if (Configuration == null || string.IsNullOrEmpty(Configuration.APIKey))
            {
                Debug.Log("API Key is empty!");
                yield break;
            }

            string uri = RequestAddress + "?key=" + Configuration.APIKey;
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            UnityWebRequest www = UnityWebRequest.Post(uri, "");

            byte[] data = System.Text.Encoding.UTF8.GetBytes(GetRequestBody(text));
            UploadHandlerRaw upHandler = new UploadHandlerRaw(data);
            upHandler.contentType = "application/json";
            www.uploadHandler = upHandler;

            yield return www.SendWebRequest();
            requestCallback(www);
        }

        private AudioClip GetAudioClip(string wavString)
        {
            JSONNode jsonAudio = JSON.Parse(wavString);
            string wavContent =(jsonAudio["audioContent"].ToString().Replace("\"", ""));
            return WavUtility.ToAudioClip(Convert.FromBase64String(wavContent));
        }

        private string GetRequestBody(string text)
        {
            return string.Format("{{{0}, {1}, {2} }}", GetSynthesisInput(text), GetVoiceSelectionParams(), GetAudioConfig());
        }

        private string GetSynthesisInput(string text)
        {
            return string.Format("\"input\" : {{\"text\" : \"{0}\"}}", text);
        }

        private string GetVoiceSelectionParams()
        {
            return string.Format("\"voice\": {{ \"languageCode\" : \"{0}\", \"name\" : \"{1}\"}}", Configuration.Language, Configuration.VoiceName);
        }

        private string GetAudioConfig()
        {
            return "\"audioConfig\" : { \"audioEncoding\" : \"LINEAR16\", \"sampleRateHertz\": \"44100\"}";
        }
    }
}


