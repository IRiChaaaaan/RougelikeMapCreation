using System.Linq;
using System.Collections.Generic;
using System;

namespace Rougelike
{
    enum MAP
    {
        NONE,
        ROAD,
        ROOM
    }

    class RougelikeMap
    {
        // ステージの大きさ
        public int width { get; private set; }
        public int height{ get; private set; }
        // 分割数
        private int splitNum;
        // 生成される部屋の大きさの最小値
        private int minWidth = 3;
        private int minHeight = 3;
        // 部屋情報のリスト
        private List<RoomInformation> splitRooms = new List<RoomInformation>();
        private List<RoomInformation> stageRooms = new List<RoomInformation>();
        // 道情報のリスト
        private List<RoadInformation> stageRoads = new List<RoadInformation>();
        // ランダムクラス
        static Random rand = new Random();

        // コンストラクタ
        public RougelikeMap(int width = 100, int height = 100, int splitNum = 10, int minWidth = 5, int minHeight = 5)
        {
            this.width = width;
            this.height = height;
            this.splitNum = splitNum;
            this.minWidth = minWidth;
            this.minHeight = minHeight;
            splitRooms.Add(new RoomInformation(0, 0, width, height, 1));
        }

        // マップ生成
        public int[,] Create()
        {
            // ステージ配列の初期化
            int[,] stage = new int[height, width];
            for (int y = 0; y<height; y++)
            {
                for (int x = 0; x<width; x++)
                {
                    stage[y, x] = (int)MAP.NONE;
                }
            }
            // 分割の最小値を設定
            int splitWidth = 4 + minWidth;
            int splitHeight = 4 + minHeight;
            // ステージを分割
            for(int i = 0; i < splitNum; i++)
            {
                // サイズが一番大きい部屋情報を取得
                RoomInformation ri = splitRooms.OrderByDescending(value => value.size).FirstOrDefault();
                // 縦・横で大きい方を分割( 同じときは縦を分割 )
                if (ri.width > ri.height)
                {
                    // 分割できないときは終了
                    if(ri.width < splitWidth * 2) break;
                    // 横を分割
                    int w;
                    if (splitWidth == ri.width - splitWidth) w = ri.width;
                    else w = rand.Next(splitWidth, ri.width - splitWidth);
                    // 左側の分割部屋をリストに追加
                    splitRooms.Add(new RoomInformation(ri.x, ri.y, w, ri.height, ri.id));
                    // 右側の分割部屋をリストに追加
                    splitRooms.Add(new RoomInformation(ri.x + w, ri.y, ri.width - w, ri.height, i + 2));
                }
                else
                {
                    // 分割できないときは終了
                    if (ri.height < splitHeight * 2) break;
                    // 縦を分割
                    int h;
                    if (splitHeight == ri.height - splitHeight) h = splitHeight;
                    h = rand.Next(splitHeight, ri.height - splitHeight);
                    // 上側の分割部屋をリストに追加
                    splitRooms.Add(new RoomInformation(ri.x, ri.y, ri.width, h, ri.id));
                    // 下側の分割部屋をリストに追加
                    splitRooms.Add(new RoomInformation(ri.x, ri.y + h, ri.width, ri.height - h, i + 2));
                }
                // 分割した部屋をリストから削除
                splitRooms.Remove(ri);
            }
            // 部屋の隣接関係を設定
            foreach (RoomInformation ri in splitRooms)
            {
                foreach (RoomInformation ri_conf in splitRooms)
                {
                    if (ri.id != ri_conf.id)
                    {
                        // 上側
                        if (ri.y == ri_conf.y + ri_conf.height)
                        {
                            if ((ri.x < ri_conf.x + ri_conf.width) && (ri.x + ri.width > ri_conf.x))
                            {
                                ri.upRoomId.Add(ri_conf.id);
                                
                            }
                        }
                        // 下側
                        if (ri.y + ri.height == ri_conf.y)
                        {
                            if ((ri.x < ri_conf.x + ri_conf.width) && (ri.x + ri.width > ri_conf.x))
                            {
                                ri.downRoomId.Add(ri_conf.id);
                            }
                        }
                        // 左側
                        if (ri.x == ri_conf.x + ri_conf.width)
                        {
                            if ((ri.y <= ri_conf.y + ri_conf.height) && (ri.y + ri.height >= ri_conf.y))
                            {
                                ri.leftRoomId.Add(ri_conf.id);
                            }
                        }
                        // 右側
                        if (ri.x + ri.width == ri_conf.x)
                        {
                            if ((ri.y <= ri_conf.y + ri_conf.height) && (ri.y + ri.height >= ri_conf.y))
                            {
                                ri.rightRoomId.Add(ri_conf.id);
                            }
                        }
                    }
                }
            }
            // 部屋生成
            foreach (RoomInformation ri in splitRooms)
            {
                // 部屋の縦・幅の決定
                int w = rand.Next(minWidth, ri.width - 4);
                int h = rand.Next(minHeight, ri.height - 4);
                // 座標をずらす
                int moveX = rand.Next(2, ri.width - 2 - w);
                int moveY = rand.Next(2, ri.height - 2 - h);
                // 部屋情報を保存
                stageRooms.Add(new RoomInformation(ri.x + moveX, ri.y + moveY, w, h, ri.id));
                // 配列に書き出す
                for (int y = ri.y + moveY; y < ri.y + moveY + h; y++)
                {
                    for (int x = ri.x + moveX; x < ri.x + moveX + w; x++)
                    {
                        stage[y, x] = ri.id;
                    }
                }
            }
            // 通路作成
            for (int i = 0; i < splitNum + 1; i++)
            {
                RoomInformation splitRoom = splitRooms[i];
                RoomInformation stageRoom = stageRooms[i];
                // 上に道を伸ばす
                foreach (int id in splitRoom.upRoomId)
                {
                    int x = 0;
                    while (x % 2 == 0) x = rand.Next(stageRoom.x, stageRoom.x + stageRoom.width);
                    int max = Math.Max(stageRoom.y, splitRoom.y);
                    int min = Math.Min(stageRoom.y, splitRoom.y);
                    stageRoads.Add(new RoadInformation(x, min, x, max));
                }
                // 下に道を伸ばす
                foreach (int id in splitRoom.downRoomId)
                {
                    int x = 0;
                    while (x % 2 == 0) x = rand.Next(stageRoom.x, stageRoom.x + stageRoom.width);
                    int max = Math.Max(stageRoom.y + stageRoom.height, splitRoom.y + splitRoom.height);
                    int min = Math.Min(stageRoom.y + stageRoom.height, splitRoom.y + splitRoom.height);
                    stageRoads.Add(new RoadInformation(x, min, x, max));
                }
                // 左に道を伸ばす
                foreach (int id in splitRooms[i].leftRoomId)
                {
                    int y = 0;
                    while (y % 2 == 0) y = rand.Next(stageRoom.y, stageRoom.y + stageRoom.height);
                    int max = Math.Max(stageRoom.x, splitRoom.x);
                    int min = Math.Min(stageRoom.x, splitRoom.x);
                    stageRoads.Add(new RoadInformation(min, y, max, y));
                }
                // 右に道を伸ばす
                foreach (int id in splitRooms[i].rightRoomId)
                {
                    int y = 0;
                    while (y % 2 == 0) y = rand.Next(stageRoom.y, stageRoom.y + stageRoom.height);
                    int max = Math.Max(stageRoom.x + stageRoom.width, splitRoom.x + splitRoom.width);
                    int min = Math.Min(stageRoom.x + stageRoom.width, splitRoom.x + splitRoom.width);
                    stageRoads.Add(new RoadInformation(min, y, max, y));
                }
            }
            foreach(RoomInformation ri in splitRooms)
            {
                List<RoadInformation> roads = new List<RoadInformation>();
                // 上
                foreach (RoadInformation roadInfo in stageRoads)
                {
                    if (ri.x <= roadInfo.startX && ri.x + ri.width >= roadInfo.startX)
                    {
                        if (ri.y == roadInfo.startY || ri.y == roadInfo.endY) roads.Add(roadInfo);
                    }
                }
                if (roads.Count > 1)
                {
                    stageRoads.Add(
                        new RoadInformation(
                            roads.OrderBy(rec => rec.startX).FirstOrDefault().startX,
                            ri.y,
                            roads.OrderByDescending(rec => rec.startX).FirstOrDefault().startX + 1,
                            ri.y
                        )
                    );
                }
                // 下
                roads.Clear();
                foreach (RoadInformation roadInfo in stageRoads)
                {
                    if (ri.x <= roadInfo.startX && ri.x + ri.width >= roadInfo.startX)
                    {
                        if (ri.y + ri.height == roadInfo.endY || ri.y + ri.height == roadInfo.startY) roads.Add(roadInfo);
                    }
                }
                if (roads.Count > 1)
                {
                    stageRoads.Add(
                        new RoadInformation(
                            roads.OrderBy(rec => rec.startX).FirstOrDefault().startX,
                            ri.y + ri.height,
                            roads.OrderByDescending(rec => rec.startX).FirstOrDefault().startX + 1,
                            ri.y + ri.height
                        )
                    );
                }
                
                // 左
                roads.Clear();
                foreach (RoadInformation roadInfo in stageRoads)
                {
                    if (ri.y <= roadInfo.startY && ri.y + ri.height >= roadInfo.startY)
                    {
                        if (ri.x == roadInfo.startX || ri.x == roadInfo.endX) roads.Add(roadInfo);
                    }
                        
                }
                if (roads.Count > 1)
                {
                    stageRoads.Add(
                        new RoadInformation(
                            ri.x,
                            roads.OrderBy(rec => rec.startY).FirstOrDefault().startY,
                            ri.x,
                            roads.OrderByDescending(rec => rec.startY).FirstOrDefault().startY + 1
                        )
                    );
                }
                
                // 右
                roads.Clear();
                foreach (RoadInformation roadInfo in stageRoads)
                {
                    if (ri.y <= roadInfo.startY && ri.y + ri.height >= roadInfo.startY)
                    {
                        if (ri.x + ri.width == roadInfo.startX || ri.x + ri.width == roadInfo.endX)
                        {
                            roads.Add(roadInfo);
                        }
                    }
                }
                if (roads.Count > 1)
                {
                    stageRoads.Add(
                        new RoadInformation(
                            ri.x + ri.width,
                            roads.OrderBy(rec => rec.startY).FirstOrDefault().startY,
                            ri.x + ri.width,
                            roads.OrderByDescending(rec => rec.startY).FirstOrDefault().startY + 1
                        )
                    );
                }
                roads.Clear();
            }
            // 配列に書き出す
            foreach (RoomInformation ri in stageRooms)
            {
                for (int y = ri.y; y < ri.y + ri.height; y++)
                {
                    for (int x = ri.x; x < ri.x + ri.width; x++)
                    {
                        stage[y, x] = (int)MAP.ROOM;
                    }
                }
            }
            foreach (RoadInformation ri in stageRoads)
            {
                for(int y = ri.startY; y < ri.endY; y++)
                {
                    stage[y, ri.startX] = (int)MAP.ROAD;
                }
                for (int x = ri.startX; x < ri.endX; x++)
                {
                    stage[ri.startY, x] = (int)MAP.ROAD;
                }
            }
            
            return stage;
        }
    }
}
