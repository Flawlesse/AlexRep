using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace DivineRebellion
{
    public enum Direction{ Left, Up, Right, Down }
    public enum UnitType{ Range, Melee }
    public enum DamageType{ Magic, Physical }
    public enum Team { Red, Blue }
    
    public class Unit: IDisposable
    {
        bool _disposed = false;
        private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~Unit() => Dispose(false);
        protected virtual void Dispose(bool disposing)
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
            
        }

        private int[,] wayMap;
        public Bitmap Texture;
        protected string Name { get; private set; }
        public bool IsAlive { get; set; } 
        public Direction Dir { get; private set; }
        public UnitType UType { get; private set; }
        public DamageType DmgType { get; private set; }
        public Team UTeam { get; private set; }
        public Unit Target { get; set; }
        public int AttackRange { get; private set; }
        protected int MoveSpeed { get; private set; }
        int ux, uy;//координаты
        public Unit()//constructor
        {
            Texture = new Bitmap(@"D:\My desktop personalization\stork_PNG47.png");

            Name = "Unit";
            IsAlive = true;
            Dir = Direction.Up;
            UType = UnitType.Melee;
            DmgType = DamageType.Physical;
            Target = null;
            AttackRange = 1;//1 tile
            MoveSpeed = 1;//1 tile

            wayMap = new int[BattleField.height / BattleField.resolution, BattleField.width / BattleField.resolution];
            for (int i = 0; i < BattleField.height / BattleField.resolution; i++)
                for (int j = 0; j < BattleField.width / BattleField.resolution; j++)
                    wayMap[i, j] = default;
        }
        protected void ChangeTexture()
        { 
            
        }
        protected bool InRange()
        {
            return true;
        }
        
        public void Move()
        {
            if (Target != null)
            {
                //for (int i = 0; i < 10; i++, await Task.Delay(100))
                  // Texture.Location = new Point(Texture.Location.X + (Target.Texture.Location.X - Texture.Location.X) / 10, Texture.Location.Y + (Target.Texture.Location.Y - Texture.Location.Y) / 10);
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
        private void CreateNewWayMap(int ax, int ay, int h, int w)//lee algo
        {
            int[] dx = { 1, 0, -1, 0 };   // смещения, соответствующие соседям ячейки
            int[] dy = { 0, 1, 0, -1 };   // справа, снизу, слева и сверху
            int d, x, y, k;
            bool stop;

            d = 0;
            wayMap[ay, ax] = 0;// стартовая ячейка помечена 0
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
        }
    }
}
