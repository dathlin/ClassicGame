using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassicGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Tetris.OnBitmapShowEvent += Tetris_OnBitmapShowEvent;
            TimerInitialization();
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            SetImageShow(Tetris.GetRenderBitmap());

            TetrisMini.MoveTeries.LocationDefault = new Point(0, 0);
            Point[] points = Tetris.GetNewItemPoints();
            TetrisMini.MoveTeries.SetPoint(points.ToArray());
            SetImageMiniShow(TetrisMini.GetRenderBitmap());

        }

        private void Tetris_OnBitmapShowEvent(object sender, GameEventArgs e)
        {
            SetImageShow(e.RenderBitmap);
        }

        //size is about 252*420
        TetrisEngine Tetris = new TetrisEngine(new Size(12, 20));
        TetrisEngine TetrisMini = new TetrisEngine(new Size(4, 4));

        private bool IsGameAccelerate { get; set; } = false;

        private void SetImageShow(Bitmap bimap)
        {
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = bimap;
        }
        private void SetImageMiniShow(Bitmap bimap)
        {
            pictureBox2.Image?.Dispose();
            pictureBox2.Image = bimap;
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                Tetris.MoveLeft();
            }
            else if (e.KeyCode == Keys.Right)
            {
                Tetris.MoveRight();
            }
            else if (e.KeyCode == Keys.Up)
            {
                Tetris.MoveChange();
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (!IsGameAccelerate)
                {
                    //这个时间决定了加速时的速度
                    TimerPlayer.Interval = 15;
                    TimerPlayer_Tick(null, null);
                    IsGameAccelerate = true;
                }
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (IsGameAccelerate)
                {
                    TimerPlayer.Interval = 400;
                    IsGameAccelerate = false;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if (label1.Text=="开始")
            {
                Tetris.ArraysReset();
                SetImageShow(Tetris.GetRenderBitmap());
                //重新生成图形
                Tetris.MoveTeries.SetPoint(TetrisMini.MoveTeries.InFactPoints.ToArray());
                Point[] points = Tetris.GetNewItemPoints();
                TetrisMini.MoveTeries.SetPoint(points.ToArray());
                SetImageMiniShow(TetrisMini.GetRenderBitmap());
                Tetris.PlayerScore = 0;
                label1.Text = "暂停";
                IsGameRunning = true;
            }
            else if(label1.Text=="暂停")
            {
                TimerPlayer.Stop();
                label1.Text = "继续";
            }
            else
            {
                label1.Text = "暂停";
                TimerPlayer.Start();
            }
        }


        /********************************************************************************
        * 
        *    定时器处理
        * 
        * 
        * 
        *******************************************************************************/

        private bool IsGameRunning { get; set; } = false;
        

        private Timer TimerPlayer { get; set; }
        private void TimerInitialization()
        {
            TimerPlayer = new Timer()
            {
                Interval = 400
            };
            TimerPlayer.Tick += TimerPlayer_Tick;
            TimerPlayer.Start();
        }

        private void TimerPlayer_Tick(object sender, EventArgs e)
        {
            if(IsGameRunning)
            {
                int result = Tetris.MoveDown();
                if (result == 1)
                {
                    TimerPlayer.Interval = 400;
                    //重新生成图形
                    Tetris.MoveTeries.SetPoint(TetrisMini.MoveTeries.InFactPoints.ToArray());
                    Point[] points = Tetris.GetNewItemPoints();
                    TetrisMini.MoveTeries.SetPoint(points.ToArray());
                    SetImageMiniShow(TetrisMini.GetRenderBitmap());
                    label3.Text = Tetris.PlayerScore.ToString(); ;
                }
                else if(result==2)
                {
                    IsGameRunning = false;
                    label1.Text = "开始";
                }
            }
        }

       
    }
}
