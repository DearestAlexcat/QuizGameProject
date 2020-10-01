using UnityEngine;

public enum ModelOperation : int
{
    // 2 ст
    ChoiseSection = 1,
    UpdateSectionStat = 16,
    UpdateQuestionBlockStat = 8,
    UnlockNextBlockQ = 32,

    // 3 ст
    ChoiseQuestionBlock = 2,
    GetQuestion = 4,
}

public interface IPageEvents
{
    void EnterPage();
    void ExitPage();
}

public class GameController 
{

    // Запоминаем прошлый выбор чтобы не грузить одни и те же данные
    // int prevSectionIndex = -1;
    // int questionBLockIndex = -1;

    public bool Execute(ModelOperation operation, ProjectModel model, QuestionBlock attribute)
    {
        if (model == null)
        {
            Debug.Log("Model is null");
            return false;
        }

        switch (operation)
        {
            case ModelOperation.UpdateQuestionBlockStat:
                model.QuestionBlockStat = attribute;
                break;
            default: //Message Box
                break;
        }

        return true;
    }

    public bool Execute(ModelOperation operation, ProjectModel model, SectionChoisedEventArgs attribute)
    {
        if(model == null)
        {
            Debug.Log("Model is null");
            return false;
        }

        //TODO: Перед установкой индексов модель должна запросить данные из сервера

        switch (operation)
        {
            case ModelOperation.ChoiseSection:
                model.CurrentSectionIndex = attribute.Index;
                break;
            case ModelOperation.ChoiseQuestionBlock:
                model.CurrentQuestionBlockIndex = attribute.Index;
                break;
            case ModelOperation.GetQuestion:
                break;
            default: //Message Box
                break;
        }

        return true;
    }
}
