using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnswerData : MonoBehaviour, IPointerDownHandler
{
    [Header("UI Elements")]
    public Text answerText;
    public Image buttonBackground;
    public Color pressColor;
    public Color resetColor;

    public event SectionChoisedEventHandler ansverChoised;
    public int id;

    public bool SetBackgroundColor
    {
        set
        {
            if (value)
            {
                buttonBackground.color = Color.red;
            }
            else
            {
                buttonBackground.color = Color.green;
            }
        }
    }

    public bool SetBackgroundButton
    {
        set
        {
            if (value)
            {
                buttonBackground.color = pressColor;
            }
            else
            {
                buttonBackground.color = resetColor;
            }
        }
    }

    public void SetData(string text, int id)
    {
        this.answerText.text = text;
        this.id = id;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ansverChoised.Invoke(this, new SectionChoisedEventArgs(id, null));
    }
}
