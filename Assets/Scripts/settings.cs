using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class settings : MonoBehaviour
{
    public Packslist Packslist = new Packslist();
    private List<string> options = new List<string>();
    public Dropdown PackDropDown;
    public Dropdown VoiceDropDown;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(getPacks());

        //Debug.Log(PlayerPrefs.GetString("voice"));
        for (var i = 0; i < VoiceDropDown.options.Count; i++)
        {
            if (VoiceDropDown.options[i].text == PlayerPrefs.GetString("voice"))
            {
                VoiceDropDown.value = i;
            }
        }



        PackDropDown.onValueChanged.AddListener(delegate {
            Listen2Select();
        });

        VoiceDropDown.onValueChanged.AddListener(delegate {
            PlayerPrefs.SetString("voice", VoiceDropDown.options[VoiceDropDown.value].text);
            //Debug.Log("Voice: " + PlayerPrefs.GetString("voice"));
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Close()
    {
        SceneManager.LoadScene("menu");
    }


    public void Listen2Select()
    {

        foreach (packs packs in Packslist.Packs)
        {
            if (PackDropDown.options[PackDropDown.value].text == packs.packname)
            {
                PlayerPrefs.SetInt("pack", packs.packid);
                PlayerPrefs.SetString("packname", packs.packname);
                //Debug.Log("pack selected: " + packs.packname + " / " + packs.packid);
            }

        }

    }


    public void SetPackActive()
    {
        //get pack name
        //Debug.Log(PlayerPrefs.GetString("packname"));
        for (var i = 0; i < PackDropDown.options.Count; i++)
        {
            if (PackDropDown.options[i].text == PlayerPrefs.GetString("packname"))
            {
                PackDropDown.value = i;
            }
        }
    }

    // Fetch words and store them
    public IEnumerator getPacks()
    {
        options.Clear();
        PackDropDown.options.Clear();
        PackDropDown.ClearOptions();
        Packslist = new Packslist();

        WWWForm form = new WWWForm();
        form.AddField("method", "getPacks");
        form.AddField("userid", PlayerPrefs.GetInt("userid"));
        using (var w = UnityWebRequest.Post("https://nati.games/apis/spellingflea.cfc", form))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError)
            { print(w.error); }
            else
            {
                string myString = w.downloadHandler.text;
                Packslist = JsonUtility.FromJson<Packslist>(myString);

                foreach (packs pack in Packslist.Packs)
                {
                    options.Add(pack.packname);
                }
                PackDropDown.AddOptions(options);
                SetPackActive();


            }
        }
    }



}
