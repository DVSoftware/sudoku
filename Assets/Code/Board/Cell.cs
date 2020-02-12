using UnityEngine;
using UnityEngine.UI;

public class Cell : ScriptableObject
{
    private int x;
    private int y;
    private bool generated;
    private int value;
    private Button button;
    private Button buttonPrefab;
    private GameObject numberInputPrefab;
    private GameObject panel;
    private Board board;

    private Color32 normalColor = new Color32(255, 255, 255, 220);
    private Color32 errorColor = new Color32(255, 140, 140, 220);
    private Color32 completeColor = new Color32(180, 255, 145, 220);
    
    public void init(int x, int y,  Board board)
    {
        this.x = x;
        this.y = y;
        this.board = board;

        button.transform.Translate(new Vector3(
           20 + ((x / 3) * 20) + x * 110,
           20 + ((y / 3) * 20) + y * 110,
           0
        ), panel.transform);

        SetValue(0);

        button.onClick.AddListener(OnButtonClick);
    }

    public Vector2Int GetCoords()
    {
        return new Vector2Int(x, y);
    }


    public void OnEnable()
    {
        buttonPrefab = Resources.Load<Button>("Prefabs/CellButton");
        numberInputPrefab = Resources.Load<GameObject>("Prefabs/NumberInput");

        panel = GameObject.Find("SudokuPanel");
        button = Instantiate(buttonPrefab, panel.transform);
    }

    public int GetValue()
    {
        return value;
    }

    public void SetValue(int value, bool generationPhase = true)
    {
        this.value = value;
        button.GetComponentInChildren<Text>().text = value == 0 ? "" : value.ToString();
        generated = value != 0 && generationPhase;
        button.interactable = !generated;
    }

    private void OnButtonClick()
    {
        GameObject input = Instantiate(numberInputPrefab, button.transform);
        input.transform.SetParent(panel.transform);
        input.GetComponent<NumberInput>().SetHandler(HandleInput);
    }

    void HandleInput(int value)
    {

        SetValue(value, false);
        Character.GetInstance().ResetIdleTimer();

        if (board.CheckDuplicates() && Game.IsDifficultyEasy())
        {
            Character.GetInstance().TriggerAngry();
        } else
        {
            Character.GetInstance().TriggerIdle();
        }
    }

    public void SetError(bool error)
    {
        if (error)
        {
            button.GetComponent<Image>().color = errorColor;
        } else
        {
            button.GetComponent<Image>().color = normalColor;
        }
    }

    public void SetCompleted()
    {
        button.GetComponent<Image>().color = completeColor;
        button.enabled = false;
    }
}
