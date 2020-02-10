using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class help : MonoBehaviour
{

    public tips book;
    public Text definition;
    public Text examples;
    public Text synonyms;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(getHelp());
    }

    public void reload()
    {
        StartCoroutine(getHelp());
    }

    public void close()
    {
        SceneManager.LoadScene("game");
    }

    // Gets info for word and populates
    public IEnumerator getHelp()
    {

        WWWForm form = new WWWForm();
        form.AddField("method", "getHelp");
        form.AddField("word", varmanager.Instance.getCurrentWord());
        using (var w = UnityWebRequest.Post("https://nati.games/apis/spellingflea.cfc", form))
        {
            yield return w.SendWebRequest();

            if (w.isNetworkError || w.isHttpError)
            { print(w.error); }
            else
            {
                var myString = w.downloadHandler.text;
                var temp = JsonUtility.FromJson<tips>(myString);
                definition.text = temp.definition;
                examples.text = temp.examples;
                synonyms.text = temp.synonyms;
            }
        }
    }

}
