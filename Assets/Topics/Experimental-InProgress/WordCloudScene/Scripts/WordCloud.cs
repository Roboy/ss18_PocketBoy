using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Wordcloud;
using TMPro;

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


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && cloud_spawned)
        {
            OnMouseDown();
        }
    }

    /// <summary>
    /// User clicks on words, triggers reaction.
    /// </summary>
    private void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        GameObject m_hittedObject = null;

        int layer_mask = LayerMask.GetMask("Words");

        //Check if an object is hit at all, but only on the specific layer
        if (Physics.Raycast(ray, out hit, 1000, layer_mask))
        {
            m_hittedObject = hit.transform.gameObject;
        }

        if (m_hittedObject == null)
            return;
        //Only check objects that have a Word component
        Word tmp_word = m_hittedObject.GetComponent<Word>();
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
                    rr[i].material = mat_correct;
                }
                break;
            case WordInCloud.CR.no:
                for (int i = 0; i < rr.Length; i++)
                {
                    rr[i].material = mat_incorrect;
                }
                break;
            case WordInCloud.CR.undefined:
                for (int i = 0; i < rr.Length; i++)
                {
                    rr[i].material = mat_undefined;
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

        foreach (WordInCloud w in Content.Words)
        {
            var word = Instantiate(WordPrefab, transform);
            SimpleHelvetica tmp = word.Obj.GetComponent<SimpleHelvetica>();
            tmp.Text = w.Word;
            word.Text = tmp.Text;
            word.name = "Word-" + tmp.Text;
            int n = Random.Range(0, 11);
            if (n < 2)
            {
                tmp.Orientation = SimpleHelvetica.alignment.vertical;
            }
            if (n >= 2)
            {
                tmp.Orientation = SimpleHelvetica.alignment.horizontal;
            }
            
            tmp.GenerateText();
            tmp.AddBoxCollider();
            word.transform.localScale = tmp.transform.localScale;


            //Add Collider
            BoxCollider tmp_collider = word.gameObject.AddComponent<BoxCollider>();
            tmp_collider.size = tmp.GetComponent<BoxCollider>().size;
            tmp_collider.center = tmp.GetComponent<BoxCollider>().center;
            tmp.RemoveBoxCollider();

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
        Bounds tmp = word.gameObject.GetComponent<BoxCollider>().bounds;
        GameObject parent = word.transform.parent.gameObject;
        while (true)
        {
            var radians = currentAngle * Mathf.Deg2Rad;
            var x = Mathf.Cos(radians) * currentDistance;
            var y = Mathf.Sin(radians) * currentDistance;
            word.transform.position = new Vector3(word.transform.parent.position.x + x, word.transform.parent.position.y + y, word.transform.parent.position.z);
            currentDistance += 0.0005f;
            currentAngle += 0.1f;
            if (!Physics.CheckBox(word.transform.position, tmp.size, Quaternion.identity, layerMask))
                break;

        }
        word.gameObject.layer = 10;
        yield return null;
    }
}
