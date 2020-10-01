using UnityEngine;
using UnityEngine.UI;

public delegate void SectionChoisedEventHandler(object sender, SectionChoisedEventArgs e);

public class SectionChoisedEventArgs : System.EventArgs
{
    public SectionChoisedEventArgs(int indexPage, string pageName)
    {
        Index = indexPage;
        Name = pageName;
    }

    public virtual string Name { get; }
    public virtual int Index { get; }
}

public class SectionData : MonoBehaviour
{
    [Header("UI Elements")]
    public Text sectionName;
    public Text numberQuestion;

    public int id { get; private set; }

    public event SectionChoisedEventHandler sectionChoised;

    public void SetSectionData(Section section, int id)
    {
        this.id = id;
        sectionName.text = section.sectionText;
        numberQuestion.text = section.GetNumberAllQuestions() + " questions";
    }

    public void SectionChoised() // Событие OnClick
    {
        sectionChoised?.Invoke(this, new SectionChoisedEventArgs(id, sectionName.text));
    }
}
