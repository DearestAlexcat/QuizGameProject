using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Page2UI : MonoBehaviour, IPageEvents
{
    [Header("References")]
    [SerializeField] GameEvents events = null;
    [SerializeField] RectTransform questionBlockArea;

    [Header("Prefab")]
    public GameObject questionBlockPrefab;

    [Header("UI Section")]
    public TextMeshProUGUI percentText;
    public Image fillAreaQuestion;
    public TextMeshProUGUI questionProgressLabel;
    [Space]
    public Image fillAreaLevels;
    public TextMeshProUGUI levelProgressLabel;

    [Header("Other Сonfiguration")]
    public float progressBlockAnimSpeed;

    int numAllCorrect = 0;
    int numAllQuestion = 0;
    int numQuestionBlocks = 0;
    int currentLevel = 0;

    List<QuestionBlockData> questionBlockButtons = new List<QuestionBlockData>();

    public GameObject getGameObject => this.gameObject;

    void Awake()
    {
        events.updateQuestionBlockUI += QuestionBlockDisplay;
        events.updataProgressUI += ProgressBlockDisplay;
        events.updataBlockQuestionUI += RefreshQuestionBlock;
    }

    void OnApplicationQuit()
    {
        events.updateQuestionBlockUI -= QuestionBlockDisplay;
        events.updataProgressUI -= ProgressBlockDisplay;
        events.updataBlockQuestionUI -= RefreshQuestionBlock;
    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    void ProgressBlockAnim()
    {
        Sequence sequence = DOTween.Sequence();

        float percent = (float)Math.Round(1.0 * numAllCorrect / numAllQuestion, 2);

        fillAreaQuestion.fillAmount = (percent > 0.5f) ? 0.0f : 1.0f; // Анимировать с начала или с конца
        sequence.Insert(0, fillAreaQuestion.DOFillAmount(percent, progressBlockAnimSpeed).SetEase(Ease.InOutCubic));
       
        float endValue = 1.0f * currentLevel / numQuestionBlocks;

        fillAreaLevels.fillAmount = (endValue > 0.5f) ? 0.0f : 1.0f; // Анимировать с начала или с конца
        sequence.Insert(0, fillAreaLevels.DOFillAmount(endValue, progressBlockAnimSpeed).SetEase(Ease.InOutCubic));
       
    }

    void ProgressBlockDisplay(Section section)
    {
        numAllCorrect = section.GetNumCorrectAnswersForAllBlocks();
        numAllQuestion = section.GetNumberAllQuestions();
        numQuestionBlocks = section.questionBlockList.Length;

        // Block 1
        float percent = 1.0f * numAllCorrect / numAllQuestion; //Math.Round(1.0 * numAllCorrect / numAllQuestion, 2);
        percentText.text = ((percent > 0) ? (percent * 100).ToString("#.##") : "0") + "<size=50>%</size>";
        questionProgressLabel.text = $"You have answered {numAllCorrect} out of {numAllQuestion} questions available";

        // Block 2
        if (events.isFinishRound && section.currentLevel < numQuestionBlocks)
        {
            questionBlockButtons[section.currentLevel].Interactable = true;     // view
            section.questionBlockList[section.currentLevel].isBlocked = false;
            section.currentLevel++;
        }

        currentLevel = section.currentLevel;

        levelProgressLabel.text = section.currentLevel + $"<size=50>/{numQuestionBlocks}</size>";

        ProgressBlockAnim();
    }

    void RefreshQuestionBlock(Section section)
    {
        int numCorrect = section.questionBlockList[events.currentQuestionBlockIndex].numberCorrectAnswers;
        int numQuestion = section.questionBlockList[events.currentQuestionBlockIndex].questionList.Length;
        bool isHeaderOn = section.questionBlockList[events.currentQuestionBlockIndex].isHeaderOn;
        
        questionBlockButtons[events.currentQuestionBlockIndex].SetHeaderInfo(numCorrect, numQuestion, isHeaderOn);
    }

    void EraseQuestionBlock()
    {
        foreach (var item in questionBlockButtons)
        {
            item.scrollController.Detach(item.scrollElement);
            Destroy(item.gameObject);
        }

        questionBlockButtons.Clear();
    }
    void QuestionBlockDisplay(QuestionBlock[] questionBlocks)
    {
        EraseQuestionBlock();

        for (int i = 0; i < questionBlocks.Length; i++)
        {
            GameObject obj = Instantiate(questionBlockPrefab, questionBlockArea);
            QuestionBlockData questionBlockData = obj.GetComponent<QuestionBlockData>();
            questionBlockButtons.Add(questionBlockData);
            questionBlockData.SetQuestionBlockData(i, questionBlocks[i].numberCorrectAnswers,
                questionBlocks[i].questionList.Length, questionBlocks[i].isBlocked, questionBlocks[i].isHeaderOn);
        }
    }

    public void EnterPage(Action callback)
    {
        callback?.Invoke();
    }

    public void ExitPage(Action callback)
    {
        callback?.Invoke();
    }
}
