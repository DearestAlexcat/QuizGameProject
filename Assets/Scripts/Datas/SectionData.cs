using System;
using UnityEngine;
using UnityEngine.UI;

public class SectionData : MonoBehaviour
{  
    [Header("References")]
    [SerializeField] GameEvents events;

    [Header("UI Elements")]
    public Text sectionName;
    public Text numberQuestion;

    public int id { get; private set; }

    public void SetSectionData(Section section, int id)
    {
        this.id = id;
        sectionName.text = section.sectionText;
        numberQuestion.text = section.GetNumberAllQuestions() + " questions";
    }


    public void SetSectionIndex() // Событие OnClick
    {
        events.isFinishRound = false; // ??? 
        events.currentSectionIndex = id;
        bool isSelect = events.selectQuestionBlock(); // генерация блоков вопросов определенной секции

        if(isSelect)
        {
           events.displayNextPage(sectionName.text);
        }
    }
}
