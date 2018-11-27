using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Pocketboy.GoogleCloud
{
    public enum Language
    {
        /// <summary>
        /// Dutch
        /// </summary>
        nl_Nl,
        /// <summary>
        /// English (Australia)
        /// </summary>
        en_AU,
        /// <summary>
        /// English (Greatbritain)
        /// </summary>
        en_GB,
        /// <summary>
        /// English (USA)
        /// </summary>
        en_US,
        /// <summary>
        /// French
        /// </summary>
        fr_FR,
        /// <summary>
        /// French (Canada)
        /// </summary>
        fr_CA,
        /// <summary>
        /// German
        /// </summary>
        de_DE,
        /// <summary>
        /// Italian
        /// </summary>
        it_IT,
        /// <summary>
        /// Japanese
        /// </summary>
        ja_JP,
        /// <summary>
        /// Korean
        /// </summary>
        ko_KR,
        /// <summary>
        /// Portuguese
        /// </summary>
        pt_BR,
        /// <summary>
        /// Spanish
        /// </summary>
        es_ES,
        /// <summary>
        /// Swedish
        /// </summary>
        sv_SE,
        /// <summary>
        /// Turkish
        /// </summary>
        tr_TR
    }

    public enum VoiceType
    {
        Basic,
        WaveNet
    }

    public enum BasicVoice
    {
        nl_NL_Standard_A,
        en_AU_Standard_A,
        en_AU_Standard_B,
        en_AU_Standard_C,
        en_AU_Standard_D,
        en_GB_Standard_A,
        en_GB_Standard_B,
        en_GB_Standard_C,
        en_GB_Standard_D,
        en_US_Standard_B,
        en_US_Standard_C,
        en_US_Standard_D,
        en_US_Standard_E,
        fr_FR_Standard_A,
        fr_FR_Standard_B,
        fr_FR_Standard_C,
        fr_FR_Standard_D,
        fr_CA_Standard_A,
        fr_CA_Standard_B,
        fr_CA_Standard_C,
        fr_CA_Standard_D,
        /// <summary>
        /// German standard female voice
        /// </summary>
        de_DE_Standard_A,
        /// <summary>
        /// German standard male voice
        /// </summary>
        de_DE_Standard_B,
        it_IT_Standard_A,
        ja_JP_Standard_A,
        ko_KR_Standard_A,
        pt_BR_Standard_A,
        es_ES_Standard_A,
        sv_SE_Standard_A,
        tr_TR_Standard_A
    }

    public enum WaveNetVoice
    {
        nl_NL_Wavenet_A,
        en_AU_Wavenet_A,
        en_AU_Wavenet_B,
        en_AU_Wavenet_C,
        en_AU_Wavenet_D,
        en_GB_Wavenet_A,
        en_GB_Wavenet_B,
        en_GB_Wavenet_C,
        en_GB_Wavenet_D,
        en_US_Wavenet_A,
        en_US_Wavenet_B,
        en_US_Wavenet_C,
        en_US_Wavenet_D,
        en_US_Wavenet_E,
        en_US_Wavenet_F,
        fr_FR_Wavenet_A,
        fr_FR_Wavenet_B,
        fr_FR_Wavenet_C,
        fr_FR_Wavenet_D,
        /// <summary>
        /// German WaveNet female Voice
        /// </summary>
        de_DE_Wavenet_A,
        /// <summary>
        /// German WaveNet male Voice
        /// </summary>
        de_DE_Wavenet_B,
        /// <summary>
        /// German WaveNet female Voice
        de_DE_Wavenet_C,
        /// <summary>
        /// German WaveNet male Voice
        /// </summary>
        de_DE_Wavenet_D,
        it_IT_Wavenet_A,
        ja_JP_Wavenet_A,
        ko_KR_Wavenet_A,
        sv_SE_Wavenet_A,
        tr_TR_Wavenet_A
    }

    [CreateAssetMenu(fileName = "TTSConfiguration", menuName = "PocketBoy/Google/TTSConfiguration")]
    public class GoogleCloudTTSConfiguration : ScriptableObject
    {
        [Header("Private API Key. Make sure NOT TO PUBLISH THIS.")]
        public string APIKey;

        [SerializeField]
        private VoiceType m_VoiceType;

        [SerializeField]
        private Language m_Language;

        [SerializeField]
        private BasicVoice m_BasicVoice;

        [SerializeField]
        private WaveNetVoice m_WaveNetVoice;

        public string VoiceType { get { return m_VoiceType.ToString().Replace("_", "-"); } }

        public string Language { get { return m_Language.ToString().Replace("_", "-"); } }

        public string VoiceName
        { get
            {
                string voiceName = "";
                if (m_VoiceType == GoogleCloud.VoiceType.WaveNet)
                {
                    voiceName = m_WaveNetVoice.ToString().Replace("_", "-");
                }
                else
                {
                    voiceName = m_BasicVoice.ToString().Replace("_", "-");
                }
                return voiceName;
            }
        }

        /// <summary>
        /// TO DO: Return languages for wavenet instead of voice names
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string[] GetLanguagesByType(VoiceType type)
        {
            if (type == GoogleCloud.VoiceType.WaveNet)
                return System.Enum.GetNames(typeof(GoogleCloud.WaveNetVoice)).Where(enumString => enumString.Contains("Wavenet")).ToArray();
            else
            {
                return System.Enum.GetNames(typeof(GoogleCloud.Language));
            }
        }
    }
}
