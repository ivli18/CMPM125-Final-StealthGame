using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private TMP_Text scoreText;
    private int score = 0;
    private int scoreAtSceneStart = 0;
    public int GetScore() => score;

    void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject); return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        scoreAtSceneStart = score;
        GameObject obj = GameObject.FindWithTag("ScoreText");
        if (obj != null) scoreText = obj.GetComponent<TMP_Text>();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }
    public void ResetScore()
    {
        score = scoreAtSceneStart;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}