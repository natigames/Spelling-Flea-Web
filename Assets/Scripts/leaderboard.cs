using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class leaderboard : MonoBehaviour
{

    public Text name1;
    public Text name2;
    public Text name3;
    public Text name4;
    public Text name5;
    public Text yourname;
    public Text score1;
    public Text score2;
    public Text score3;
    public Text score4;
    public Text score5;
    public Text yourscore;
    public Text games1;
    public Text games2;
    public Text games3;
    public Text games4;
    public Text games5;
    public Text yourgames;
    public Text level;
    public Text pack;

    public leaders leaders;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(getLeaders());   
    }

    public void End()
    {
        SceneManager.LoadScene("menu");
    }

    // Fetch words and store them
    public IEnumerator getLeaders()
    {
        WWWForm form = new WWWForm();
        form.AddField("method", "getTop5");
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
                leaders = JsonUtility.FromJson<leaders>(myString);
                name1.text = leaders.name1;
                name2.text = leaders.name2;
                name3.text = leaders.name3;
                name4.text = leaders.name4;
                name5.text = leaders.name5;
                yourname.text = leaders.nameyou;
                score1.text = leaders.score1;
                score2.text = leaders.score2;
                score3.text = leaders.score3;
                score4.text = leaders.score4;
                score5.text = leaders.score5;
                yourscore.text = leaders.scoreyou;
                games1.text = leaders.total1;
                games2.text = leaders.total2;
                games3.text = leaders.total3;
                games4.text = leaders.total4;
                games5.text = leaders.total5;
                yourgames.text = leaders.totalyou;
                level.text = leaders.level;
                pack.text = leaders.packname;
            }
        }
    }



}
