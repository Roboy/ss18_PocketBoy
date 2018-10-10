using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;
using TMPro;

namespace Pocketboy.Wordcloud
{

    public class WordCloud : MonoBehaviour
    {

        public Word WordPrefab;
        public WordCloudContent Content;
        /// <summary>
        /// Colour if the word is context related.
        /// </summary>
        public Material mat_correct;
        /// <summary>
        /// Colour if the word is context unrelated.
        /// </summary>
        public Material mat_incorrect;
        /// <summary>
        /// Colour when the context relation is undefined.
        /// </summary>
        public Material mat_undefined;

        public Material mat_trans;

        public GameObject Explanation;

        [SerializeField]
        private bool cloud_spawned = false;

        /// <summary>
        /// Deletes the current WordCloud.
        /// </summary>
        public void DestroyCloud()
        {
            foreach (Transform t in transform)
            {
                if (t.GetComponent<Word>() != null)
                {
                    Destroy(t.gameObject);
                }
            }
            cloud_spawned = false;
            TextMeshProUGUI textfield = Explanation.GetComponent<TextMeshProUGUI>();
            textfield.text = "Please start again!";
        }

        /// <summary>
        /// Starts the generation of the WordCloud.
        /// </summary>
        public void GenerateWordCloud()
        {
            StartCoroutine(CreateWords());
        }


        /// <summary>
        /// User clicks on words, triggers reaction.
        /// </summary>
        public void OnMouseDown(GameObject go)
        {
            GameObject m_hittedObject = go;
            Word tmp_word = null;
            //Only check objects that have a Word component
            if (m_hittedObject.GetComponent<Word>() != null)
            {
                tmp_word = m_hittedObject.GetComponent<Word>();
            }
            WordInCloud tmp_content = null;
            WordInCloud.CR related = WordInCloud.CR.undefined;

            if (tmp_word != null)
            {
                string tmp_text = tmp_word.Text;
                foreach (WordInCloud w in Content.Words)
                {
                    //Search for the correct word in the scriptable objects
                    if (w.Word == tmp_text)
                    {
                        tmp_content = w;
                    }
                }

                related = tmp_content.ContentRelated;
            }

            Renderer[] rr = tmp_word.GetComponentsInChildren<Renderer>();

            //Visualize the context relation of the word to the topic.
            switch (related)
            {
                case WordInCloud.CR.yes:
                    for (int i = 0; i < rr.Length; i++)
                    {
                        if (rr[i].gameObject.tag == "NoColorChange")
                        {
                            //Do nothing
                            //It's the bounding box
                        }
                        else
                        {
                            rr[i].material = mat_correct;
                        }
                    }
                    break;
                case WordInCloud.CR.no:
                    for (int i = 0; i < rr.Length; i++)
                    {
                        if (rr[i].gameObject.tag == "NoColorChange")
                        {
                            //Do nothing
                            //It's the bounding box
                        }
                        else
                        {
                            rr[i].material = mat_incorrect;
                        }

                    }
                    break;
                case WordInCloud.CR.undefined:
                    for (int i = 0; i < rr.Length; i++)
                    {
                        if (rr[i].gameObject.tag == "NoColorChange")
                        {
                            //Do nothing
                            //It's the bounding box
                        }
                        else
                        {
                            rr[i].material = mat_undefined;
                        }

                    }
                    break;
                default:
                    Debug.Log("Error code 42.");
                    break;
            }

            TextMeshProUGUI textfield = Explanation.GetComponent<TextMeshProUGUI>();
            textfield.text = tmp_content.Explanation;

        }

        /// <summary>
        /// Creates the single parts of the word cloud.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CreateWords()
        {
            //if (cloud_spawned)
            //{
            //    //use the current transform
            //    //change nothing
            //}
            //else
            //{
            //    transform.parent = null;
            //    //cloudposition = camera forward position
            //    GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            //    transform.position = camera.transform.position;
            //    transform.position = camera.transform.forward * 5.0f;
            //    transform.parent = LevelManager.Instance.Roboy.transform.parent;

            //}

            foreach (WordInCloud w in Content.Words)
            {
                var word = Instantiate(WordPrefab, transform);
                SimpleHelvetica Word3D = word.Obj.GetComponent<SimpleHelvetica>();
                Word3D.Text = w.Word;
                word.Text = Word3D.Text;
                word.name = "Word-" + Word3D.Text;
                if (Word3D.Text.Length > 4)
                {
                    Word3D.Orientation = SimpleHelvetica.alignment.horizontal;
                }
                if (Word3D.Text.Length < 4)
                {
                    int n = Random.Range(0, 6);
                    if (n < 5)
                    {
                        Word3D.Orientation = SimpleHelvetica.alignment.vertical;
                    }
                    if (n >= 5)
                    {
                        Word3D.Orientation = SimpleHelvetica.alignment.horizontal;
                    }
                }
                Word3D.GenerateText();
                Word3D.AddBoxCollider();
                word.transform.localScale = Word3D.transform.localScale;


                //Add Collider
                BoxCollider2D tmp_collider = word.gameObject.AddComponent<BoxCollider2D>();
                tmp_collider.size = Word3D.GetComponent<BoxCollider2D>().size;
                tmp_collider.offset = Word3D.GetComponent<BoxCollider2D>().offset;
                Word3D.RemoveBoxCollider();
                word.transform.parent = transform;

                yield return StartCoroutine(PlaceWord(word));


            }
            cloud_spawned = true;
            yield return null;




        }
        /// <summary>
        /// Placing the word along a geometric path, if there are collision look further for an empty spot.
        /// </summary>
        /// <param name="word">The word that will be placed on the spiral.</param>
        /// <returns></returns>
        private IEnumerator PlaceWord(Word word)
        {
            word.gameObject.layer = LayerMask.NameToLayer("Ignore");
            int layerMask = ~(1 << LayerMask.NameToLayer("Ignore"));

            var currentDistance = 0f;
            var currentAngle = 0f;
            Bounds tmp = word.gameObject.GetComponent<BoxCollider2D>().bounds;

            while (true)
            {
                var radians = currentAngle * Mathf.Deg2Rad;
                var x = Mathf.Cos(radians) * currentDistance;
                var y = Mathf.Sin(radians) * currentDistance;
                word.transform.position = new Vector3(word.transform.parent.position.x + x, word.transform.parent.position.y + y, word.transform.parent.position.z);
                currentDistance += 0.005f;
                currentAngle += 2.5f;
                Vector2 pos = new Vector2(word.transform.position.x, word.transform.position.y);
                Vector2 size = new Vector2(tmp.size.x, tmp.size.y);
                Vector2 dir = new Vector2(x, y);
                RaycastHit2D[] r_tmp = Physics2D.BoxCastAll(pos, size * 2, 0.0f, dir);

                if (r_tmp.Length == 1)
                {
                    GameObject boundingbox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    boundingbox.transform.parent = word.transform;
                    boundingbox.gameObject.tag = "NoColorChange";
                    boundingbox.transform.localScale = new Vector3(tmp.size.x, tmp.size.y, 0.1f);
                    Vector2 offset = word.GetComponent<BoxCollider2D>().offset;
                    boundingbox.transform.position = new Vector3(word.transform.position.x + offset.x, word.transform.position.y + offset.y, word.transform.position.z + 0.01f);
                    boundingbox.GetComponent<Renderer>().material = mat_trans;
                    DestroyImmediate(boundingbox.GetComponent<BoxCollider>());
                    break;
                }

                
            }
            word.gameObject.layer = LayerMask.NameToLayer("Words");
            yield return null;
        }
    }
}
