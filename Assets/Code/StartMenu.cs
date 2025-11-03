using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{

    public Button startbtn;
    public Button optionsbtn;
    public Button creditsbtn;
    public Button exitbtn;

    public GameObject paneloptions;
    public GameObject panelcredits;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startbtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Intro");
        }
        );

        optionsbtn.onClick.AddListener(() =>
        {
            paneloptions.SetActive(true);
            gameObject.SetActive(false);
        }
        );

        creditsbtn.onClick.AddListener(() =>
        {
            panelcredits.SetActive(true);
            gameObject.SetActive(false);
        }
        );

        exitbtn.onClick.AddListener(() => {
           Application.Quit();
        }
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
