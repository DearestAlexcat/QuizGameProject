using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ViewPage2 : MonoBehaviour, IPageEvents
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
    [SerializeField] RectTransform questionBlockArea;
    [SerializeField] RectTransform _head;

    [Header("Prefab")]
    public QuestionBlockData questionBlockPrefab;

    [Header("UI Section")]
    public TextMeshProUGUI percentText;
    public TextMeshProUGUI questionProgressLabel;
    [Space]
    public TextMeshProUGUI levelProgressLabel;

    int numAllCorrect = 0;
    int numAllQuestion = 0;
    int numQuestionBlocks = 0;

    List<QuestionBlockData> questionBlockButtons = new List<QuestionBlockData>();

    void OnSectionChoisedEventHandler(object sender, SectionChoisedEventArgs e)
    {
        _pagination.GotoNextPage(e.Name, () =>
        {
            bool result = _controller.Execute(ModelOperation.ChoiseQuestionBlock, _model, e); // Запрос на получение данных из модели
            if (result)
            {
                _pagination.SequenceAnim.Kill();
            }
        });
    }

    void EraseQuestionBlock()
    {
        foreach (var item in questionBlockButtons)
        {
            //item.scrollController.Detach(item.scrollElement);
            Destroy(item.gameObject);
        }

        questionBlockButtons.Clear();
    }
    void QuestionBlockDisplay(QuestionBlock[] questionBlocks)
    {
        EraseQuestionBlock();

        QuestionBlockData questionBlockData;
        RectTransform rectQuestionBlock = null;

        float x_pos = _head.rect.width / 2;
        float dx = 0;

        for (int i = 0; i < questionBlocks.Length; i++)
        {
            questionBlockData = Instantiate(questionBlockPrefab, questionBlockArea);
            questionBlockButtons.Add(questionBlockData);
            questionBlockData.SetQuestionBlockData(i, questionBlocks[i].numberCorrectAnswers,
                questionBlocks[i].questionList.Length, questionBlocks[i].isBlocked, questionBlocks[i].isHeaderOn);
            questionBlockData.sectionChoised += OnSectionChoisedEventHandler;

           /* rectQuestionBlock = questionBlockData.GetComponent<RectTransform>();
            dx = (x_pos - rectQuestionBlock.rect.width / 2) + i * rectQuestionBlock.rect.width;
            rectQuestionBlock.anchoredPosition = new Vector3(dx, 0, 1);*/
            
            //questionBlockArea.sizeDelta = new Vector2(-1 * (x_pos - rectQuestionBlock.rect.width / 2.0f - (rectQuestionBlock.rect.width * i)), questionBlockArea.sizeDelta.y);
        }

       // questionBlockArea.sizeDelta = new Vector2(questionBlockArea.sizeDelta.x + x_pos, questionBlockArea.sizeDelta.y);

    }

    // Обновить блок вопроса после прохождения
    void RefreshQuestionBlock()
    {
        QuestionBlock qb = _model.QuestionBlock;

        int numCorrect = qb.numberCorrectAnswers;
        int numQuestion = qb.questionList.Length;
        bool isHeaderOn = qb.isHeaderOn;

        questionBlockButtons[_model.CurrentQuestionBlockIndex].SetHeaderInfo(numCorrect, numQuestion, isHeaderOn);
    }

    void UnlockNextBlockQuestions(Section section)
    {
        if (section.currentLevel < section.questionBlockList.Length)
        {
            questionBlockButtons[section.currentLevel].Interactable = true;     // Индексация начинается с нуля. 1й индекс это 2я страница
            section.questionBlockList[section.currentLevel].isBlocked = false;
            section.currentLevel++;
        }
    }

    void ProgressBlockDisplay(Section section)
    {
        numAllCorrect = section.GetNumCorrectAnswersForAllBlocks();
        numAllQuestion = section.GetNumberAllQuestions();
        numQuestionBlocks = section.questionBlockList.Length;

        // Block 1
        float percent = 1.0f * numAllCorrect / numAllQuestion; 
        percentText.text = ((percent > 0) ? (percent * 100).ToString("#.##") : "0") + "<size=50>%</size>";
        questionProgressLabel.text = $"You have answered {numAllCorrect} out of {numAllQuestion} questions available";

        // Block 2
        levelProgressLabel.text = section.currentLevel + $"<size=50>/{numQuestionBlocks}</size>";

        //ProgressBlockAnim();
    }

    public void EnterPage()
    {
        if (_model != null)
        {
            _model.PropertyChanged += _model_PropertyChanged;
        }

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

    private void _model_PropertyChanged(object sender, NotifyPropertyChangedEventArgs e)
    {
        if(((int)ModelOperation.ChoiseSection & e.Property) != 0) // Отобразить блоки
        {
            QuestionBlockDisplay(_model.QuestionBlocks);
        }

        if (((int)ModelOperation.UnlockNextBlockQ & e.Property) != 0) // Разблокировать блок
        {
            UnlockNextBlockQuestions(_model.Section);
        }

        if (((int)ModelOperation.UpdateSectionStat & e.Property) != 0) // Отобразить статистику выбранной секции
        {
            ProgressBlockDisplay(_model.Section);
        }

        if (((int)ModelOperation.UpdateQuestionBlockStat & e.Property) != 0) // Обновить статистику конкретного блока
        {
            RefreshQuestionBlock();
        }
    }

    void Start()
    {
        _pagination.RegisterPage(1, this);
    }
}
