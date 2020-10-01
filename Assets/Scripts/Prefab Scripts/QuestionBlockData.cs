using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionBlockData : MonoBehaviour
{
    public int id { get; set; }

    [Header("UI Elements")]
    public Text blockName = null;
    public TextMeshProUGUI record = null;
    public TextMeshProUGUI textState = null;
    public Button buttonStart = null;
    public RectTransform header = null;
    public Image colorState = null;

    public Color PassedColor;
    public Color FailedColor;

    public event SectionChoisedEventHandler sectionChoised;

    public ScrollController scrollController { get; private set; } = null;

    public ScrollElement scrollElement { get; private set; } = null;

    private void Start()
    {
        scrollController = GetComponentInParent<ScrollController>();

        if (scrollController != null)
        {
            scrollElement = new ScrollElement { bt = buttonStart, transform = gameObject.transform };
            scrollController.Attach(scrollElement);
        }
        else
        {
            Debug.LogError("ScrollElement.Start. ScrollController is null");
        }
    }

    private void OnDestroy()
    {
        //scrollController.Detach(scrollElement);
    }

    public void SetHeaderInfo(int numberCorrectAnswers, int numberQuestion, bool isHeaderOn)
    {
        header.gameObject.SetActive(isHeaderOn);

        if (numberCorrectAnswers == numberQuestion)
        {
            textState.text = "Passed";
            colorState.color = PassedColor;
        }
        else
        {
            textState.text = "Failed";
            colorState.color = FailedColor;
        }

        record.text = numberCorrectAnswers + " of " + numberQuestion;
    }

    public bool Interactable
    {
        set
        {
            buttonStart.interactable = value;
        }
    }

    public void SetQuestionBlockData(int id, int numberCorrectAnswers, int numberQuestion, bool isBlocked, bool isHeaderOn)
    {
        this.id = id;
        blockName.text = (id + 1).ToString();

        SetHeaderInfo(numberCorrectAnswers, numberQuestion, isHeaderOn);
        
        buttonStart.interactable = !isBlocked;
        //canvasGroup.interactable = !isBlocked;
    }

    public void SetQuestionBlockIndex()  // Событие OnClick
    {
        sectionChoised?.Invoke(this, new SectionChoisedEventArgs(id, blockName.text));
    }
}
