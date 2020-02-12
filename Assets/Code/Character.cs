using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Emotion
{
    Idle,
    Smiling,
    Confused,
    Sad,
    Annoyed,
    Angry,
    Happy,
}

public enum Speech
{
    Normal,
    Shouting,
}

public struct SpeechLine
{
    public string text;
    public Emotion emotion;
    public Speech speech;
    public SpeechLine(string text, Emotion emotion, Speech speech)
    {
        this.text = text;
        this.emotion = emotion;
        this.speech = speech;
    }
}

public class Character : MonoBehaviour
{
    public Sprite[] characterSprites;
    public Sprite[] speechSprites;

    static private Character instance;
    private Image portraitImage;
    private Image speechImage;
    private Text speechText;
    private double lastClickTime = 0f;

    private readonly SpeechLine[] introLines =
    {
        new SpeechLine("Hi, welcome to Sudoku World!", Emotion.Happy, Speech.Normal),
        new SpeechLine("I will keep you company while you play", Emotion.Smiling, Speech.Normal),
        new SpeechLine("In case you get stuck, I'll try to help you.", Emotion.Smiling, Speech.Normal),
        new SpeechLine("Enjoy!", Emotion.Happy, Speech.Normal),
    };

    private readonly SpeechLine[] wonLines =
    {
        new SpeechLine("Congratulations!", Emotion.Happy, Speech.Normal),
        new SpeechLine("You won the game!", Emotion.Smiling, Speech.Normal),
        new SpeechLine("I hope you enjoyed it.", Emotion.Happy, Speech.Normal),
    };

    private readonly SpeechLine[] idleLines =
    {
        new SpeechLine("Are you there?", Emotion.Confused, Speech.Normal),
        new SpeechLine("Look for single empty cells in a row, they are the easiest to solve", Emotion.Smiling, Speech.Normal),
        new SpeechLine("Look for single empty cells in a column and pick a number that isn't there", Emotion.Smiling, Speech.Normal),
        new SpeechLine("Look for single empty cells in an area, you'll probably figure it out", Emotion.Smiling, Speech.Normal),
        new SpeechLine("It seems that you are having a hard time solving this...", Emotion.Sad, Speech.Normal),
    };

    private readonly SpeechLine[] angryLines =
    {
        new SpeechLine("That is not how you play!", Emotion.Annoyed, Speech.Shouting),
        new SpeechLine("Try something else!", Emotion.Annoyed, Speech.Shouting),
        new SpeechLine("You need to practice more!", Emotion.Annoyed, Speech.Shouting),
        new SpeechLine("Numbers must not repeat in a row, a column or an area!", Emotion.Angry, Speech.Shouting),
        new SpeechLine("If there was a Sudoku Police, I would call them!", Emotion.Angry, Speech.Shouting),
        new SpeechLine("Pathetic...", Emotion.Sad, Speech.Normal),
    };
    // Start is called before the first frame update
    void Start()
    {
        portraitImage = transform.Find("Portrait").GetComponent<Image>();
        speechImage = transform.Find("Speech").GetComponent<Image>();
        speechText = speechImage.GetComponentInChildren<Text>();

        IEnumerator playIntro = PlayIntro();
        StartCoroutine(playIntro);

        Character.instance = this;
    }

    public static Character GetInstance()
    {
        return Character.instance;
    }

    private IEnumerator PlayIntro() 
    {
        foreach(SpeechLine introLine in introLines)
        {
            TriggerIdle();
            yield return new WaitForSeconds(.5f);
            SetSpeechLine(introLine);
            yield return new WaitForSeconds(5f);

        }

        TriggerIdle();
    }

    private IEnumerator PlayIdle()
    {
        TriggerIdle();
        yield return new WaitForSeconds(.5f);
        SetSpeechLine(idleLines[Random.Range(0, idleLines.Length)]);
        yield return new WaitForSeconds(10f);
        TriggerIdle();
    }

    private IEnumerator PlayAngry()
    {
        TriggerIdle();
        yield return new WaitForSeconds(.5f);
        SetSpeechLine(angryLines[Random.Range(0, angryLines.Length)]);
        yield return new WaitForSeconds(10f);
        TriggerIdle();
    }

    private IEnumerator PlayWon()
    {
        foreach (SpeechLine wonLine in wonLines)
        {
            TriggerIdle();
            yield return new WaitForSeconds(.5f);
            SetSpeechLine(wonLine);
            yield return new WaitForSeconds(5f);

        }

        SceneManager.LoadScene("Menu");
    }

    // Update is called once per frame
    void Update()
    {
        lastClickTime += Time.deltaTime;
        if (lastClickTime > 60)
        {
            lastClickTime = 0;
            IEnumerator playIdle = PlayIdle();
            StartCoroutine(playIdle);
        }
    }

    public void SetSpeechLine(SpeechLine speechLine)
    {
        speechImage.enabled = true;
        speechText.enabled = true;
        portraitImage.sprite = characterSprites[(int)speechLine.emotion];
        speechImage.sprite = speechSprites[(int)speechLine.speech];
        speechText.text = speechLine.text;
    }

    public void TriggerIdle()
    {
        speechImage.enabled = false;
        speechText.enabled = false;
        portraitImage.sprite = characterSprites[(int)Emotion.Idle];
    }

    public void TriggerAngry()
    {
        StopAllCoroutines();
        IEnumerator playAngry = PlayAngry();
        StartCoroutine(playAngry);
    }

    public void TriggerWin()
    {
        StopAllCoroutines();
        IEnumerator playWon = PlayWon();
        StartCoroutine(playWon);
    }

    public void ResetIdleTimer()
    {
        StopAllCoroutines();
        lastClickTime = 0;
    }
}
