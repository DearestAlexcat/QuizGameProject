using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionBlockData : MonoBehaviour
{
    public int id { get; set; }

    [Header("References")]
    [SerializeField] GameEvents events = null;

    [Header("UI Elements")]
    public Text blockName = null;
    public TextMeshProUGUI record = null;
    public TextMeshProUGUI textState = null;
    public RectTransform header;
    public Image colorState = null;
    public CanvasGroup canvasGroup = null;
    public Color PassedColor;  
    public Color FailedColor;

    public ScrollController scrollController { get; private set; } = null;
    public ScrollElement scrollElement { get; private set; } = null;

    private void Start()
    {
        scrollController = GetComponentInParent<ScrollController>();

        if (scrollController != null)
        {
            scrollElement = new ScrollElement { cg = canvasGroup, transform = gameObject.transform };
            scrollController.Attach(scrollElement);
        }
        else
        {
            Debug.LogError("ScrollElement.Start. ScrollController is null");
        }
    }

    private void OnDestroy()
    {
        scrollController.Detach(scrollElement);
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
            canvasGroup.interactable = value;
        }
    }

    public void SetQuestionBlockData(int id, int numberCorrectAnswers, int numberQuestion, bool isBlocked, bool isHeaderOn)
    {
        this.id = id;
        blockName.text = (id + 1).ToString();

        SetHeaderInfo(numberCorrectAnswers, numberQuestion, isHeaderOn);

        canvasGroup.interactable = !isBlocked; 
    }

    public void SetSectionIndex()  // Событие OnClick
    {
        events.currentQuestionBlockIndex = id;
        events.resetQuestionElement();
        bool isSelect = events.selectQuestion();

        if(isSelect)
        {
            events.displayNextPage(blockName.text);
        }
    }
}
