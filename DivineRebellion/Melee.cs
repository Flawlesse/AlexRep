using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivineRebellion
{
    public class Melee: Unit
    {
        /***********IDISPOSABLE*****************/
        bool _disposed = false;
        private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);
        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~Melee() => Dispose(false);
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
                _safeHandle?.Dispose();
            }
            _disposed = true;
            base.Dispose(disposing);
        }
        /***********IDISPOSABLE*****************/
        /***********PROPERTIES******************/
        private Stack<int> px, py;
        private int[,] wayMap;
        /***********PROPERTIES******************/
        /***********METHODS*********************/
        public Melee(Team team, int x, int y) : base(team, x, y)
        {
            string path = @"D:\VS2020\DivineRebellion\Textures\MeleePhys";
            path += (team == Team.Blue) ? "Blue.png" : "Red.png";

            Image img = Image.FromFile(path);//will be specific
            Bitmap bmp = BattleField.ResizeImage(img, BattleField.resolution * 2, BattleField.resolution * 4);
            Texture = bmp;
            img.Dispose();

            px = new Stack<int>();
            py = new Stack<int>();
            

            wayMap = new int[BattleField.height / BattleField.resolution, BattleField.width / BattleField.resolution];
            for (int i = 0; i < BattleField.height / BattleField.resolution; i++)
                for (int j = 0; j < BattleField.width / BattleField.resolution; j++)
                    wayMap[i, j] = default;
        }
        public override bool SomeoneInRange(Tile[,] bt, int h, int w)//only for melee
        {
            int[] dx = { 1, 0, -1, 0 };   // смещения, соответствующие соседям ячейки
            int[] dy = { 0, 1, 0, -1 };   // справа, снизу, слева и сверху
            for (int k = 0; k < 4; k++)                    // проходим по всем непомеченным соседям
            {
                int iy = UY + dy[k], ix = UX + dx[k];
                if (iy >= 0 && iy < h && ix >= 0 && ix < w && bt[iy, ix].Warrior != null && bt[iy, ix].Warrior.IsAlive && bt[iy, ix].Warrior.UTeam != this.UTeam)
                {
                    SetTarget(bt[iy, ix].Warrior);
                    switch (k)
                    {
                        case 0: Dir = Direction.Right; break;
                        case 1: Dir = Direction.Down; break;
                        case 2: Dir = Direction.Left; break;
                        case 3: Dir = Direction.Up; break;
                    }
                    IsAttacking = true;
                    return true;
                }
            }
            return false;
        }
        public override void Move(Tile[,] bt)
        {
            if (HasMoved)
                return;

            IsAttacking = false;
            InititalizeWayMap(bt);
            CreateNewWayMap();

            //проверка на действительность клетки
            if (bt[py.Peek(), px.Peek()].IsFree)
            {
                if (UX < px.Peek())
                    Dir = Direction.Right;
                else if (UX > px.Peek())
                    Dir = Direction.Left;
                else if (UY < py.Peek())
                    Dir = Direction.Down;
                else if (UY > py.Peek())
                    Dir = Direction.Up;

                bt[py.Peek(), px.Peek()].Warrior = this;
                bt[py.Peek(), px.Peek()].IsFree = false;
                bt[UY, UX].Warrior = null;
                bt[UY, UX].IsFree = true;
                UX = px.Pop();
                UY = py.Pop();
            }
            HasMoved = true;
        }
        public override void AttackTarget(BattleField bf)
        {
            if (Target != null)
            {
                Target.TakeDamage(bf, AADmg, DmgType);
            }
        }
        private void InititalizeWayMap(Tile[,] bt)//инициализирует карту для работы алгоритма поиска пути на каждом юните
        {
            for (int i = 0; i < BattleField.height / BattleField.resolution; i++)
                for (int j = 0; j < BattleField.width / BattleField.resolution; j++)
                    if (bt[i, j].IsFree)
                        wayMap[i, j] = -1;
                    else
                    {
                        if (bt[i, j].Warrior == null)
                            wayMap[i, j] = -2;//wall
                        else if (bt[i, j].Warrior != null && bt[i, j].Warrior.UTeam != this.UTeam)
                            wayMap[i, j] = -3;//possible target
                        else if (bt[i, j].Warrior != null && bt[i, j].Warrior.UTeam == this.UTeam)
                            wayMap[i, j] = -2;//act like it's a wall
                    }
        }
        private void CreateNewWayMap()//lee algo
        {
            int h = BattleField.height / BattleField.resolution;
            int w = BattleField.width / BattleField.resolution;
            wayMap[UY, UX] = 0;//start will be zero
            px.Clear();
            py.Clear();
            int[] dx = { 1, 0, -1, 0 };   // смещения, соответствующие соседям ячейки
            int[] dy = { 0, 1, 0, -1 };   // справа, снизу, слева и сверху
            int d, x, y, k;
            bool stop;

            d = 0;
            do
            {
                stop = true;               // предполагаем, что все свободные клетки уже помечены
                for (y = 0; y < h; y++)
                    for (x = 0; x < w; x++)
                        if (wayMap[y, x] == d)                         // ячейка (x, y) помечена числом d
                        {
                            for (k = 0; k < 4; k++)                    // проходим по всем непомеченным соседям
                            {
                                int iy = y + dy[k], ix = x + dx[k];
                                if (iy >= 0 && iy < h && ix >= 0 && ix < w && wayMap[iy, ix] == -1)
                                {
                                    stop = false;              // найдены непомеченные клетки
                                    wayMap[iy, ix] = d + 1;      // распространяем волну
                                }
                            }
                        }
                d++;
            } while (!stop);

            // восстановление пути
            int len = 1000;            // длина кратчайшего пути из (ax, ay) в (bx, by)
            int bx = 0;
            int by = 0;
            for (y = 0; y < h; y++)
                for (x = 0; x < w; x++)
                    if (wayMap[y, x] == -3)                         // ячейка (x, y) помечена числом d
                    {
                        for (k = 0; k < 4; k++)                    // проходим по всем непомеченным соседям
                        {
                            int iy = y + dy[k], ix = x + dx[k];
                            if (iy >= 0 && iy < h && ix >= 0 && ix < w && wayMap[iy, ix] < len && wayMap[iy, ix] >= 0)
                            {
                                bx = ix;
                                by = iy;
                                len = wayMap[iy, ix];
                            }
                        }
                    }

            d = len;
            while (d > 0)
            {
                px.Push(bx);
                py.Push(by);                   // записываем ячейку (x, y) в путь
                d--;
                for (k = 0; k < 4; ++k)
                {
                    int iy = by + dy[k], ix = bx + dx[k];
                    if (iy >= 0 && iy < h && ix >= 0 && ix < w && wayMap[iy, ix] == d)
                    {
                        bx = bx + dx[k];
                        by = by + dy[k];           // переходим в ячейку, которая на 1 ближе к старту
                        break;
                    }
                }
            }
        }
        /***********METHODS*********************/
    }
}
