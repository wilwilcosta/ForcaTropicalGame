using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageBotoes1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("score", 0); //seta o score como 0 para começo do game
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartForcaGame()
    {
        SceneManager.LoadScene("Forca_Game");
    }


    public void SetDificuldadeEasy()
    {
        PlayerPrefs.SetString("dificuldade", "easy");
        StartForcaGame();
    }

    public void SetDificuldadeNormal()
    {
        PlayerPrefs.SetString("dificuldade", "normal");
        StartForcaGame();
    }

    public void SetDificuldadeHard()
    {
        PlayerPrefs.SetString("dificuldade", "hard");
        StartForcaGame();
    }

    public void GoToCreditos()
    {
        SceneManager.LoadScene("Creditos");
    }

}
