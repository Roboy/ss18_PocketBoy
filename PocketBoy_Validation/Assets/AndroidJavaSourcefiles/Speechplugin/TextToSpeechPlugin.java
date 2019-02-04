package com.pocketboy.roboy.unityplugin.speech;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

import android.os.Bundle;
import android.speech.tts.TextToSpeech;
import android.speech.tts.UtteranceProgressListener;

import java.util.Locale;

public class TextToSpeechPlugin {

    private static TextToSpeech m_TTS;
    private static float m_Speed = 1;
    private static float m_Pitch = 1;

    public static void setPitch(float pitch)
    {
        m_Pitch = pitch;
    }

    public static void setSpeed(float speed)
    {
        m_Speed = speed;
    }

    public static void promptSpeechOutput(String text)
    {
        final String textToSpeak = text;
        m_TTS = new TextToSpeech(UnityPlayer.currentActivity, new TextToSpeech.OnInitListener()
        {
            @Override
            public void onInit(int status) {
                if(status == TextToSpeech.SUCCESS)
                {
                    int languageSetSuccess = m_TTS.setLanguage(Locale.GERMAN);
                    if(languageSetSuccess == TextToSpeech.LANG_MISSING_DATA || languageSetSuccess == TextToSpeech.LANG_NOT_SUPPORTED)
                    {
                        return;
                    }
                    m_TTS.setSpeechRate(m_Speed);
                    m_TTS.setPitch(m_Pitch);
                    m_TTS.speak(textToSpeak, TextToSpeech.QUEUE_FLUSH, null, null);
                }
            }
        });
    }

    public static void promptSpeechOutputWithCallback(final String text, final String unityReceiver, final String callback)
    {
        m_TTS = new TextToSpeech(UnityPlayer.currentActivity, new TextToSpeech.OnInitListener()
        {
            @Override
            public void onInit(int status) {
                if(status == TextToSpeech.SUCCESS)
                {
                    int languageSetSuccess = m_TTS.setLanguage(Locale.GERMAN);
                    if(languageSetSuccess == TextToSpeech.LANG_MISSING_DATA || languageSetSuccess == TextToSpeech.LANG_NOT_SUPPORTED)
                    {
                        return;
                    }

                    Bundle params = new Bundle();
                    params.putString(TextToSpeech.Engine.KEY_PARAM_UTTERANCE_ID, "");

                    m_TTS.setSpeechRate(m_Speed);
                    m_TTS.setPitch(m_Pitch);
                    m_TTS.speak(text, TextToSpeech.QUEUE_FLUSH, params, "UniqueID");
                    m_TTS.setOnUtteranceProgressListener(new UtteranceProgressListener() {
                        @Override
                        public void onStart(String utteranceId) {

                        }

                        @Override
                        public void onDone(String utteranceId) {
                            UnityPlayer.UnitySendMessage(unityReceiver, callback, "Success");
                        }

                        @Override
                        public void onError(String utteranceId) {

                        }
                    });
                }
            }
        });
    }
	
	public static void stopSpeech()
    {
        if(m_TTS == null)
            return;
        
        m_TTS.stop();
    }
}
