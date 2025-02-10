using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainmenuManager : MonoBehaviour
{
    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager.PlayBGM("Mainmenu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GotoScene(string sceneName)
    {
        audioManager.PlaySFX("click");
        SceneManager.LoadScene(sceneName);
    }
}
