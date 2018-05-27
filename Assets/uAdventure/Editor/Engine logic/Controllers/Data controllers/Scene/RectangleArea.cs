using System.Collections.Generic;
using UnityEngine;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public interface RectangleArea
    {

        bool isRectangular();

        List<Vector2> getPoints();

        void addPoint(int x, int y);

        Vector2 getLastPoint();

        void deletePoint(Vector2 point);

        void setRectangular(bool selected);

        Rectangle getRectangle();
    }
}