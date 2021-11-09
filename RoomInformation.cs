using System.Collections.Generic;
using System;

namespace Rougelike
{
    class RoomInformation
    {
        // ID
        public int id { get; private set; }
        // 座標
        public int x { get; private set; }
        public int y { get; private set; }
        // 大きさ
        public int width { get; private set; }
        public int height { get; private set; }
        // 面積
        public int size { get; private set; }
        // 隣接部屋
        public List<int> upRoomId { get; set; }
        public List<int> downRoomId { get; set; }
        public List<int> leftRoomId { get; set; }
        public List<int> rightRoomId { get; set; }

        public RoomInformation(int x, int y, int width, int height, int id)
        {
            Set(x, y, width, height, id);

            upRoomId = new List<int>();
            downRoomId = new List<int>();
            leftRoomId = new List<int>();
            rightRoomId = new List<int>();
        }

        public void Set(int x, int y, int width, int height, int id)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            size = width * height;
        }

        public void Print()
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine("id:" + id);
            Console.WriteLine("x:" + x + ", y:" + y);
            Console.WriteLine("width:" + width + ", height:" + height);
            Console.WriteLine("size:" + size);
            Console.Write("upRoomId:");
            foreach (int i in upRoomId) Console.Write(i + " ");
            Console.WriteLine();
            Console.Write("downRoomId:");
            foreach (int i in downRoomId) Console.Write(i + " ");
            Console.WriteLine();
            Console.Write("leftRoomId:");
            foreach (int i in leftRoomId) Console.Write(i + " ");
            Console.WriteLine();
            Console.Write("rightRoomId:");
            foreach (int i in rightRoomId) Console.Write(i + " ");
            Console.WriteLine();
            Console.WriteLine("-------------------------");
        }
    }
}
