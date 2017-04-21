using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ClassicGame
{
    /***************************************************************************
     * 
     *    copyright by Richard.Hu
     *    
     *    this class controls all the actions 
     *    
     *    [这个类控制整个系统运行的核心引擎]
     * 
     **************************************************************************/

    /**************************************************************************
     * 
     *    采用数据层加一个移动数据对象来实现整个俄罗斯方块的实现
     * 
     *    One layers of data and a mobile data objects are used to implement the whole tetris
     * 
     **************************************************************************/



    public class TetrisEngine : GameEngine
    {

        public override void Initialization(Size size)
        {
            base.Initialization(size);
            MoveTeries= new TetrisPoints(new Point[]
            {
                new Point(0,0),new Point(0,1),new Point(1,1),new Point(1,2)
            }, new Point(BoardSize.Width / 2 - 2, 20), BoardSize, OnMoveObjectSuccess);
        }
        public bool OnMoveObjectSuccess(Point[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].X >= 0 && points[i].X < BoardSize.Width &&
                    points[i].Y >= 0 && points[i].Y < BoardSize.Height + 4)
                {
                    if (points[i].Y < BoardSize.Height)
                    {
                        if ((GameArrays[points[i].X, points[i].Y]).HasItem)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        public TetrisEngine(Size size)
        {
            //must call the methon 
            Initialization(size);
        }

        public Bitmap GetRenderBitmap()
        {
            Bitmap bitmp = new Bitmap(BoardSize.Width * (WidthOfItem + 1), BoardSize.Height * (WidthOfItem + 1));
            Graphics g = Graphics.FromImage(bitmp);
            g.Clear(SystemColors.Control);
            DrawBackgroundImage(g);

            //paint other info
            for (int i = 0; i < MoveTeries.AllPoints.Length; i++)
            {
                DrawTetrisItem(g, MoveTeries.AllPoints[i].X, MoveTeries.AllPoints[i].Y, MoveTeries.PaintColor);
            }

            g.Dispose();
            return bitmp;
        }

        private int ClearLines()
        {
            int count = 0;
            for (int i = 0; i < BoardSize.Height; i++)
            {
                while(IsLinesClear(i))
                {
                    count++;
                    LinesMoveDown(i);
                }
            }
            return count;
        }

        private bool IsLinesClear(int i)
        {
            for (int j = 0; j < BoardSize.Width; j++)
            {
                if (!GameArrays[j, i].HasItem)
                {
                    return false;
                }
            }
            return true;
        }
        private void LinesMoveDown(int i)
        {
            for (int j = i; j < BoardSize.Height; j++)
            {
                if (j < BoardSize.Height - 1)
                {
                    for (int k = 0; k < BoardSize.Width; k++)
                    {
                        GameArrays[k, j].HasItem = GameArrays[k, j + 1].HasItem;
                        GameArrays[k, j].ItemColor = GameArrays[k, j + 1].ItemColor;
                        GameArrays[k, j].ItemCode = GameArrays[k, j + 1].ItemCode;
                    }
                }
                else
                {
                    for (int k = 0; k < BoardSize.Width; k++)
                    {
                        GameArrays[k, j].HasItem = false ;
                    }
                }
            }
        }
        private bool IsGameOver()
        {
            for (int j = 0; j < BoardSize.Width; j++)
            {
                if (GameArrays[j, BoardSize.Height-1].HasItem)
                {
                    return true;
                }
            }
            return false;
        }
        private int GetScoreFromLinesClear(int line)
        {
            switch(line)
            {
                case 0:return 0;
                case 1:return 1;
                case 2:return 3;
                case 3:return 6;
                case 4:return 10;
                default:return 0;
            }
        }

        /********************************************************************************
         * 
         *    界面的一个移动的对象
         * 
         * 
         * 
         *******************************************************************************/

        public TetrisPoints MoveTeries { get; set; }

        /********************************************************************************
        * 
        *    响应键盘块
        * 
        * 
        * 
        *******************************************************************************/
        public void MoveLeft()
        {
            MoveTeries.MoveLeft();
            OnBitmapShow(GetRenderBitmap());
        }
        public void MoveRight()
        {
            MoveTeries.MoveRight();
            OnBitmapShow(GetRenderBitmap());
        }
        public void MoveChange()
        {
            MoveTeries.MoveChange();
            OnBitmapShow(GetRenderBitmap());
        }
        /// <summary>
        /// 返回0，不处理，1，游戏继续，2，游戏结束
        /// </summary>
        /// <returns></returns>
        public int MoveDown()
        {
            if(OnMoveObjectSuccess(MoveTeries.DownPoints))
            {
                MoveTeries.MoveDown();
                OnBitmapShow(GetRenderBitmap());
                return 0;
            }
            else
            {
                //到达图形底部
                SetPointFixed(MoveTeries.AllPoints);
                MoveTeries.Location = MoveTeries.LocationDefault;
                //判断并消除原有的行
                int lines=ClearLines();
                //游戏是否结束
                if (!IsGameOver())
                {
                    //追加得分
                    PlayerScore += GetScoreFromLinesClear(lines);
                    OnBitmapShow(GetRenderBitmap());
                    return 1;
                }
                else
                {
                    //游戏结束
                    return 2;
                }
            }
        }


        /********************************************************************************
         * 
         *    新图形生成块
         * 
         * 
         * 
         *******************************************************************************/
        List<Point[]> list = new List<Point[]>()
            {
                new Point[]{new Point(0,0),new Point(0,1),new Point(1,1),new Point(1,0)},
                new Point[]{new Point(0,0),new Point(0,1),new Point(0,2),new Point(0,3)},
                new Point[]{ new Point(0, 0), new Point(0, 1), new Point(1, 1), new Point(1, 2) },
                new Point[]{ new Point(0, 2), new Point(0, 1), new Point(1, 1), new Point(1, 0) },
                new Point[]{ new Point(0, 2), new Point(1, 2), new Point(1, 1), new Point(1, 0) },
                new Point[]{ new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(2, 0) },
                new Point[]{ new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 2) },
                //new Point[]{new Point(0,0),new Point(0,3),new Point(3,3),new Point(3,0)},
                new Point[]{new Point(0,0),new Point(1,1),new Point(2,2),new Point(3,3)},
            };
        public Point[] GetNewItemPoints()
        {
            Random r = new Random();
            return list[r.Next(list.Count)].ToArray();
        }


    }


    public class TetrisPoints
    {
        /// <summary>
        /// this object color [绘图的颜色]
        /// </summary>
        public Color PaintColor { get; set; } = Color.Black;
        /// <summary>
        /// all items point [所有的点集，指定图形的样貌]
        /// </summary>
        private Point[] points;
        /// <summary>
        /// 
        /// </summary>
        private Size BoardSize { get; set; }
        private Func<Point[], bool> MoveJudge = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt">all points</param>
        /// <param name="locationDefault">default of the location</param>
        /// <param name="board">the board of movement</param>
        /// <param name="moveJudge"></param>
        public TetrisPoints(Point[] pt, Point locationDefault,Size board,Func<Point[],bool> moveJudge)
        {
            points = pt;
            BoardSize = board;
            LocationDefault = locationDefault;
            Location = locationDefault;
            MoveJudge = moveJudge;
        }
        public Point LocationDefault { get; set; }
        public Point Location { get; set; }
        public TetrisPoints(TetrisPoints tp)
        {
            Location = LocationDefault;
            SetPoint(tp.InFactPoints);
        }

        public void SetPoint(Point[] points_new)
        {
            Location = LocationDefault;
            points = new Point[points_new.Length];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Point(points_new[i].X, points_new[i].Y);
            }
        }

        public void MoveLeft()
        {
            if(MoveJudge(LeftPoints))
            {
                Location = new Point(Location.X - 1, Location.Y);
            }
        }
        public void MoveRight()
        {
            if(MoveJudge(RightPoints))
            {
                Location = new Point(Location.X + 1, Location.Y);
            }
        }
        public void MoveUp()
        {
            if(MoveJudge(UpPoints))
            {
                Location = new Point(Location.X, Location.Y + 1);
            }
        }
        public void MoveDown()
        {
            if(MoveJudge(DownPoints))
            {
                Location = new Point(Location.X, Location.Y - 1);
            }
        }
        public void MoveChange()
        {
            if(MoveJudge(ChangePoints))
            {
                points = OriginChangePoints;
                //如果位置位于窗口外面，左移回来
                while (LeftXPoint(AllPoints))
                {
                    Location = new Point(Location.X - 1, Location.Y);
                }
            }
        }
        
        
        //原点复位的算法
        public bool ResetXPoint(Point[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].X == 0) return false;
            }
            return true;
        }
        public bool ResetYPoint(Point[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].Y == 0) return false;
            }
            return true;
        }
        public bool LeftXPoint(Point[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].X >= BoardSize.Width) return true;
            }
            return false;
        }
        //#####################################################################################
        //原始的坐标点
        public Point[] InFactPoints
        {
            get { return points; }
        }
        //获取点，经过坐标转换之后的点
        public Point[] AllPoints
        {
            get
            {
                Point[] ptNow = new Point[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    ptNow[i] = new Point(points[i].X + Location.X, points[i].Y + Location.Y);
                }
                return ptNow;
            }
        }
        //坐标转换后再加向上移动
        public Point[] UpPoints
        {
            get
            {
                Point[] ptNow = new Point[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    ptNow[i] = new Point(points[i].X + Location.X, points[i].Y + Location.Y + 1);
                }
                return ptNow;
            }
        }
        //坐标转换后再加向下移动
        public Point[] DownPoints
        {
            get
            {
                Point[] ptNow = new Point[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    ptNow[i] = new Point(points[i].X + Location.X, points[i].Y + Location.Y - 1);
                }
                return ptNow;
            }
        }
        //坐标转换后再向左移动
        public Point[] LeftPoints
        {
            get
            {
                Point[] ptNow = new Point[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    ptNow[i] = new Point(points[i].X + Location.X - 1, points[i].Y + Location.Y);
                }
                return ptNow;
            }
        }
        //坐标转换后再向右移动
        public Point[] RightPoints
        {
            get
            {
                Point[] ptNow = new Point[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    ptNow[i] = new Point(points[i].X + Location.X + 1, points[i].Y + Location.Y);
                }
                return ptNow;
            }
        }
        public Point[] OriginChangePoints
        {
            get
            {
                Point[] ptNow = new Point[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    ptNow[i] = new Point(3 - points[i].Y, points[i].X);
                }
                while (ResetXPoint(ptNow))
                {
                    for (int i = 0; i < ptNow.Length; i++)
                    {
                        ptNow[i].X--;
                    }
                }
                while (ResetYPoint(ptNow))
                {
                    for (int i = 0; i < ptNow.Length; i++)
                    {
                        ptNow[i].Y--;
                    }
                }
                return ptNow;
            }
        }

        //旋转后的坐标
        public Point[] ChangePoints
        {
            get
            {
                Point[] ptNow = OriginChangePoints;
                //转换成实际坐标
                for (int i = 0; i < points.Length; i++)
                {
                    ptNow[i] = new Point(ptNow[i].X + Location.X, ptNow[i].Y + Location.Y);
                }
                //如果位置位于窗口外面，旋转回来
                while (LeftXPoint(ptNow))
                {
                    for (int i = 0; i < ptNow.Length; i++)
                    {
                        ptNow[i].X--;
                    }
                }
                return ptNow;
            }
        }
    }
}
