using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;
using Pocketboy.Common;

namespace Pocketboy.HistoryScene
{
    /// <summary>
    /// Displays images/videos on a plane. Has an interface to change the content.
    /// </summary>
    public class TVController : MonoBehaviour
    {

        /// <summary>
        /// Container class for an image or a video as tv content. Is used so you can put both types into the ContentList.
        /// </summary>
        private class TVObject
        {
            public bool IsVideo { get; private set; }
            public Texture2D Image { get; private set; }
            public VideoClip Video { get; private set; }

            public TVObject(Texture2D image)
            {
                Image = image;
                IsVideo = false;
            }

            public TVObject(VideoClip video)
            {
                Video = video;
                IsVideo = true;
            }
        }

        [SerializeField]
        private Material DefaultMaterial;

        [SerializeField, Tooltip("Material which is displayed when changing the content.")]
        private Material StaticMaterial;

        [SerializeField, Range(0f, 1f), Tooltip("Duration of the static image effect.")]
        private float StaticImageTime = 0.5f;


        [SerializeField, Tooltip("Renderer component of the plane on which the content is displayed on")]
        private Renderer MonitorRenderer;

        [SerializeField, Tooltip("Can either be of type VideoClip or Texture2D")]
        private UnityEngine.Object[] Content;

        [SerializeField, Tooltip("Is used to render a video.")]
        private RenderTexture VideoTexture;

        [SerializeField, Tooltip("Can be used to enable content switchting via arrow keys on a desktop and via touch on mobile.")]
        private bool m_DebugMode;

        /// <summary>
        /// Content filtered to by only of type VideoClip or Texture2D.
        /// </summary>
        private List<TVObject> m_ContentList = new List<TVObject>();

        /// <summary>
        /// Index of the current content.
        /// </summary>
        private int m_CurrentContent;

        /// <summary>
        /// Used to play videos.
        /// </summary>
        private VideoPlayer m_VideoPlayer;

        /// <summary>
        /// Helper variable to stop multiple coroutine from running.
        /// </summary>
        private bool m_Changing;

        private void Awake()
        {
            FillContentList();
        }

        void Update()
        {
            if (!m_DebugMode)
                return;

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.RightArrow)) { ShowNextContent(); }
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) { ShowPreviousContent(); }
#elif UNITY_ANDROID
            if (Input.touches.Length > 0 && Input.GetTouch(0).phase == TouchPhase.Began )
            {
                var pos = Input.GetTouch(0).position;
                if (pos.x > Screen.width / 2)
                {
                    ShowNextContent();
                }
                else
                {
                    ShowPreviousContent();
                }
            }
#endif
        }

        private void OnDestroy()
        {
            // we need to reset the material because changes of materials in play mode are saved
            DefaultMaterial.mainTexture = null;
        }

        public void ShowNextContent()
        {
            ShowContent(m_CurrentContent + 1);
        }

        public void ShowPreviousContent()
        {
            ShowContent(m_CurrentContent - 1);
        }

        public void RepeatContent()
        {
            if (m_ContentList[m_CurrentContent].IsVideo)
            {
                ShowContent(m_CurrentContent, false);
                Debug.Log("R");
            }            
        }

        public void ShowContent(int index, bool showStaticImage = true)
        {
            if (m_ContentList.Count == 0)
                return;

            m_CurrentContent = MathUtility.WrapArrayIndex(index, m_ContentList.Count);
            StartCoroutine(UpdateContent(showStaticImage));
        }

        private IEnumerator UpdateContent(bool showStaticImage)
        {
            while (m_Changing)
                yield return null;

            m_Changing = true;

            if (showStaticImage)
            {
                MonitorRenderer.material = StaticMaterial;
                yield return new WaitForSeconds(StaticImageTime);
            }
            
            if (!m_ContentList[m_CurrentContent].IsVideo)
            {
                DefaultMaterial.mainTexture = m_ContentList[m_CurrentContent].Image;
                m_VideoPlayer.clip = null;
            }
            else
            {
                DefaultMaterial.mainTexture = VideoTexture;
                m_VideoPlayer.clip = m_ContentList[m_CurrentContent].Video;
                m_VideoPlayer.Stop();
                m_VideoPlayer.Play();
            }
            MonitorRenderer.material = DefaultMaterial;
            m_Changing = false;
        }

        /// <summary>
        /// Fills the content list with objects of the serialized content array which are either of type VideoClip or Texture2D.
        /// </summary>
        private void FillContentList()
        {
            bool videoIncluded = false;
            for (int i = 0; i < Content.Length; i++)
            {
                if (Content[i] == null)
                    continue;

                try
                {
                    var image = (Texture2D)Content[i];
                    if (image != null)
                        m_ContentList.Add(new TVObject(image));
                }
                catch (InvalidCastException e)
                {
                    var video = (VideoClip)Content[i];
                    if (video != null)
                    {
                        m_ContentList.Add(new TVObject(video));
                        videoIncluded = true;
                    }
                }
            }
            if (!videoIncluded)
                return;

            m_VideoPlayer = gameObject.AddComponent<VideoPlayer>();
            m_VideoPlayer.playOnAwake = false;
            m_VideoPlayer.aspectRatio = VideoAspectRatio.FitInside;
            m_VideoPlayer.isLooping = false;
            m_VideoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            m_VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
            m_VideoPlayer.targetTexture = VideoTexture;
        }
    }
}