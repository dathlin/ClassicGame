using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicGame
{
    public class GameEngine
    {
        /// <summary>
        /// 每个小框框的宽度
        /// </summary>
        protected int WidthOfItem { get; set; } = 20;
        protected GameItem[,] GameArrays { get; set; }

        /// <summary>
        /// 游戏画布的大小
        /// </summary>
        protected Size BoardSize { get; set; }

        /// <summary>
        /// 得分情况
        /// </summary>
        public int PlayerScore { get; set; } = 0;
        /// <summary>
        /// 游戏是否启动
        /// </summary>
        public bool IsGameStart { get; set; } = false;



        /// <summary>
        /// Initialization an object [实例化一个对象]
        /// </summary>
        /// <param name="size"></param>
        public virtual void Initialization(Size size)
        {
            BoardSize = size;

            GameArrays = new GameItem[size.Width, size.Height];
            for (int i = 0; i < BoardSize.Width; i++)
            {
                for (int j = 0; j < BoardSize.Height; j++)
                {
                    GameArrays[i, j] = new GameItem() ;
                }
            }
            //restore the colors [还原所有的颜色]
            TetrisArrayAction(t =>
            {
                t.ItemColor = Color.Black;
            });
        }
        
        private void TetrisArrayAction(Action<GameItem> action)
        {
            for (int i = 0; i < BoardSize.Width; i++)
            {
                for (int j = 0; j < BoardSize.Height; j++)
                {
                    action(GameArrays[i, j]);
                }
            }
        }

        /// <summary>
        /// paint the item by location
        /// </summary>
        /// <param name="g"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected void DrawTetrisItem(Graphics g, int x, int y)
        {
            if (x >= 0 && x < BoardSize.Width && y >= 0 && y < BoardSize.Height)
            {
                int location_x = x * (WidthOfItem + 1);
                int location_y = (BoardSize.Height - y - 1) * (WidthOfItem + 1);
                if (GameArrays[x, y].HasItem)
                {
                    using (Brush brush = new SolidBrush(GameArrays[x, y].ItemColor))
                    {
                        g.FillRectangle(brush, new Rectangle(location_x, location_y, WidthOfItem, WidthOfItem));
                        g.FillRectangle(Brushes.LightGray, new Rectangle(location_x + 2, location_y + 2, WidthOfItem - 4, WidthOfItem - 4));
                        g.FillRectangle(brush, new Rectangle(location_x + WidthOfItem / 4, location_y + WidthOfItem / 4, WidthOfItem / 2, WidthOfItem / 2));
                    }
                }
                else
                {
                    g.FillRectangle(Brushes.LightGray, new Rectangle(location_x, location_y, WidthOfItem, WidthOfItem));
                    g.FillRectangle(Brushes.WhiteSmoke, new Rectangle(location_x+2,location_y+ 2, WidthOfItem - 4, WidthOfItem - 4));
                    g.FillRectangle(Brushes.LightGray, new Rectangle(location_x + WidthOfItem / 4, location_y + WidthOfItem / 4, WidthOfItem / 2, WidthOfItem / 2));
                }
            }
        }
        protected void DrawTetrisItem(Graphics g, int x, int y, Color color)
        {
            if (x >= 0 && x < BoardSize.Width && y >= 0 && y < BoardSize.Height)
            {
                int location_x = x * (WidthOfItem + 1);
                int location_y = (BoardSize.Height - y - 1) * (WidthOfItem + 1);
                using (Brush brush = new SolidBrush(GameArrays[x, y].ItemColor))
                {
                    g.FillRectangle(brush, new Rectangle(location_x, location_y, WidthOfItem, WidthOfItem));
                    g.FillRectangle(Brushes.LightGray, new Rectangle(location_x + 2, location_y + 2, WidthOfItem - 4, WidthOfItem - 4));
                    g.FillRectangle(brush, new Rectangle(location_x + WidthOfItem / 4, location_y + WidthOfItem / 4, WidthOfItem / 2, WidthOfItem / 2));
                }
            }
        }

        public event EventHandler<GameEventArgs> OnBitmapShowEvent;
        /// <summary>
        /// 触发一个事件，刷新结果显示
        /// </summary>
        /// <param name="bitmap"></param>
        protected void OnBitmapShow(Bitmap bitmap)
        {
            OnBitmapShowEvent?.Invoke(this, new GameEventArgs() { RenderBitmap = bitmap });
        }


        public void DrawBackgroundImage(Graphics g)
        {
            for (int x = 0; x < BoardSize.Width; x++)
            {
                for (int y = 0; y < BoardSize.Height; y++)
                {
                    DrawTetrisItem(g, x, y);
                }
            }
        }


        public void ArraysReset()
        {
            for (int x = 0; x < BoardSize.Width; x++)
            {
                for (int y = 0; y < BoardSize.Height; y++)
                {
                    GameArrays[x, y].HasItem = false;
                }
            }
        }

        public void SetPointFixed(Point[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].X >= 0 && points[i].X < BoardSize.Width &&
                    points[i].Y >= 0 && points[i].Y < BoardSize.Height)
                {
                    GameArrays[points[i].X, points[i].Y].HasItem = true;
                }
            }
        }

    }


    /// <summary>
    /// 单个的点
    /// </summary>
    public class GameItem
    {
        /// <summary>
        /// 指代不同的东西
        /// </summary>
        public int ItemCode { get; set; } = 1;
        public Color ItemColor { get; set; } = Color.Black;
        public bool HasItem { get; set; } = false;
        public Point Position { get; set; } = new Point(0, 0);
    }
    /// <summary>
    /// 自定义事件，传送一个图片接口
    /// </summary>
    public class GameEventArgs : EventArgs
    {
        public Bitmap RenderBitmap { get; set; }
    }
    /// <summary>
    /// 准备自动移动的方向
    /// </summary>
    public enum MoveDirections
    {
        Left = 1,
        Right = 2,
        Up = 3,
        Down = 4
    }


    public class GameCustom
    {

        private List<GameItem> all_Items;
        /// <summary>
        /// default is down 对象的移动方向，默认向下
        /// </summary>
        public MoveDirections Direction { get; set; } = MoveDirections.Down;


        public GameCustom(List<GameItem> items, Func<GameItem[], bool> condition)
        {
            all_Items = items;
            if (all_Items == null) throw new NullReferenceException("Items is not allow null ,[传入的参数对象不能为空]");
            Condition = condition;
            if (Condition == null) throw new NullReferenceException("Condition is not allow null ,[传入的参数对象不能为空]");
        }

        private Func<GameItem[], bool> Condition { get; set; }

        public bool MoveLeft()
        {
            Direction = MoveDirections.Left;
            return Offset(-1, 0);
        }
        public bool MoveRight()
        {
            Direction = MoveDirections.Right;
            return Offset(1, 0);
        }
        public bool MoveUp()
        {
            Direction = MoveDirections.Up;
            return Offset(0, 1);
        }
        public bool MoveDown()
        {
            Direction = MoveDirections.Down;
            return Offset(0, -1);
        }
        public bool MoveSelf()
        {
            switch (Direction)
            {
                case MoveDirections.Left: return MoveLeft();
                case MoveDirections.Right: return MoveRight();
                case MoveDirections.Up: return MoveUp();
                case MoveDirections.Down: return MoveDown();
                default: return false;
            }
        }

        public bool Offset(int dx, int dy)
        {
            GameItem[] temp = all_Items.ToArray();
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i].Position.Offset(dx, dy);
            }
            if (Condition(temp))
            {
                for (int i = 0; i < all_Items.Count; i++)
                {
                    all_Items[i].Position.Offset(dx, dy);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }

}
