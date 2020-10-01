using System;
using UnityEngine;

[Serializable]
public class Answer
{
    public string answerText;
    public bool isCorrect;
}

[Serializable]
public class Question
{
    public string questionText;
    public bool useImage;
    public Sprite image;
    public Answer[] answersList;
    public int GetCurrentAnsewer()
    {
        int index = -1;

        for (int i = 0; i < answersList.Length; i++)
        {
            if (answersList[i].isCorrect)
            {
                index = i;
                break;
            }
        }

        return index;
    }
}

[Serializable]
public class QuestionBlock
{
    public bool isBlocked;
    public bool isHeaderOn;
    public int numberCorrectAnswers;
    public Question[] questionList;
}

[CreateAssetMenu(fileName = "Question Section", menuName = "Quiz/Create section block")]
//[Serializable]
public class Section : ScriptableObject
{
    public string sectionText;     // Отображается на первой странице
    public int currentLevel;       // Отображает прогресс открытых блоков вопросов
    public QuestionBlock[] questionBlockList;

    public int GetNumCorrectAnswersForAllBlocks()
    {
        int count = 0;

        for (int i = 0; i < questionBlockList.Length; i++)
        {
            count += questionBlockList[i].numberCorrectAnswers;
        }

        return count;
    }

    public int GetNumberAllQuestions()
    {
        int count = 0;
        for (int i = 0; i < questionBlockList.Length; i++)
        {
            count += questionBlockList[i].questionList.Length;
        }
        return count;
    }
}
