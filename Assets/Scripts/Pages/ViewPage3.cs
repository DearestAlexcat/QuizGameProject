using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewPage3 : MonoBehaviour, IPageEvents
{
    [Header("Links")]
    [SerializeField]
    ProjectModel _model;
    [SerializeField]
    Pagination _pagination;
    [SerializeField]
    Canvas _parentCanvas;

    GameController _controller = new GameController();

    [Header("References")]
    [SerializeField] RectTransform answersOptionsArea;
    [SerializeField] CanvasGroup answersAreaCG;
    [SerializeField] SubmitButton submitButton;
    [SerializeField] CanvasGroup holderCG;
    [SerializeField] RectTransform _head;

    [Header("Prefabs")]
    public AnswerData answerPrefab;
    AnswerData pickedAnswer;

    //public event SectionChoisedEventHandler sectionChoised;

    [Header("UI Section")]
    [SerializeField] Text questionText;
    [SerializeField] Image questionImage;
    [SerializeField] Scrollbar progressBar;
    [SerializeField] RectTransform submitButtonPos;
    [SerializeField] RectTransform content;

    [Header("Buttons Param")]
    [SerializeField] Color badRedColor;
    [SerializeField] Color gutGreenColor;

    [Header("Other Сonfiguration")]
    public float displayAnswersSpeed;
    public float buttonSpace;

    List<AnswerData> answerButtons = new List<AnswerData>();

    int[] randomAnswerIndexes;
    int _numberCorrectAnswers;

    Question currentQuestion;

    private void _model_PropertyChanged(object sender, NotifyPropertyChangedEventArgs e)
    {
        currentQuestion = _model.GetNextQuestion;

        if(currentQuestion != null)
        {
            QuestionDisplay(currentQuestion);
        }
        else
        {
            _pagination.GotoPreviousPage();

            QuestionBlock questionBlock = new QuestionBlock();
            questionBlock.numberCorrectAnswers = _numberCorrectAnswers;
            questionBlock.isHeaderOn = true;

            _controller.Execute(ModelOperation.UpdateQuestionBlockStat, _model, questionBlock);
        }
    }

    public void EnterPage()
    {
        if (_model != null)
        {
            _model.PropertyChanged += _model_PropertyChanged;   
        }

        _numberCorrectAnswers = 0;
        _parentCanvas.enabled = true;
    }

    public void ExitPage()
    {
        if (_model != null)
        {
            _model.PropertyChanged -= _model_PropertyChanged;
        }

        _parentCanvas.enabled = false;
    }


    void QuestionDisplay(Question question)
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

        ProgressBarUpdate();
        StartCoroutine(GenerateAnswers(question));
    }

    void ProgressBarUpdate()
    {
        if (_model.QuestionsCount != 0)
        {
            progressBar.size = 1.0f * _model.QuestionsPassedCount / _model.QuestionsCount;
        }
        else
        {
            //nameof()
            Debug.LogError("Page3UI.ProgressBarUpdate: Division by zero");
        }
    }
    
    IEnumerator GenerateAnswers(Question question)
    {
        EraseAnswer();

        int index, i, length;
        length = question.answersList.Length;

        if (length == 0)
        {
            throw new System.Exception("The question does not contain answers");
        }

        randomAnswerIndexes = GetRandomAnswerIndexes(length);

        AnswerData obj = null;
        RectTransform objRect = null;

        content.sizeDelta = new Vector2(_head.rect.width, 0);

        for (i = 0; i < length; i++)
        {
            obj = Instantiate(answerPrefab, answersOptionsArea);
            answerButtons.Add(obj);
            index = randomAnswerIndexes[i];
            obj.SetData(question.answersList[index].answerText, index);
            obj.ansverChoised += UpdateAnswers;
        }

        yield return null; 

        for (i = 0; i < answerButtons.Count; i++)
        {
            objRect = answerButtons[i].GetComponent<RectTransform>();
            objRect.anchoredPosition = new Vector2(objRect.anchoredPosition.x, -i * (objRect.rect.height + buttonSpace));
            content.sizeDelta = new Vector2(content.sizeDelta.x, (i + 1) * (objRect.rect.height + buttonSpace));
            submitButtonPos.anchoredPosition = new Vector2(submitButtonPos.anchoredPosition.x, -(i + 1) * (objRect.rect.height + buttonSpace));
        }

        content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y + buttonSpace + submitButtonPos.rect.height);
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

    public void ButtonSubmitHandler() // Событие OnClick
    {
        if (submitButton.ButtonState == SubmitButton.State.next)
        {           
            submitButton.Interactable = false;
            submitButton.ButtonState = SubmitButton.State.submit;                
           
            //_controller.Execute(ModelOperation.GetQuestion, _model, null);
            _model_PropertyChanged(this, null); // ???? // Получить след. вопрос
        }
        else if (submitButton.Interactable) // Кнопка может быть Submit, но не Interactable
        {
            int answerIndex = currentQuestion.GetCurrentAnsewer();
            submitButton.ButtonState = SubmitButton.State.next;

            if (pickedAnswer.id == answerIndex) // Если корректно
            {
                _numberCorrectAnswers++;
                ConfirmAnswer(pickedAnswer.id);
            }
            else
            {
                ConfirmAnswer(answerIndex, pickedAnswer.id);
            }
        }
    }

    public void UpdateAnswers(object sender, SectionChoisedEventArgs e) // событие IPointerDownHandler, вызывается из класса AnswerData
    {
        submitButton.Interactable = true; // Разрешаем подтвердить ответ

        if (pickedAnswer != null)
        {
            pickedAnswer.answerText.color = Color.black;
            pickedAnswer.SetBackgroundButton = false;
        }

        pickedAnswer = sender as AnswerData;

        if(pickedAnswer != null)
        {
            pickedAnswer.answerText.color = Color.white;
            pickedAnswer.SetBackgroundButton = true;
        }
        else
        {
            Debug.Log("ViewPage3. UpdateAnswers: Сonversion error to AnswerData");
        }
    }

    void ConfirmAnswer(params int[] buttons)
    {
        answersAreaCG.blocksRaycasts = false; // после подтверждения варианты ответов блокируются

        AnswerData buttonGreen = null;
        AnswerData buttonRed = null;

        // Получить правильный индекс
        if (buttons[0] > -1)
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
            //messageBox.Show("No answer is set for this question", MessageBox.MessageBoxButtons.Ok);
        }

        holderCG.blocksRaycasts = false; // пока идут анимации, интерфейс недоступен
        //buttonRed?.buttonBackground.DOColor(badRedColor, displayAnswersSpeed);

        if(buttonRed != null)
        buttonRed.SetBackgroundColor = true;

        if (buttonGreen != null)
        {
            //buttonGreen.buttonBackground.DOColor(gutGreenColor, displayAnswersSpeed).OnComplete(() => {
            //    page3CG.blocksRaycasts = true;
            //});
            buttonGreen.SetBackgroundColor = false;
            holderCG.blocksRaycasts = true;
        }
        else
        {
            holderCG.blocksRaycasts = true;
        }
    }

    void Start()
    {
        //content.sizeDelta = new Vector2(_head.rect.width, 0);
        _pagination.RegisterPage(2, this);
    }
}
