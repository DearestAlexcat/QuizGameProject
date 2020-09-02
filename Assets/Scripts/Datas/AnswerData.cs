using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class AnswerData : MonoBehaviour, IPointerDownHandler
{
    [Header("UI Elements")]
    public /*TextMeshProUGUI*/ Text answerText;
    public Image buttonBackground;
    public Color pressColor;
    public Color resetColor;

    [Header("References")]
    [SerializeField] GameEvents events = null;

    public int id;
    private void Start()
    {
        buttonBackground = GetComponent<Image>();
        answerText = GetComponentInChildren<Text>();
    }

    public bool SetBackgroundButton
    {
        set
        {
            if(value)
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
        events.updateQuestionAnswer(this);
    }
}
