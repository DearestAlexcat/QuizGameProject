using UnityEngine;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Quiz/Create game events")]
public class GameEvents : ScriptableObject
{
    //Section callback
    public delegate void SectionUICallback(Section[] sections);
    public SectionUICallback sectionUI = null;

    //Question callbacks
    public delegate bool SelectQuestionBlockUI();
    public SelectQuestionBlockUI selectQuestionBlock = null;
    public delegate void UpdateQuestionBlockUICallback(QuestionBlock[] questionBlocks);
    public UpdateQuestionBlockUICallback updateQuestionBlockUI = null;

    public delegate void UpdataProgressUICallback(Section section);
    public UpdataProgressUICallback updataProgressUI = null;
    public delegate void UpdataBlockQuestionUICallback(Section section);
    public UpdataBlockQuestionUICallback updataBlockQuestionUI = null;


    public delegate bool SelectQuestionUI();
    public SelectQuestionUI selectQuestion = null;
    public delegate void UpdateQuestionUICallback(Question question, int numQuestionsPassed, int numQuestion);
    public UpdateQuestionUICallback updateQuestionUI = null;

    //Pages callbacks
    public delegate void DisplayPreviousPageCallback();
    public DisplayPreviousPageCallback displayPreviousPage = null;
    public delegate void DisplayNextPageCallback(string pageName);
    public DisplayNextPageCallback displayNextPage = null;


    public delegate void ConfirmAnswerCallback(params int[] buttons);
    public ConfirmAnswerCallback confirmAnswer = null;

    public delegate void UpdateQuestionAnswerCallback(AnswerData pickedAnswer);
    public UpdateQuestionAnswerCallback updateQuestionAnswer = null;


    public delegate void ResetQuestionElementCallback();
    public ResetQuestionElementCallback resetQuestionElement = null;


    [HideInInspector]
    public int currentQuestionBlockIndex;
    [HideInInspector]
    public int currentSectionIndex;
    [HideInInspector]
    public bool isFinishRound;
}
