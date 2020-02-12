using System;
using UnityEngine;
using UnityEngine.UI;

public class NumberInput : MonoBehaviour
{
    private static bool isActive = true;
    public delegate void Handler(int value);
    RectTransform rectTransform;

    Handler handler;

    // Start is called before the first frame update
    void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach(Button button in buttons)
        {
            string buttonText = button.GetComponentInChildren<Text>().text;
            int value = Int32.Parse(buttonText == "Clear" ? "0": buttonText);
            button.onClick.AddListener(delegate { HandleButtonClick(value); });
        }
        
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && isActive)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, null))
            {
                isActive = false;
                Close();
            }
        }

        // Reposition if it's offscreen
        Reposition();
    }

    private void Close()
    {
        Animator animator = GetComponent<Animator>();
        animator.Play("NumberInputHide");
    }

    public void OnCloseAnimationFinished()
    {
        Destroy(gameObject);
    }

    private void HandleButtonClick(int value)
    {
        handler(value);
        Close();
    }

    private void Reposition()
    {

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        if (corners[0].y < 0)
        {
            transform.Translate(0, -corners[0].y, 0);
        }

        if (corners[0].x < 0)
        {
            transform.Translate(-corners[0].x, 0, 0);
        }

        if (corners[1].y > Screen.height)
        {
            transform.Translate(0, Screen.height - corners[1].y, 0, 0);
        }
    }

    internal void SetHandler(Handler handler)
    {
        this.handler = handler;
    }
}
