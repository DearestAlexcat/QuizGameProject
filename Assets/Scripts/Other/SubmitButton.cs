using UnityEngine;
using UnityEngine.UI;

public class SubmitButton : MonoBehaviour
{
    public enum State { submit, next }

    [Header("Button param")]
    [SerializeField] Color buttonColorNext;
    [SerializeField] Color buttonColorSubmit;

    Text buttonSubmitText;
    Image buttonSubmitImage;
    Button button;

    public State state;

    public State ButtonState
    {
        get
        {
            return state;
        }
        set
        {
            if (value == State.submit)
            {
                buttonSubmitText.text = "Submit";
                buttonSubmitImage.color = buttonColorSubmit;
            }
            else
            {
                buttonSubmitText.text = "Next >";
                buttonSubmitImage.color = buttonColorNext;
            }

            state = value;
        }
    }

    public bool Interactable
    {
        get
        {
            return button.interactable;
        }
        set
        {
            button.interactable = value;
        }
    }

    void Awake()
    {
        buttonSubmitText = GetComponentInChildren<Text>();
        buttonSubmitImage = GetComponent<Image>();
        button = GetComponent<Button>();
        ButtonState = State.submit;
    }

    private void OnEnable()
    {
        button.interactable = false;
    }
}
