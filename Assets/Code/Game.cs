using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Difficulty
{
    Easy,
    Hard,
}

public class Game : MonoBehaviour
{
    public GameObject panel;
    public GameObject character;
    public static Difficulty difficulty;
    public static float boardScale;
    public Button buttonExit;


    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<Board>();
        buttonExit.onClick.AddListener(OnExitClick);
    }

    // Update is called once per frame
    void Update()
    {
        // Scaling
        boardScale = (float) Screen.height / 1200.0f;
        panel.transform.localScale = new Vector3(boardScale, boardScale);
        character.transform.localScale = new Vector3(boardScale, boardScale);
    }

    public void OnExitClick()
    {
        SceneManager.LoadScene("Menu");
    }


    public static bool IsDifficultyEasy()
    {
        return difficulty == Difficulty.Easy;
    }
}
