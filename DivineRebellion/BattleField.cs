using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DivineRebellion
{
    public class BattleField
    {
        private PictureBox FieldBox;
        private Bitmap CleanField;
        private Bitmap InitField;
        public static readonly int resolution = 75;
        private Tile[,] BattleTiles;
        public static readonly int width = 900, height = 750;
        private Pen Pen;
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

            //initializing battle tiles (most respective part)
            BattleTiles = new Tile[height / resolution, width / resolution];
            
            for (int i = 0; i < height / resolution; i++)
                for (int j = 0; j < width / resolution; j++)
                    BattleTiles[i, j] = new Tile();

            FieldBox.MouseMove += new MouseEventHandler(FieldBox_MouseMove);
            FieldBox.MouseClick += new MouseEventHandler(FieldBox_MouseClick);
        }
        private void FieldBox_MouseMove(object sender, MouseEventArgs e)
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
        private void FieldBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (BattleTiles[e.Y / resolution, e.X / resolution].IsFree)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Image img = Image.FromFile(@"D:\My desktop personalization\stork_PNG47.png");//will be specific
                    Bitmap bmp = ResizeImage(img, resolution, resolution); ;
                    LoadTexture(bmp, (e.X / resolution) * resolution, (e.Y / resolution) * resolution);
                    bmp.Dispose();
                    BattleTiles[e.Y / resolution, e.X / resolution].Warrior = new Unit();
                    BattleTiles[e.Y / resolution, e.X / resolution].IsFree = false;
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
        
    }
}
