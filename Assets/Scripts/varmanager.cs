using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class varmanager : MonoBehaviour
{

    public bool usePayPal = true; //set to true when compiling for web 
    private info info;

    public float score = 0f;
    public List<string> right;
    public List<string> wrong;
    public string[] words;
    public int wordindex = 0;
    public string wordsleft;

    public bool ready = false;

    private static varmanager instance = null;
    public static varmanager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (varmanager)FindObjectOfType(typeof(varmanager));
            }
            return instance;
        }
    }

    void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }


    private void Start()
    {
        // Initialize vars
        StartCoroutine(PlayerInfo());
        init();
    }

    public void init()
    {
        wordindex = 0;
        words = new string[0];
        right = new List<string>();
        wrong = new List<string>();
        ready = false;
        StartCoroutine(API_getWords());
    }

    public bool usepaypal()
    {
        return usePayPal;
    }

    public string wrongwords()
    {
        if (wrong.Count == 0)
        {
            return "Perfect Game! Nothing to Practice...";
        }
        else
        {
            var myString = "";

            for (var i = 0; i < wrong.Count; i++)
            {
                if (i == 0)
                    myString = wrong[i];
                else
                    myString = myString + ", " + wrong[i];
            }
            return myString;
        }
    }


    public string remain()
    {
        return wordsleft;
    }

    public void nextWord()
    {
        wordindex++;
    }

    public string getCurrentWord()
    {
        if (wordindex < words.Length && words.Length > 0 && ready)
            return words[wordindex];
        else
            return "";
    }

    public info getinfo()
    {
        return info;
    }


    public void addRight(string myword)
    {
        right.Add(myword);
    }

    public void addWrong(string myword)
    {
        wrong.Add(myword);
    }

    public string getScore()
    {
        return right.Count + " of " + (right.Count + wrong.Count);
    }

    public float getNumberScore()
    {
        if (right.Count + wrong.Count > 0)
            return (right.Count / (right.Count + wrong.Count)) * 10;
        else
            return 0;
    }

    public string getRemain()
    {
        return (words.Length - wordindex) + " words remaining";
    }

    public string getPackName()
    {
        return PlayerPrefs.GetString("packname");
    }

    public string getLevel()
    {
        switch (PlayerPrefs.GetString("level"))
        {
            case "E":
                return "Level: Easy";
            case "M":
                return "Level: Normal";
            case "H":
                return "Level: Hard";
            default:
                return "";
        }

    }


    // Fetch words and store them
    public IEnumerator API_getWords()
    {
        words = new string[0];
        WWWForm form = new WWWForm();
        form.AddField("method", "getWords");
        form.AddField("level", PlayerPrefs.GetString("level"));
        form.AddField("pack", PlayerPrefs.GetInt("pack"));
        using (var w = UnityWebRequest.Post("https://nati.games/apis/spellingflea.cfc", form))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            { print(w.error); }
            else
            {
                var myString = w.downloadHandler.text;
                words = myString.Split(","[0]);
                ready = true;

            }
        }
    }

    // Fetch words and store them
    public IEnumerator PlayerInfo()
    {
        //wordlist = new string[0];
        WWWForm form = new WWWForm();
        form.AddField("method", "playerinfo");
        form.AddField("userid", PlayerPrefs.GetInt("userid"));
        form.AddField("pack", PlayerPrefs.GetInt("pack"));
        form.AddField("level", PlayerPrefs.GetString("level"));
        using (var w = UnityWebRequest.Post("https://nati.games/apis/spellingflea.cfc", form))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            { print(w.error); }
            else
            {
                var myString = w.downloadHandler.text;
                info = JsonUtility.FromJson<info>(myString);
                PlayerPrefs.SetString("packname",info.packname);
                wordsleft = info.leftwords + " Words Left";
            }
        }
    }


}
