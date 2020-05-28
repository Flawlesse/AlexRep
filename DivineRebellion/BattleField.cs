using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DivineRebellion
{
    public class TileEventArgs : EventArgs
    { 
        public int X { get; set; }
        public int Y { get; set; }
    }
    public class BattleField
    {
        System.Windows.Forms.Timer BattleTimer;
        private PictureBox FieldBox;
        private Bitmap CleanField;
        private Bitmap InitField;
        public static readonly int resolution = 75;
        public string UnitChoice { get; set; }
        public Tile[,] BattleTiles { get; private set; }
        private bool allowMouseMove;
        private bool allowMouseClick;
        public static readonly int width = 900, height = 750;
        private Pen Pen;
        public EventHandler<TileEventArgs> OnDied;//событие для обработки умершего воина
        private string toShow;
        public BattleField()
        {
            FieldBox = new PictureBox();
            Form1.ActiveForm.Controls.Add(FieldBox);
            FieldBox.SizeMode = PictureBoxSizeMode.StretchImage;
            FieldBox.Image = Image.FromFile(@"D:\My desktop personalization\600528.png");
            FieldBox.Image = ResizeImage(FieldBox.Image, height, width);
            InitField = new Bitmap(FieldBox.Image);
            CleanField = new Bitmap(FieldBox.Image);
            FieldBox.ClientSize = new Size(width, height);
            FieldBox.Parent = Form1.ActiveForm;
            Pen = new Pen(Color.Yellow);
            Pen.Width = 5;
            BattleTimer = new System.Windows.Forms.Timer();
            BattleTimer.Interval = 250;
            BattleTimer.Tick += new EventHandler(TimerEventProcessor);
            allowMouseClick = true;
            allowMouseMove = true;
            

            OnDied += FreeBCell;

            //initializing battle tiles (most respective part)
            BattleTiles = new Tile[height / resolution, width / resolution];
            
            for (int i = 0; i < height / resolution; i++)
                for (int j = 0; j < width / resolution; j++)
                    BattleTiles[i, j] = new Tile();

            FieldBox.MouseMove += new MouseEventHandler(FieldBox_MouseMove);
            FieldBox.MouseClick += new MouseEventHandler(FieldBox_MouseClick);
        }
        private void FreeBCell(object sender, TileEventArgs e)//надо малость переписать
        {
            Unit warrior = sender as Unit;
            if (warrior.Equals(BattleTiles[e.Y, e.X].Warrior))
            {
                BattleTiles[e.Y, e.X].Warrior.Dispose();
                BattleTiles[e.Y, e.X].Warrior = null;
                BattleTiles[e.Y, e.X].IsFree = true;
            }
        }
        private void FieldBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (allowMouseMove)
            {
                FieldBox.Image.Dispose();
                FieldBox.Image = (Image)InitField.Clone();
                //redraw

                Bitmap bmp = FieldBox.Image as Bitmap;
                Graphics graphics = Graphics.FromImage(bmp);
                graphics.DrawRectangle(Pen, (e.X / resolution) * resolution, (e.Y / resolution) * resolution, resolution, resolution);
                FieldBox.Image = bmp;

                graphics.Dispose();
            }
        }
        private void FieldBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (allowMouseClick)
            {
                if (BattleTiles[e.Y / resolution, e.X / resolution].IsFree)//переписать на загрузку из нового экземпляра Unit
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        
                        Team team;
                        if (e.Y < height / 2)
                            team = Team.Blue;
                        else
                            team = Team.Red;
                        switch (UnitChoice)
                        {
                            case "Melee":
                                BattleTiles[e.Y / resolution, e.X / resolution].Warrior = new Melee(team, e.X / resolution, e.Y / resolution);
                                break;
                            case "Ranged":
                                BattleTiles[e.Y / resolution, e.X / resolution].Warrior = new Ranged(team, e.X / resolution, e.Y / resolution);
                                break;
                        }
                        BattleTiles[e.Y / resolution, e.X / resolution].IsFree = false;
                        LoadTexture(BattleTiles[e.Y / resolution, e.X / resolution].Warrior.Texture, (e.X / resolution) * resolution, (e.Y / resolution) * resolution);
                    }
                }
                else
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        BattleTiles[e.Y / resolution, e.X / resolution].Warrior.Dispose();
                        ClearCell((e.X / resolution) * resolution, (e.Y / resolution) * resolution);
                        BattleTiles[e.Y / resolution, e.X / resolution].Warrior = null;
                        BattleTiles[e.Y / resolution, e.X / resolution].IsFree = true;
                    }
                }
            }
        }
        public static Bitmap ResizeImage(Image image, int new_height, int new_width)//ресайз изображения, чтобы его размер совпадал с клиентской областью
        {
            Bitmap new_image = new Bitmap(new_width, new_height);
            Graphics g = Graphics.FromImage(new_image);
            g.DrawImage(image, 0, 0, new_width, new_height);
            g.Dispose();
            return new_image;
        }
        public void LoadTexture(Bitmap bmp, int x1, int y1)//загружает переданную текстуру в InitField
        {
            for (int x = 0; x < resolution; x++)
                for (int y = 0; y < resolution; y++)
                {
                    Color col = bmp.GetPixel(x, y);
                    if (col != Color.FromArgb(0,0,0,0))//transparency
                        InitField.SetPixel(x1 + x, y1 + y, col);
                }

        }
        public void ClearCell(int x1, int y1)//очищает клетку от изображения
        {
            for (int x = 0; x < resolution; x++)
                for (int y = 0; y < resolution; y++)
                {
                    Color col = CleanField.GetPixel(x1 + x, y1 + y);
                    if (col != Color.FromArgb(0, 0, 0, 0))//transparency
                        InitField.SetPixel(x1 + x, y1 + y, col);
                }
        }
        public void ClearFullField(bool cleanObjects)//стирает всё, оставляя фон поля
        {
            InitField.Dispose();
            InitField = (Bitmap)CleanField.Clone();
            FieldBox.Image.Dispose();
            FieldBox.Image = (Image)InitField.Clone();
            if (cleanObjects)
            {
                for (int i = 0; i < height / resolution; i++)
                    for (int j = 0; j < width / resolution; j++)
                    {
                        if (BattleTiles[i, j].Warrior != null)
                        {
                            BattleTiles[i, j].Warrior.Dispose();
                            BattleTiles[i, j].Warrior = null;
                        }
                        BattleTiles[i, j].IsFree = true;
                    }
            }
        }
        private void Redraw()
        {
            int h = height / resolution, w = width / resolution;
            ClearFullField(false);
            

            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    if (BattleTiles[i, j].IsFree || BattleTiles[i, j].Warrior == null || !BattleTiles[i, j].Warrior.IsAlive)
                        continue;
                    else
                        LoadTexture(BattleTiles[i, j].Warrior.Texture, j * resolution, i * resolution);

            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    if (BattleTiles[i, j].Missiles.Count != 0)
                    {
                        foreach (Missile m in BattleTiles[i, j].Missiles.ToArray())
                            LoadTexture(m.Texture, j * resolution, i * resolution);
                    }

            FieldBox.Image.Dispose();
            FieldBox.Image = (Image)InitField.Clone();
        }
        private void TimerEventProcessor(object obj, EventArgs e)
        {
            //Redraw();
            int h = height / resolution, w = width / resolution;
           
            if (CheckFightDone())
            {
                BattleTimer.Stop();
                MessageBox.Show(toShow);
                allowMouseClick = true;
                allowMouseMove = true;
                return;
            }
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                {
                    if (BattleTiles[i, j].Missiles.Count != 0)
                        foreach (Missile m in BattleTiles[i, j].Missiles.ToArray())
                            m.Act(this, BattleTiles);

                    if (BattleTiles[i, j].IsFree || BattleTiles[i, j].Warrior == null || !BattleTiles[i, j].Warrior.IsAlive)
                        continue;
                    if (BattleTiles[i, j].Warrior.SomeoneInRange(BattleTiles, h, w))
                        BattleTiles[i, j].Warrior.AttackTarget(this);
                    else
                        BattleTiles[i, j].Warrior.Move(BattleTiles);
                }
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                {
                    if (BattleTiles[i, j].Missiles.Count != 0)
                        foreach (Missile m in BattleTiles[i, j].Missiles)
                            m.UnsetMove();

                    if (BattleTiles[i, j].IsFree)
                        continue;
                    else
                        BattleTiles[i, j].Warrior.UnsetMove();
                }
            Redraw();
        }
        private bool CheckFightDone()
        {
            int tblue = 0, tred = 0;
            for (int y = 0; y < height / resolution; y++)
                for (int x = 0; x < width / resolution; x++)
                    if (!BattleTiles[y, x].IsFree && BattleTiles[y, x].Warrior.UTeam == Team.Blue)
                        tblue++;
                    else if (!BattleTiles[y, x].IsFree && BattleTiles[y, x].Warrior.UTeam == Team.Red)
                        tred++;
                if (tred == 0 && tblue == 0)
                {
                    toShow = "No winner.";
                    return true;
                }
                else if (tred == 0 && tblue != 0)
                {
                    toShow = "Blue team won!";
                    return true;
                }
                else if (tred != 0 && tblue == 0)
                {
                    toShow = "Red team won!";
                    return true;
                }
            return false;
        }
        public void StartFight()
        {
            int tblue = 0, tred = 0;
            allowMouseClick = false;
            allowMouseMove = false;
            for (int y = 0; y < height / resolution; y++)
                for (int x = 0; x < width / resolution; x++)
                    if (!BattleTiles[y, x].IsFree && BattleTiles[y, x].Warrior.UTeam == Team.Blue)
                        tblue++;
                    else if (!BattleTiles[y, x].IsFree && BattleTiles[y, x].Warrior.UTeam == Team.Red)
                        tred++;
            if (tred == 0 || tblue == 0)
            {
                MessageBox.Show("Не во всех командах есть воины!");
                allowMouseClick = true;
                allowMouseMove = true;
                return;
            }
            BattleTimer.Start();
            
        }
        
    }
}
