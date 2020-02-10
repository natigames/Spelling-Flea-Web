using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{

    public Button easybtn;
    public Button normalbtn;
    public Button hardbtn;
    public Text remain;


    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("userid") == 0)
        {
            SceneManager.LoadScene("UI");
        }

        // Initialize String Vars (int/float default to cero when new)
        if (PlayerPrefs.GetString("voice") == "0" || PlayerPrefs.GetString("voice") == "")
        {
            PlayerPrefs.SetString("voice", "English");
        }
        if (PlayerPrefs.GetString("packname") == "0" || PlayerPrefs.GetString("packname") == "")
        {
            PlayerPrefs.SetString("packname", "Columbia 1st Grade - English");
        }
        if (PlayerPrefs.GetString("level") == "0" || PlayerPrefs.GetString("level") == "")
        {
            PlayerPrefs.SetString("level", "E");
        }

        StartCoroutine(PlayerInfo());

        // Update Screen with Level GUI
        levelGUI();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Purchase()
    {
        if (varmanager.Instance.usepaypal())
            Application.OpenURL("https://nati.games/spellingflea/paypal.cfm");
        else
            SceneManager.LoadScene("purchase");
    }

    // Goes back to Login
    public void Quit()
    {
        PlayerPrefs.SetInt("userid",0);
        SceneManager.LoadScene("UI");
    }

    // Goes back to game
    public void Continue()
    {
        SceneManager.LoadScene("game");
    }

    // Restart initializes vars and launches
    public void Restart()
    {
        //vars iniciales playerprfs
        varmanager.Instance.init();
        SceneManager.LoadScene("game");
    }

    // Opens Settings Scene
    public void Settings()
    {
        SceneManager.LoadScene("settings");
    }

    // Opens Leaderboard Scene
    public void Leaders()
    {
        SceneManager.LoadScene("leaders");
    }

    // Draws the Level Options Accordingly 
    public void levelGUI()
    {
        switch (PlayerPrefs.GetString("level"))
        {
            case "E":
                easybtn.gameObject.SetActive(false);
                normalbtn.gameObject.SetActive(true);
                hardbtn.gameObject.SetActive(true);
                break;
            case "M":
                easybtn.gameObject.SetActive(true);
                normalbtn.gameObject.SetActive(false);
                hardbtn.gameObject.SetActive(true);
                break;
            case "H":
                easybtn.gameObject.SetActive(true);
                normalbtn.gameObject.SetActive(true);
                hardbtn.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    // Set the New Level 
    public void setLevel(string newlevel)
    {
        PlayerPrefs.SetString("level", newlevel);
        StartCoroutine(PlayerInfo());
        varmanager.Instance.init();
        levelGUI();
    }


    // Updates DB.player with pack/level
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
                var info = JsonUtility.FromJson<info>(myString);
                PlayerPrefs.SetString("packname", info.packname);
                remain.text = info.leftwords + " Words Left";
            }

        }
    }

}
