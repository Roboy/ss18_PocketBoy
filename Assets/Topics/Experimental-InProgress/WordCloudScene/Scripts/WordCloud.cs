using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Wordcloud;
using TMPro;

public class WordCloud : MonoBehaviour
{

    public Word WordPrefab;
    public List<WordCloudContent> Words_Content;

    public Material mat_correct;
    public Material mat_incorrect;
    public Material mat_undefined;

    public GameObject Explanation;

    [SerializeField]
    private bool cloud_spawned = false;


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
        WordCloudContent tmp_content = null;
        WordCloudContent.CR related = WordCloudContent.CR.undefined;

        if (tmp_word != null)
        {
            string tmp_text = tmp_word.Text;
            foreach (WordCloudContent w in Words_Content)
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

        //Visualize the context relation of the word to the topic
        switch (related)
        {
            case WordCloudContent.CR.yes:
                for (int i = 0; i < rr.Length; i++)
                {
                    rr[i].material = mat_correct;
                }
                break;
            case WordCloudContent.CR.no:
                for (int i = 0; i < rr.Length; i++)
                {
                    rr[i].material = mat_incorrect;
                }
                break;
            case WordCloudContent.CR.undefined:
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

    private IEnumerator CreateWords()
    {

        foreach (WordCloudContent w in Words_Content)
        {
            var word = Instantiate(WordPrefab, transform);
            SimpleHelvetica tmp = word.Obj.GetComponent<SimpleHelvetica>();
            tmp.Text = w.Word;
            word.Text = tmp.Text;
            word.name = "Word-" + tmp.Text;
            SimpleHelvetica.alignment tmp_alignment = (SimpleHelvetica.alignment)Random.Range(0, 2);
            tmp.Orientation = tmp_alignment;
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
            //if (!Physics.CheckBox(word.transform.position, word.transform.localScale /2f, Quaternion.identity, layerMask))
            //    break;
            if (!Physics.CheckBox(word.transform.position, tmp.size, Quaternion.identity, layerMask))
                break;

        }
        word.gameObject.layer = 10;
        yield return null;
    }
}
