using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinnerManager : MonoBehaviour

{

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("score", 0); //zera o score para recomeçar o game
        RenderUltimaPalavra();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Forca_Game");
    }

    public void RenderUltimaPalavra()
    {
        GameObject.Find("UltimaPalavra").GetComponent<Text>().text = PlayerPrefs.GetString("ultimaPalavra").ToString().ToUpper() ;
    }
}
