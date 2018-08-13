using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;

namespace Pocketboy.QuizSystem
{
    public class QuizCollection : MonoBehaviour
    {
        public Dictionary<int, Image> QuestionPictures { get; private set; }

        private Dictionary<int, QuizQuestion> m_QuestionsByID = new Dictionary<int, QuizQuestion>();

        private string m_QuestionsRelativePath = "/QuizQuestions/";

        private string m_QuestionsAssetBundleName = "quizquestions";

        private string m_QuestionDefaultIdentifier = "Question_";

        private bool m_QuestionLoaded;

        public List<QuizQuestion> GetQuizQuestions()
        {
            return m_QuestionsByID.Values.ToList();
        }

        private void Awake()
        {
            LoadQuestionsFromAssetBundle();
        }

        private void OnDestroy()
        {
            SaveQuestions();
        }

        private bool LoadQuestions()
        {
            #if UNITY_EDITOR
            return LoadQuestionsFromAssetBundle();
            #elif UNITY_ANDROID
            return LoadQuestionsFromPersistentPath();
            #endif
            return false;
        }

        private bool LoadQuestionsFromPersistentPath()
        {
            if (!Directory.Exists(Path.Combine(Application.persistentDataPath, m_QuestionsRelativePath)))
            {
                if (!CopyQuestionsToPersistentPath()) { return false; }
            }
            string[] questionPaths = Directory.GetFiles(Path.Combine(Application.persistentDataPath, m_QuestionsRelativePath), "*.json");
            foreach (var questionPath in questionPaths)
            {
                string questionInTextForm = File.ReadAllText(questionPath);
                var questionObject = JsonUtility.FromJson<QuizQuestion>(questionInTextForm);
                if (questionObject == null)
                    continue;

                m_QuestionsByID.Add(questionObject.ID, questionObject);
            }
            return m_QuestionsByID.Count > 0 ? true : false;
        }

        private bool LoadQuestionsFromAssetBundle()
        {
            var questionsInTextForm = LoadQuestionsAssetBundle();
            if (questionsInTextForm == null)
            {
                return false;
            }
            foreach (var questionText in questionsInTextForm)
            {
                var questionObject = JsonUtility.FromJson<QuizQuestion>(questionText.text);
                if (questionObject != null)
                    m_QuestionsByID.Add(questionObject.ID, questionObject);
            }
            // if the json questions could not be loaded into a QuizQuestion => mismatch of the question format
            if (m_QuestionsByID.Count > 0) return true;
            else
            {
                Debug.LogError("[QuizSystem] JSON questions from AssetBundle could not be transformed into QuizQuestion.");
                return false;
            }
        }

        private TextAsset[] LoadQuestionsAssetBundle()
        {
            var questionsAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, m_QuestionsAssetBundleName));
            if (questionsAssetBundle == null)
            {
                Debug.LogError("[QuizSystem] Failed to load question asset bundle!");
                return null;
            }
            var questionsInTextForm = questionsAssetBundle.LoadAllAssets<TextAsset>();
            if (questionsInTextForm.Length == 0)
            {
                Debug.LogError(string.Format("[QuizSystem] Could not find any questions in the AssetBundle \"{0}\".", m_QuestionsAssetBundleName));
                return null;
            }
            return questionsInTextForm;
        }

        private bool CopyQuestionsToPersistentPath()
        {
            var questionsInTextForm = LoadQuestionsAssetBundle();
            if (questionsInTextForm == null)
            {
                return false;
            }
            var questionsPath = Path.Combine(Application.persistentDataPath, m_QuestionsRelativePath);
            foreach (var questionText in questionsInTextForm)
            {               
                Directory.CreateDirectory(questionsPath);
                var questionObject = JsonUtility.FromJson<QuizQuestion>(questionText.text);
                if (questionObject == null)
                    continue;

                var currentQuestionPath = Path.Combine(questionsPath, m_QuestionDefaultIdentifier + questionObject.ID.ToString());
                File.WriteAllText(currentQuestionPath, questionText.text);
            }
            if (Directory.GetFiles(questionsPath, "*.json").Length != questionsInTextForm.Length)
            {
                Debug.LogErrorFormat("[QuizSystem] Not all question were copied to {0}.", questionsPath);
                return false;
            }            
            return true;
        }

        private void SaveQuestions()
        {
            #if UNITY_EDITOR
            return;
            #endif
            string questionsAbsolutePath = Path.Combine(Application.persistentDataPath, m_QuestionsRelativePath);
            if (string.IsNullOrEmpty(questionsAbsolutePath))
                return;

            foreach (var questionByID in m_QuestionsByID)
            {
                var questionIdentifier = string.Format("{0}{1}.json", m_QuestionDefaultIdentifier, questionByID.Key);
                var questionPath = Path.Combine(questionsAbsolutePath, questionIdentifier);
                File.WriteAllText(questionPath, JsonUtility.ToJson(questionByID.Value));
            }
        }

    }
}