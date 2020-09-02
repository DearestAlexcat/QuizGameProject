using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Section[] sections;
    QuestionBlock[] questionBlocks;  // Блоки вопросов
    Question[] questions;            // Вопросы определенного блока
    
    [SerializeField] MessageBox messageBox = null;

    int questionIndex;
    int numberCorrectAnswers;

    List<int> questionsPassed = new List<int>(); // Список пройденных вопросов
    [SerializeField] GameEvents events = null;
    bool isFinished => questionsPassed.Count < questions.Length;
 
    public SubmitButton submitButton;
    AnswerData pickedAnswer = null;

    GameData gameData = null;
    private void Load()
    {
        gameData = DataController.LoadData();
        
        if(gameData == null)
        {
            messageBox.Show("Failed to load data", MessageBox.MessageBoxButtons.Ok);
            return;
        }
        
        sections = gameData.allSectionData;    // Получаем загруженные данные из файла

        if(sections.Length == 0)
        {
            Debug.LogError("Sections is empty");
            return;
        }

        if (events.sectionUI == null)
        {
            Debug.LogError("GameEvents.sectionDisplay is null");
        }
        else
        {
            events.sectionUI(sections); // Генерируем все секции
        }
    }

    bool SelectQuestionBlockDisplay()
    {
        int sectionIndex = events.currentSectionIndex;
        questionBlocks = sections[sectionIndex].questionBlockList; // По индексу получаем список блоков вопросов выбранной секции

        if (questionBlocks.Length == 0 || sections[sectionIndex].GetNumberAllQuestions() == 0 || events.sectionUI == null)
        {
            Debug.LogError("GameEvents.QuestionBlockDisplay. Data is null");
            return false;
        }
        
        events.updateQuestionBlockUI(questionBlocks);                   // Отобразить блоки с вопросами выбранной секции 
        events.updataProgressUI(sections[events.currentSectionIndex]);  // Отобразить прогресс выбранной секции

        return true;
    }

    void ResetQuestionElement()
    {
        questionsPassed.Clear();
        numberCorrectAnswers = 0;
        submitButton.Interactable = false;
        submitButton.ButtonState = SubmitButton.State.submit;
    }

    bool SelectQuestionDisplay()
    {
        int index = events.currentQuestionBlockIndex;
        questions = questionBlocks[index].questionList;
        bool selected = true;

        try
        {
            if (questions.Length == 0)
            {             
                throw new System.Exception("This block contains no questions");
            }

            var question = GetRandomQuestion();

            if (events.sectionUI == null)
            {
                throw new System.Exception("GameEvents.QuestionBlockDisplay is null");
            }

            events.updateQuestionUI(question, questionsPassed.Count, questions.Length);
        }
        catch (System.Exception ex)
        {
            selected = false;
            messageBox.Show("This block has an incomplete structure. Fill in all the question fields for the correct operation.", MessageBox.MessageBoxButtons.Ok);
            Debug.LogError(ex.Message);
        }

        return selected;
    }

    private Question GetRandomQuestion()
    {
        questionIndex = GetRandomIndex();
        return questions[questionIndex];
    }

    private int GetRandomIndex()
    {
        int index = 0;

        if (isFinished)
        {
            do
            {
                index = UnityEngine.Random.Range(0, questions.Length);
            } while (questionsPassed.Contains(index));
        }

        return index;
    }

    void OnEnable()
    {
        events.selectQuestionBlock += SelectQuestionBlockDisplay;
        events.selectQuestion += SelectQuestionDisplay;
        events.updateQuestionAnswer += UpdateAnswers;
        events.resetQuestionElement += ResetQuestionElement;
    }

    void OnDisable()
    {
        events.selectQuestionBlock -= SelectQuestionBlockDisplay;
        events.selectQuestion -= SelectQuestionDisplay;
        events.updateQuestionAnswer -= UpdateAnswers;
        events.resetQuestionElement -= ResetQuestionElement;
    }

    //private void OnApplicationQuit()
    //{
    //    if (gameData != null)
    //    {
    //        DataController.SaveData(gameData);
    //    }
    //}

    private void OnApplicationPause()
    {
        if(gameData != null)
        {
            DataController.SaveData(gameData);
        }
    }

    //!!!
    void ButtonSubmitHandler() // Событие OnClick
    {
        if(submitButton.ButtonState == SubmitButton.State.next)
        {
            questionsPassed.Add(questionIndex);                     // помечаем вопрос как пройденный
            
            #region Следующий вопрос или возврат в меню?
            if (!isFinished)
            {
                questionBlocks[events.currentQuestionBlockIndex].numberCorrectAnswers = numberCorrectAnswers;
                questionBlocks[events.currentQuestionBlockIndex].isHeaderOn = true;

                events.isFinishRound = true;

                events.updataProgressUI(sections[events.currentSectionIndex]);
                events.updataBlockQuestionUI(sections[events.currentSectionIndex]);

                numberCorrectAnswers = 0;
                questionsPassed.Clear();
                events.displayPreviousPage();
                events.isFinishRound = false;
            }
            else
            {
                SelectQuestionDisplay();
            }

            submitButton.Interactable = false;
            submitButton.ButtonState = SubmitButton.State.submit;
            #endregion
        }
        else if(submitButton.Interactable)
        {

            //submitButton.Interactable = false; 

            // Подготовка элементов для анимации выбранного варианта ответа

            int answerIndex = questions[questionIndex].GetCurrentAnsewer();

            if (events.confirmAnswer == null)
            {
                Debug.LogWarning("GameEvents.confirmAnswer is null");
                return;
            }

            submitButton.ButtonState = SubmitButton.State.next;

            if (pickedAnswer.id == answerIndex) // Если выбрали корректный вариант
            {
                //Увеличить кол-во правильных ответов
                numberCorrectAnswers++;
                events.confirmAnswer(pickedAnswer.id); 
            }
            else
            {
                events.confirmAnswer(answerIndex, pickedAnswer.id); 
            }
        }
    }

    public void UpdateAnswers(AnswerData answerButton) // событие IPointerDownHandler, вызывается из класса AnswerData
    {
        submitButton.Interactable = true; // Разрешаем подтвердить ответ

        if (pickedAnswer != null)
        {
            pickedAnswer.answerText.color = Color.black;
            pickedAnswer.SetBackgroundButton = false;
        }

        pickedAnswer = answerButton;
        pickedAnswer.answerText.color = Color.white;
        pickedAnswer.SetBackgroundButton = true;
    }

    void Start()
    {
        SetupAndroidTheme(ToARGB(Color.black), ToARGB(new Color(18.0f / 255.0f, 23.0f / 255.0f, 26.0f / 255.0f, 1.0f)));
        var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        UnityEngine.Random.InitState(seed);
        Load();
    }

    public static void SetupAndroidTheme(int primaryARGB, int darkARGB, string label = null)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
                label = label ?? Application.productName;
                Screen.fullScreen = false;
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaClass layoutParamsClass = new AndroidJavaClass("android.view.WindowManager$LayoutParams");
                    int flagFullscreen = layoutParamsClass.GetStatic<int>("FLAG_FULLSCREEN");
                    int flagNotFullscreen = layoutParamsClass.GetStatic<int>("FLAG_FORCE_NOT_FULLSCREEN");
                    int flagDrawsSystemBarBackgrounds = layoutParamsClass.GetStatic<int>("FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS");
                    AndroidJavaObject windowObject = activity.Call<AndroidJavaObject>("getWindow");
                    windowObject.Call("clearFlags", flagFullscreen);
                    windowObject.Call("addFlags", flagNotFullscreen);
                    windowObject.Call("addFlags", flagDrawsSystemBarBackgrounds);
                    int sdkInt = new AndroidJavaClass("android.os.Build$VERSION").GetStatic<int>("SDK_INT");
                    int lollipop = 21;
                    //if (sdkInt > lollipop)
                    {
                        windowObject.Call("setStatusBarColor", darkARGB);
                        string myName = activity.Call<string>("getPackageName");
                        AndroidJavaObject packageManager = activity.Call<AndroidJavaObject>("getPackageManager");
                        AndroidJavaObject drawable = packageManager.Call<AndroidJavaObject>("getApplicationIcon", myName);
                        AndroidJavaObject taskDescription = new AndroidJavaObject("android.app.ActivityManager$TaskDescription", label, drawable.Call<AndroidJavaObject>("getBitmap"), primaryARGB);
                        activity.Call("setTaskDescription", taskDescription);
                    }
                }));
            #endif
    }

    public static int ToARGB(Color color)
    {
        Color32 c = (Color32)color;
        byte[] b = new byte[] { c.b, c.g, c.r, c.a };
        return System.BitConverter.ToInt32(b, 0);
    }

}
