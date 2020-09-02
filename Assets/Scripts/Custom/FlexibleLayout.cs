using UnityEngine.UI;

public class FlexibleLayout : LayoutGroup
{

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        if (rectChildren.Count == 0)
        {
            return;
        }

        LayoutElement[] layoutElements = new LayoutElement[rectChildren.Count];

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;
        float occupiedHeight = 0;   // Занимаемая высота
        int count = 0;              // Кол-во элементов которые займут оставшееся пространство

        for (int i = 0; i < rectChildren.Count; i++)
        {
            var item = rectChildren[i];

            if (item.gameObject.activeSelf)
            {
                ContentSizeFitter fitter = item.GetComponent<ContentSizeFitter>(); // ContentSizeFitter запаздывает, поэтому вызываем эго преждевременно
                fitter?.SetLayoutVertical();

                var element = item.gameObject.GetComponent<LayoutElement>();
                layoutElements[i] = element;

                if (item.gameObject.activeSelf && layoutElements[i] != null)
                {
                    if (layoutElements[i].minHeight == -1) // -1 значит для элемента не установлена фиксированная высота
                    {
                        count++;
                    }
                    else
                    {
                        occupiedHeight += rectChildren[i].rect.height <= layoutElements[i].minHeight ? layoutElements[i].minHeight : rectChildren[i].rect.height;
                    }
                }
            }
        }

        // Рассчитываем высоту для элементов, ктр займут оставшееся пространство
        // (rectChildren.Count * padding.top) - высота всех отступов сверху

        float resHeight = (parentHeight - occupiedHeight - (rectChildren.Count * padding.top)) / count;
        float size;
        float yPos = padding.top;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            var item = rectChildren[i];

            if (item.gameObject.activeSelf && layoutElements[i] != null)
            {
                if (layoutElements[i].minHeight == -1)
                {
                    size = resHeight;
                }
                else
                {
                    size = rectChildren[i].rect.height <= layoutElements[i].minHeight ? layoutElements[i].minHeight : rectChildren[i].rect.height;
                }

                SetChildAlongAxis(item, 0, 0, parentWidth); // Установка позиции и ширины по X. По умолчанию по всей ширине
                SetChildAlongAxis(item, 1, yPos, size);     // Установка позиции и высоты по Y

                yPos += size + padding.top; // Сдвиг вниз
            }
        }
    }

    public override void CalculateLayoutInputVertical()
    {
       
    }

    public override void SetLayoutHorizontal()
    {
      
    }

    public override void SetLayoutVertical()
    {
       
    }
}
