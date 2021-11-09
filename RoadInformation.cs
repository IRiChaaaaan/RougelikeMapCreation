using System;

namespace Rougelike
{
    class RoadInformation
    {
        // 始点座標
        public int startX { get; private set; }
        public int startY { get; private set; }
        // 終点座標
        public int endX { get; private set; }
        public int endY { get; private set; }

        public RoadInformation(int startX, int startY, int endX, int endY)
        {
            this.startX = startX;
            this.startY = startY;
            this.endX = endX;
            this.endY = endY;
        }

        public void Print()
        {
            Console.WriteLine("startX:" + startX + ", startY:" + startY);
            Console.WriteLine("endX:" + endX + ", endY:" + endY);
        }
    }
}
