package com.pocketboy.roboy.unityplugin.speech;

import android.content.ActivityNotFoundException;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.speech.RecognitionListener;
import android.speech.RecognizerIntent;
import android.speech.SpeechRecognizer;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import java.util.ArrayList;

public class SpeechToTextPlugin {

    public static String Delimiter = ",";

    private static SpeechRecognizer m_SpeechRecognizer;
    private static String m_Language = "en-US";

    private static boolean m_IsListening;

    public static void setLanguage(String language)
    {
        m_Language = language;
    }

    public static void setDelimiter(String delimiter)
    {
        Delimiter = delimiter;
    }

    public static void stopListening()
    {
        if(m_SpeechRecognizer == null || !m_IsListening)
            return;

        Handler handler = new Handler(Looper.getMainLooper());
        handler.post(new Runnable() {
            @Override
            public void run()
            {
                m_SpeechRecognizer.stopListening();
                m_IsListening = false;
            }}
        );
    }

    public static void startListening(final String unityReceiver, final String callback)
    {
        if(m_IsListening)
            return;

        m_IsListening = true;
        try
        {   // run this on main thread because speech recognizer must be created on main thread
            Handler handler = new Handler(Looper.getMainLooper());
            handler.post(new Runnable() {
                @Override
                public void run() {
                    m_SpeechRecognizer = SpeechRecognizer.createSpeechRecognizer(UnityPlayer.currentActivity);
                    Intent speechRecognizerIntent = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
                    speechRecognizerIntent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, RecognizerIntent.LANGUAGE_MODEL_FREE_FORM);
                    speechRecognizerIntent.putExtra(RecognizerIntent.EXTRA_LANGUAGE, m_Language);
                    m_SpeechRecognizer.setRecognitionListener(new RecognitionListener() {
                        @Override
                        public void onReadyForSpeech(Bundle bundle) {
                            Log.d("STT_Plugin", "Speech ready");
                        }

                        @Override
                        public void onBeginningOfSpeech() {
                            Log.d("STT_Plugin", "Speech began");
                        }

                        @Override
                        public void onRmsChanged(float v) {
                            Log.d("STT_Plugin", "Speech changed");
                        }

                        @Override
                        public void onBufferReceived(byte[] bytes) {
                            Log.d("STT_Plugin", "Speech received");
                        }

                        @Override
                        public void onEndOfSpeech() {
                            Log.d("STT_Plugin", "Speech end");
                        }

                        @Override
                        public void onError(int errorCode) {
                            String errorMessage = getErrorText(errorCode);
                            UnityPlayer.UnitySendMessage(unityReceiver, callback, errorMessage);
                            m_SpeechRecognizer.cancel();
                            m_SpeechRecognizer.destroy();
                            Log.d("STT_Plugin", errorMessage);
                            m_IsListening = false;
                        }

                        @Override
                        public void onResults(Bundle results) {
                            ArrayList<String> matches = results.getStringArrayList(SpeechRecognizer.RESULTS_RECOGNITION);
                            StringBuilder output = new StringBuilder();
                            for(String s : matches)
                            {
                                output.append(s);
                                output.append(Delimiter);
                            }
                            UnityPlayer.UnitySendMessage(unityReceiver, callback, output.toString());
                            m_IsListening = false;
                        }

                        @Override
                        public void onPartialResults(Bundle bundle) {

                        }

                        @Override
                        public void onEvent(int i, Bundle bundle) {

                        }

                        public String getErrorText(int errorCode) {
                            String message;
                            switch (errorCode) {
                                case SpeechRecognizer.ERROR_AUDIO:
                                    message = "Audio recording error";
                                    break;
                                case SpeechRecognizer.ERROR_CLIENT:
                                    message = "Client side error";
                                    break;
                                case SpeechRecognizer.ERROR_INSUFFICIENT_PERMISSIONS:
                                    message = "Insufficient permissions";
                                    break;
                                case SpeechRecognizer.ERROR_NETWORK:
                                    message = "Network error";
                                    break;
                                case SpeechRecognizer.ERROR_NETWORK_TIMEOUT:
                                    message = "Network timeout";
                                    break;
                                case SpeechRecognizer.ERROR_NO_MATCH:
                                    message = "No match";
                                    break;
                                case SpeechRecognizer.ERROR_RECOGNIZER_BUSY:
                                    message = "RecognitionService busy";
                                    break;
                                case SpeechRecognizer.ERROR_SERVER:
                                    message = "error from server";
                                    break;
                                case SpeechRecognizer.ERROR_SPEECH_TIMEOUT:
                                    message = "No speech input";
                                    break;
                                default:
                                    message = "Didn't understand, please try again.";
                                    break;
                            }
                            return message;
                        }
                    });
                    m_SpeechRecognizer.startListening(speechRecognizerIntent);

                }
            });
        }
        catch(ActivityNotFoundException e)
        {
            String appPackageName = "com.google.android.googlequicksearchbox";
            try
            {
                UnityPlayer.currentActivity.startActivity(new Intent(Intent.ACTION_VIEW, Uri.parse("market://details?id=" + appPackageName)));
            }
            catch (android.content.ActivityNotFoundException anfe)
            {
                UnityPlayer.currentActivity.startActivity(new Intent(Intent.ACTION_VIEW, Uri.parse("https://play.google.com/store/apps/details?id=" + appPackageName)));
            }
        }
    }
}
