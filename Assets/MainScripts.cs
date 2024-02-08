using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScripts : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void start()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void resetBalance()
    {
        PlayerPrefs.SetInt("Sum", 500);
    }

    public void exit()
    {
        Application.Quit();
    }
}
