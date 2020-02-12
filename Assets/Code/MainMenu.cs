using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button buttonNewGame, buttonExit, buttonBack, buttonEasy, buttonHard;
    public GameObject mainMenuContainer, difficultyMenuContainer;
    Animator mainMenuAnimator;
    Animator difficultyMenuAnimator;

    // Start is called before the first frame update
    void Start()
    {
        buttonNewGame.onClick.AddListener(OnNewGameClick);
        buttonExit.onClick.AddListener(OnExitClick);
        buttonBack.onClick.AddListener(OnBackClick);
        buttonEasy.onClick.AddListener(delegate { OnDifficultyClick(Difficulty.Easy); });
        buttonHard.onClick.AddListener(delegate { OnDifficultyClick(Difficulty.Hard); });

        mainMenuAnimator = mainMenuContainer.GetComponent<Animator>();
        difficultyMenuAnimator = difficultyMenuContainer.GetComponent<Animator>();
    }

    public void OnDifficultyClick(Difficulty difficulty)
    {
        Game.difficulty = difficulty;
        SceneManager.LoadScene("Game");
    }

    public void OnBackClick()
    {
        mainMenuAnimator.Play("MoveIn");
        difficultyMenuAnimator.Play("MoveAway");
    }

    public void OnNewGameClick()
    {
        mainMenuAnimator.Play("MoveAway");
        difficultyMenuAnimator.Play("MoveIn");

    }

    public void OnExitClick()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

}
