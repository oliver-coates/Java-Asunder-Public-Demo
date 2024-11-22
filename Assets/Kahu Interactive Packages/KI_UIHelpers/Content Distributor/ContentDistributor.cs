using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KahuInteractive.UIHelpers
{

public static class ContentDistributor
{
    public enum Direction
    {
        TopToBottom,
        LeftToRight
    }

    public static void DistributeContent(List<RectTransform> contents, RectTransform within, Direction direction)
    {
        int numberOfContent = contents.Count;


        float yHeight = within.rect.max.y - within.rect.min.y;
        float xWidth = within.rect.max.x - within.rect.min.x;

        float totalSpace = 0f;
        if (direction == Direction.TopToBottom)
        {
            totalSpace = yHeight;
        }
        else if (direction == Direction.LeftToRight)
        {
            totalSpace = xWidth;
        }


        #region Only one content
        if (numberOfContent == 1)
        {
            RectTransform content = contents[0];
            content.anchoredPosition = new Vector2(0, 0);
        }
        #endregion
        #region Multiple Contents
        else
        {
            float spaceBetweenContent = totalSpace / (numberOfContent - 1.0f);
            for (int i = 0; i < numberOfContent; i++)
            {
                RectTransform content = contents[i];

                float xPos = 0f;
                float yPos = 0f;

                if (direction == Direction.TopToBottom)
                {
                    yPos = (-yHeight/2f) + (i * spaceBetweenContent);
                }
                else if (direction == Direction.LeftToRight)
                {
                    xPos = (-xWidth/2f) + (i * spaceBetweenContent);
                }

                content.anchoredPosition = new Vector2(xPos, yPos);
            }
            
        }
        #endregion

    }

}


}