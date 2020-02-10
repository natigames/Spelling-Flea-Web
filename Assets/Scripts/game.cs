using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class game : MonoBehaviour
{

    // Vars for Audio
    private GCTextToSpeech MC;
    VoiceConfig myvoice = new VoiceConfig();

    // Vars for UI
    private TouchScreenKeyboard mobileKeys;
    public InputField answer;
    public Image readycallout;

    // Vars for Animation
    public AudioClip rightSound, wrongSound;

    // Vars for HUD Display
    public Text scoreDisplay;
    public Text remainDisplay;
    public Text packDisplay;
    public Text levelDisplay;

    // Var for IAP Check
    public bool needsIAP = false;

    void InitAudio()
    {
        // Initilize Text2Voice & Voice
        MC = GCTextToSpeech.Instance;
        MC.SynthesizeSuccessEvent += _gcTextToSpeech_SynthesizeSuccessEvent;
        MC.SynthesizeFailedEvent += _gcTextToSpeech_SynthesizeFailedEvent;
        switch (PlayerPrefs.GetString("voice"))
        {
            case "English":
                myvoice.languageCode = "en-US";
                myvoice.gender = Enumerators.SsmlVoiceGender.FEMALE;
                myvoice.name = "en-US-Wavenet-E";
                break;
            case "Español":
                myvoice.languageCode = "es-ES";
                myvoice.gender = Enumerators.SsmlVoiceGender.FEMALE;
                myvoice.name = "es-ES-Standard-A";
                break;
            case "Français":
                myvoice.languageCode = "fr-FR";
                myvoice.gender = Enumerators.SsmlVoiceGender.FEMALE;
                myvoice.name = "fr-FR-Wavenet-E";
                break;
            case "Portuguese":
                myvoice.languageCode = "pt-BR";
                myvoice.gender = Enumerators.SsmlVoiceGender.FEMALE;
                myvoice.name = "pt-BR-Wavenet-A";
                break;
            case "Italiano":
                myvoice.languageCode = "it-IT";
                myvoice.gender = Enumerators.SsmlVoiceGender.FEMALE;
                myvoice.name = "it-IT-Wavenet-B";
                break;
            default:
                PlayerPrefs.SetString("voice", "English");
                myvoice.languageCode = "en-US";
                myvoice.gender = Enumerators.SsmlVoiceGender.FEMALE;
                myvoice.name = "en-US-Wavenet-E";
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("userid") == 0)
        {
            SceneManager.LoadScene("UI");
        }


        InitAudio();
        HudUpdate();
        mobileKeys = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false);

        answer.onValueChanged.AddListener(val =>
        {
            if (answer.text.Length > 0)
            {
                var myletter = answer.text.Substring(answer.text.Length - 1, 1);
                speakword(myletter);
            }
        });

        answer.onEndEdit.AddListener(val =>
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || mobileKeys.status == TouchScreenKeyboard.Status.Done)
                Submit();
        });

    }

    // Update is called once per frame
    void Update()
    {
    }


    public void HudUpdate()
    {
        scoreDisplay.text = varmanager.Instance.getScore();
        remainDisplay.text = varmanager.Instance.getRemain();
        packDisplay.text = "Pack: " + varmanager.Instance.getPackName();
        levelDisplay.text = varmanager.Instance.getLevel() + " | Voice: " + PlayerPrefs.GetString("voice");
    }

    public void DestroyImage(Image obj)
    {
        foreach (Transform child in obj.transform)
        {
            Destroy(child.gameObject);
        }
        Destroy(obj);
    }

    public void repeat()
    {
        if (varmanager.Instance.ready)
        {
            if (readycallout) { DestroyImage(readycallout); }
            HudUpdate();
            focusoninput();
            speakword(varmanager.Instance.getCurrentWord());
        }
    }

    public void speakword(string word)
    {
        if (needsIAP)
        {
            if (varmanager.Instance.usepaypal())
                Application.OpenURL("https://nati.games/spellingflea/paypal.cfm");
            else
                SceneManager.LoadScene("purchase");
        }
        else
        {
            if (word.Length > 0)
            {
                MC.Synthesize(word, myvoice, false, 1, 1, 44000);
            }
        }
    }

    public void Submit()
    {

        if (answer.text.Length > 0)
        {
            var myAnswer = answer.text;
            var myTarget = varmanager.Instance.getCurrentWord();

            // Log Answer
            StartCoroutine(LogAnswer(varmanager.Instance.getCurrentWord()));

            // Repeat Full Word
            speakword(varmanager.Instance.getCurrentWord());

            // Grade Word & animate
            gradeAnswer(myAnswer,myTarget);
            HudUpdate();

            // Tween & Clear textç
            // move sprite towards the target location

            StartCoroutine(nextword()); 

        }
    }


    public void gradeAnswer(string myanswer,string mytarget)
    {

        if (mytarget.ToLower() == myanswer.ToLower())
        {
            StartCoroutine(playCorrect());
            varmanager.Instance.addRight(mytarget);
            //StartCoroutine(DisplayResult("Yes! " + wordlist[PlayerPrefs.GetInt("currentword")]));
        }
        else
        {
            StartCoroutine(playWrong());
            varmanager.Instance.addWrong(mytarget);
            //StartCoroutine(DisplayResult("No! " + wordlist[PlayerPrefs.GetInt("currentword")]));
        }
    }


    //set focus on input box
    public void focusoninput()
    {
        EventSystem.current.SetSelectedGameObject(answer.gameObject, null);
        answer.OnPointerClick(new PointerEventData(EventSystem.current));
    }


    public void menu()
    {
        SceneManager.LoadScene("menu");
    }

    public void help()
    {
        if (!readycallout)
            SceneManager.LoadScene("help");
    }

    // Play audio & video for correct word
    private IEnumerator playCorrect()
    {
        flea.instance.doAnim("correct");
        NewAudio.Instance.PlayOneShot(rightSound);
        yield return new WaitForSeconds(2f);
        flea.instance.doAnim("normal");
    }

    // Play audio & video for wrong word
    private IEnumerator playWrong()
    {
        flea.instance.doAnim("wrong");
        NewAudio.Instance.PlayOneShot(wrongSound);
        yield return new WaitForSeconds(2f);
        flea.instance.doAnim("normal");
    }


    public IEnumerator nextword()
    {
        yield return new WaitForSeconds(2);
        varmanager.Instance.nextWord();
        var thisword = varmanager.Instance.getCurrentWord();
        if (thisword == "")
        {
            SceneManager.LoadScene("over");
        }
        else
        {
            speakword(thisword);
            answer.text = "";
            focusoninput();
        }
    }

    // Logs Answer in DB
    private IEnumerator LogAnswer(string myword)
    {
        WWWForm form = new WWWForm();
        form.AddField("method", "logAnswer");
        form.AddField("input", myword);
        form.AddField("word", varmanager.Instance.getCurrentWord());
        form.AddField("level", PlayerPrefs.GetInt("level"));
        form.AddField("pack", PlayerPrefs.GetInt("pack"));
        form.AddField("userid", PlayerPrefs.GetInt("userid"));
        using (var w = UnityWebRequest.Post("https://nati.games/apis/spellingflea.cfc", form))
        {
            yield return w.SendWebRequest();
            int leftwords = int.Parse(w.downloadHandler.text);
            if (leftwords <= 0)
            {
                needsIAP = true; 
            }
            
        }
    }


    #region failed handlers
    private void _gcTextToSpeech_SynthesizeFailedEvent(string error)
    {
        Debug.Log(error);
    }
    #endregion failed handlers

    #region sucess handlers
    private void _gcTextToSpeech_SynthesizeSuccessEvent(PostSynthesizeResponse response)
    {
        var myclip = MC.GetAudioClipFromBase64(response.audioContent, Constants.DEFAULT_AUDIO_ENCODING);
        NewAudio.Instance.PlayWord(myclip);
    }
    #endregion sucess handlers

}
