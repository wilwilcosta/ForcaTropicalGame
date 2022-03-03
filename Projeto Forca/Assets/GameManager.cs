using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject letra;            // prefab da letra
    public GameObject centroDaTela;     // usado para pegar as coordenadas do centro da tela
    public string Dificuldade ; //Pega a dificuldade setada na tela de start

    private string palavraSecreta = ""; // palavra que será adivinhada
    private int tamanhoPalavra = 0;     // tamanho da palavra
    char[] letrasOcultas;               // array com as letras da palavra secreta
    bool[] letrasDescobertas;           // array que armazena qual das letras já foi descoberta
    private List<char> letrasErradas = new List<char>();   // lista que armazena todas as letras únicas inputadas que não existem na frase
    private int contadorErros = 0;     // inteiro que conta o número de erros

    private int numTentativas = 0;     //numero de tentativas do round
    private int numMaximoTentativas = 10;   //numero máximo de tentativas pqra que perca o jogo

    private int tamanhoMinimoPalavra = 0; //tamanho mínimo da palavra que é setado de acordo com a dificuldade
    private int tamanhoMaximoPalavra = 0; //tamanho máximo da palavra que é setado de acordo com a dificuldade

    int score = 0;                        //pontuação do jogador, aumenta a cada letra adivinhada corretamente
 
    public List<string> palavrasSecretas = new List<string> // array com todas as palavras ocultas
    {
        "carro",
        "elefante",
        "banana",
        "macaco",
        "predio",
        "teclado",
        "pote",
        "bola"
    };
   
    // Start is called before the first frame update
    void Start()
    {
        Dificuldade = PlayerPrefs.GetString("dificuldade");
        centroDaTela = GameObject.Find("centroDaTela");
        InicializarGame();
        InitLetras();
        UpdateNumTentativas();
        UpdateScore();
    }

    // Update is called once per frame
    void Update()
    {
        ChecarLetra();
    }

    void InitLetras()
    {
        int numLetras = tamanhoPalavra; //inicializar os "?" de acordo com tamanho da palavra

        for (int i = 0; i < numLetras; i++)
        {
            Vector3 novaPosicao;
            GameObject l;
            
            novaPosicao = new Vector3(centroDaTela.transform.position.x + ( (i - numLetras / 2.0f) * 80), centroDaTela.transform.position.y, centroDaTela.transform.position.z );
            l = (GameObject)Instantiate(letra, novaPosicao, Quaternion.identity);
            l.name = $"letra {i + 1}";
            l.transform.SetParent(GameObject.Find("Canvas").transform);
        }
    }

    void InicializarGame()
    {

        int numeroAleatorioParaEscolha = Random.Range(0, palavrasSecretas.Count - 1);
        SetDificuldade();

        palavraSecreta = PegaPalavraArquivo();          //Implementação da escolha randômica de palavras armazenadas no programa

        bool expressao = (palavraSecreta.Length >= tamanhoMinimoPalavra && palavraSecreta.Length <= tamanhoMaximoPalavra);
        while (!expressao)
        {
            palavraSecreta = PegaPalavraArquivo();
            expressao = (palavraSecreta.Length >= tamanhoMinimoPalavra && palavraSecreta.Length <= tamanhoMaximoPalavra);
        }

        palavraSecreta = palavraSecreta.ToLower();
        tamanhoPalavra = palavraSecreta.Length;

        letrasOcultas = new char[tamanhoPalavra];
        letrasDescobertas = new bool[tamanhoPalavra];

        letrasOcultas = palavraSecreta.ToCharArray();
        
    }

    void ChecarLetra()
    {
        if (Input.anyKeyDown)
        {
            char input = Input.inputString.ToLower().ToCharArray()[0];
            int letraTecladaComoInt = System.Convert.ToInt32(input);
            
            if(letraTecladaComoInt >= 97 && letraTecladaComoInt <= 122) //forma mais simples de checar se o input colocado é uma letra
            {
                numTentativas++;
                UpdateNumTentativas();
                bool anyLetraCerta = false;
                for (int i = 0; i < tamanhoPalavra; i++)  
                {
                    if (!letrasDescobertas[i])                         // se o índice não possuir uma letra já conhecida, ele checa se a letra está correta, caso já seja conhecida, ele segue
                    {
                        
                        if(input == letrasOcultas[i])
                        {
                            letrasDescobertas[i] = true;
                            CheckSeGanhou();
                        
                            GameObject letraDesmascarada = GameObject.Find($"letra {i + 1}");
                            letraDesmascarada.GetComponent<Text>().text = letrasOcultas[i].ToString().ToUpper();
                            
                            anyLetraCerta = true;

                            score = PlayerPrefs.GetInt("score");
                            score++;
                            PlayerPrefs.SetInt("score", score);
                            UpdateScore();
                        }
                    }
            
                }
                if (!anyLetraCerta)
                {
                    AdicionarLetrasErradas(input);
                }
            }
        }
    }

    void AdicionarLetrasErradas(char letra)
    {
        if (!letrasErradas.Contains(letra))
        {
            letrasErradas.Add(letra); // caso nãao haja a letra errada na lista, sérá adicionada
            contadorErros++;          // e será incrementado o contador de erro, caso o erro seja repetido, não será considerado
            RenderLetrasErradas();
        }

    }

    void RenderLetrasErradas()
    {
        int numLetrasErradas = letrasErradas.Count;
        int i = numLetrasErradas > 0 ? numLetrasErradas - 1 : -1; //Caso não haja letras erradas, não será renderizado nada 
        for (i = i; i < numLetrasErradas; i++)
        {
            Vector3 novaPosicao;
            GameObject l;

            novaPosicao = new Vector3(centroDaTela.transform.position.x + ((i - numLetrasErradas / 2.0f) * 120) - 300, centroDaTela.transform.position.y + 185, centroDaTela.transform.position.z); //renderiza as letras na posição superior esquerda da tela
            l = (GameObject)Instantiate(letra, novaPosicao, Quaternion.identity);
            l.name = $"letraErrada {i + 1}";
            l.transform.SetParent(GameObject.Find("Canvas").transform);
            Text letraErrada = l.GetComponent<Text>();      // pegando o componente de texto da letra errada numero i
            letraErrada.text = letrasErradas[i].ToString().ToUpper(); // mudando o texto da letra errada
            letraErrada.color = Color.red;                  // mudando a cor do texto da letra errada para vermelho
        }
    }

    void AdicionarNovaPalavra(string palavra)
    {
        //Modificado o tipo  da array "palavrasSecretas" por umna lista de strings assim não teremos que fazer com um tamanho fixo de array acabando com a necessidade de
        //criar outra array e substituir pela outra no momento de uma nova palavra adicionada. 
        TextAsset text = (TextAsset)Resources.Load("palavras", typeof(TextAsset));

        string s = text.text;
        string[] palavras = s.Split(' ');
        palavras[palavras.Length + 1] = palavra;
    }

    void UpdateNumTentativas()
    {
        GameObject.Find("numTentativas").GetComponent<Text>().text = numTentativas.ToString() + " | " + numMaximoTentativas;
        if(numTentativas >= numMaximoTentativas)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    void UpdateScore()
    {
        GameObject.Find("scoreUI").GetComponent<Text>().text = $"Score: {score}";

    }

    void CheckSeGanhou()
    {
        bool ganhou = true;

        for(int i = 0; i < tamanhoPalavra; i++)
        {
            ganhou = ganhou && letrasDescobertas[i];
        }

        if (ganhou)
        {
            PlayerPrefs.SetString("ultimaPalavra", palavraSecreta);
            SceneManager.LoadScene("Winner");
        }
    }

    public string PegaPalavraArquivo()
    {
        TextAsset text = (TextAsset)Resources.Load("palavras", typeof(TextAsset));

        string s = text.text;
        string[] palavras = s.Split(' ');
        int numAleatorio = Random.Range(0, palavras.Length + 1);
        return palavras[numAleatorio];
    }

    public void SetDificuldade()
    {
        if (Dificuldade == "easy")
        {
            tamanhoMinimoPalavra = 0;
            tamanhoMaximoPalavra = 4;
        }
        else if (Dificuldade == "normal")
        {
            tamanhoMinimoPalavra = 5;
            tamanhoMaximoPalavra = 6;
        }
        else if (Dificuldade == "hard")
        {
            tamanhoMinimoPalavra = 7;
            tamanhoMaximoPalavra = 15;
        }
    }

}
 