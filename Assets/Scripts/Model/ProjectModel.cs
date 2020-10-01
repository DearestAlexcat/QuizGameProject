using System.Collections.Generic;
using UnityEngine;

public delegate void ModelPropertyChangedEventHandler(object sender, NotifyPropertyChangedEventArgs e);

public class NotifyPropertyChangedEventArgs : System.EventArgs
{
    public NotifyPropertyChangedEventArgs(int property)
    {
        Property = property;
    }

    public virtual int Property { get; }
}

public class ProjectModel : MonoBehaviour
{
    Question[] _questions;            // Вопросы определенного блока

    int _currentSectionIndex;
    int _currentQuestionBlockIndex;

    int _questionIndex;
    
    // Список пройденных вопросов
    List<int> _questionsPassed = new List<int>();

    public int QuestionsPassedCount => _questionsPassed.Count;
    public int QuestionsCount => _questions.Length;

    bool isFinished => _questionsPassed.Count < _questions.Length;

    // Вернуть секции/секцию блоков вопросов
    public Section[] Sections { get; private set; }
    public Section Section => Sections[_currentSectionIndex];

    // Вернуть блоки / блок вопросов для конкретной секции
    public QuestionBlock[] QuestionBlocks => Sections[_currentSectionIndex].questionBlockList;
    public QuestionBlock QuestionBlock => Sections[_currentSectionIndex].questionBlockList[_currentQuestionBlockIndex];

    // Вернуть рандомный вопрос
    public Question GetNextQuestion => GetRandomQuestion();  // need added event
    
    private Question GetRandomQuestion()
    {
        Question q = null;

        if (isFinished && _questions.Length > 0)
        {
            _questionIndex = GetRandomIndex();
            q = _questions[_questionIndex];
        }
            
        return q;
    }

    private int GetRandomIndex()
    {
        int index;
       
        do
        {
            index = UnityEngine.Random.Range(0, _questions.Length);
        } while (_questionsPassed.Contains(index));

        _questionsPassed.Add(index);

        return index;
    }

    public QuestionBlock QuestionBlockStat
    {
        set
        {
            QuestionBlock qb = value;
            QuestionBlock.isHeaderOn = qb.isHeaderOn;
            QuestionBlock.numberCorrectAnswers = qb.numberCorrectAnswers;
            OnPropertyChanged((int)ModelOperation.UpdateQuestionBlockStat | (int)ModelOperation.UpdateSectionStat | (int)ModelOperation.UnlockNextBlockQ);
        }
    }

    public int CurrentQuestionBlockIndex
    {
        set
        {
            _questionIndex = 0;
            _questionsPassed.Clear();
            _currentQuestionBlockIndex = value;
            _questions = Sections[_currentSectionIndex].questionBlockList[_currentQuestionBlockIndex].questionList;

            OnPropertyChanged((int)ModelOperation.ChoiseQuestionBlock);
        }
        get
        {
            return _currentQuestionBlockIndex;
        }
    }

    public int CurrentSectionIndex
    {
        set
        {
            _currentSectionIndex = value;
            OnPropertyChanged((int)ModelOperation.ChoiseSection | (int)ModelOperation.UpdateSectionStat | (int)ModelOperation.UpdateQuestionBlockStat | (int)ModelOperation.UnlockNextBlockQ); // отображаем блоки выбранной секции и обновляем статистику
        }
    }

    void Load()
    {
        Sections = Resources.LoadAll<Section>("Sections"); // Нужна обработка ошибок загрузки, обработка нулевых ссылок во всем приложении!!!
        
        if(Sections == null)
        {
            Debug.LogWarning("Sections is null");
        }

    }
    
    void Awake()
    {
        Load();
    }

    public event ModelPropertyChangedEventHandler PropertyChanged;
   
    public void OnPropertyChanged(int property)
    {
        PropertyChanged?.Invoke(this, new NotifyPropertyChangedEventArgs(property));
    }
}
