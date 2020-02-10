using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class endgame : MonoBehaviour
{
    public Text practice;
    public Text score;

    // Start is called before the first frame update
    void Start()
    {
        practice.text = varmanager.Instance.wrongwords();
        score.text = varmanager.Instance.getNumberScore().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Menu()
    {
        SceneManager.LoadScene("menu");
    }

    public void Leaders()
    {
        SceneManager.LoadScene("leaders");
    }

}
