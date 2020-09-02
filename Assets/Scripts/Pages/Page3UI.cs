using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Page3UI : MonoBehaviour, IPageEvents
{

    #region Variables

    [Header("References")]
    [SerializeField] GameEvents events = null;
    [SerializeField] RectTransform answersOptionsArea;
    [SerializeField] CanvasGroup answersAreaCG;
    [SerializeField] Button submitButton;

    CanvasGroup page3CG;

    [Header("Prefabs")]
    public AnswerData answerPrefab;

    [Header("UI Section")]
    [SerializeField] Text questionText;
    [SerializeField] Image questionImage;
    [SerializeField] Scrollbar progressBar;

    [Header("Buttons Param")]
    [SerializeField] Color badRedColor;
    [SerializeField] Color gutGreenColor;

    [Header("Other Сonfiguration")]
    public float displayAnswersSpeed;

    List<AnswerData> answerButtons = new List<AnswerData>();
    int[] randomAnswerIndexes;

    [SerializeField] MessageBox messageBox = null;
    #endregion

    public GameObject getGameObject => this.gameObject;

    public void EnterPage(Action callback) 
    {
        callback?.Invoke();
    }

    public void ExitPage(Action callback) 
    {
        if(!events.isFinishRound) // Если раунд не завершен, предупреждаем пользователя о потере данных
        {
            messageBox.Show("You haven't finished the current exam, are you sure you want to leave this page?",
            MessageBox.MessageBoxButtons.YesNo, callback);
        }
        else
        {
            callback?.Invoke();
        }
    }

    void Awake()
    {
        page3CG = GetComponent<CanvasGroup>();
        events.updateQuestionUI += QuestionDisplay;
        events.confirmAnswer += ConfirmAnswer;
    }

    void OnApplicationQuit()
    {
        events.updateQuestionUI -= QuestionDisplay;
        events.confirmAnswer -= ConfirmAnswer;
    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    void ProgressBarUpdate(int numQuestionsPassed, int numQuestion)
    {
        if(numQuestion != 0)
        {
            progressBar.size = 1.0f * numQuestionsPassed / numQuestion;
        }
        else
        {
            //nameof()
            Debug.LogError("Page3UI.ProgressBarUpdate: Division by zero");
        }
    }

    void QuestionDisplay(Question question, int numQuestionsPassed, int numQuestion)
    {
        questionText.text = question.questionText;

        if (question.useImage)
        {
            questionImage.gameObject.SetActive(true);
            questionImage.sprite = question.image;
        }
        else
        {
            questionImage.gameObject.SetActive(false);
        }

        answersAreaCG.blocksRaycasts = true;           // разблокировка новых вариантов ответов

        ProgressBarUpdate(numQuestionsPassed, numQuestion);
        GenerateAnswers(question);
    }

    void ConfirmAnswer(params int[] buttons)
    {
        answersAreaCG.blocksRaycasts = false; // после подтверждения варианты ответов блокируются

        AnswerData buttonGreen = null;
        AnswerData buttonRed = null;

        // Получить правильный индекс

        if(buttons[0] > -1)
        {
            for (int i = 0; i < answerButtons.Count; i++)
            {
                if (answerButtons[i].id == buttons[0])
                {
                    buttonGreen = answerButtons[i];
                    buttonGreen.answerText.color = Color.white;
                }
                if (buttons.Length > 1 && answerButtons[i].id == buttons[1])
                {
                    buttonRed = answerButtons[i];
                }
            }
        }
        else
        {
            messageBox.Show("No answer is set for this question", MessageBox.MessageBoxButtons.Ok);
        }

        page3CG.blocksRaycasts = false; // пока идут анимации, интерфейс недоступен

        buttonRed?.buttonBackground.DOColor(badRedColor, displayAnswersSpeed);

        if(buttonGreen != null)
        {
            buttonGreen.buttonBackground.DOColor(gutGreenColor, displayAnswersSpeed).OnComplete(() => {
                page3CG.blocksRaycasts = true;
                //submitButton.interactable = true;
            });
        }
        else
        {
            page3CG.blocksRaycasts = true;
            //submitButton.interactable = true;
        }
    }

    void EraseAnswer()
    {
        foreach (var item in answerButtons)
        {
            Destroy(item.gameObject);
        }

        answerButtons.Clear();
    }

    int[] GetRandomAnswerIndexes(int answersLength)
    {
        int index, temp, i;

        int[] randomIndexes = new int[answersLength];

        for (i = 0; i < answersLength; i++)
        {
            randomIndexes[i] = i;
        }

        for (i = 0; i < answersLength - 1; i++)
        {
            index = UnityEngine.Random.Range(i + 1, answersLength);
            temp = randomIndexes[i];
            randomIndexes[i] = randomIndexes[index];
            randomIndexes[index] = temp;
        }

        return randomIndexes;
    }

    void GenerateAnswers(Question question)
    {
        EraseAnswer();

        int index, i, length;
        length = question.answersList.Length;

        if (length == 0)
        {
            throw new Exception("The question does not contain answers");
        }

        randomAnswerIndexes = GetRandomAnswerIndexes(length);

        for (i = 0; i < length; i++)
        {
            AnswerData obj = Instantiate(answerPrefab, answersOptionsArea);
            answerButtons.Add(obj);
            index = randomAnswerIndexes[i];
            obj.SetData(question.answersList[index].answerText, index);
        }
    }
}
