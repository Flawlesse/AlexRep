using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Drawing;

namespace DivineRebellion
{
    public class Missile//: IDisposable
    {
        /***********IDISPOSABLE*****************/
        //bool _disposed = false;
        //private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);
        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}
        //~Missile() => Dispose(false);
        //protected virtual void Dispose(bool disposing)
        //{
        //    if (_disposed)
        //    {
        //        return;
        //    }

        //    if (disposing)
        //    {
        //        // Dispose managed state (managed objects).
        //        _safeHandle?.Dispose();
        //    }
        //    _disposed = true;
        //}
        /***********IDISPOSABLE*****************/
        /***********PROPERTIES******************/
        public Bitmap Texture { get; private set; }
        public Direction MDir { get; private set; }
        public DamageType DmgType { get; private set; }
        public Team MTeam { get; private set; }
        private int Damage { get; set; }
        public int Velocity { get; private set; }//измеряется в клетках
        private int MX { get; set; }
        private int MY { get; set; }
        public bool HasMoved { get; private set; }
        private Unit Target { get; set; }
        /***********PROPERTIES******************/
        /***********METHODS*********************/
        public Missile(Team t, Direction dir, int v, int x, int y, DamageType dmgt, int dmg)
        {
            Image img = Image.FromFile(@"D:\VS2020\DivineRebellion\bullet.png");//will be specific
            Bitmap bmp = BattleField.ResizeImage(img, BattleField.resolution, BattleField.resolution);
            //img.Dispose();
            Texture = bmp;

            MTeam = t;
            MDir = dir;
            Velocity = v;
            MX = x;
            MY = y;
            DmgType = dmgt;
            Damage = dmg;
            Target = null;
            HasMoved = true;
        }
        public void Act(BattleField bf, Tile[,] bt)
        {
            int h = BattleField.height / BattleField.resolution, w = BattleField.width / BattleField.resolution;
            if (SomeoneInRange(bt, h, w))
            {
                Attack(bf);
                bt[MY, MX].Missiles.Remove(this);
                //dispose?
            }
            else
            {
                Move(bt, h, w);
                //out of range -> dispose?
            }
        }
        public void Attack(BattleField bf)
        {
            if (Target != null)
            {
                Target.TakeDamage(bf, Damage, DmgType);
            }
        }
        public void Move(Tile[,] bt, int h, int w)
        {
            if (HasMoved)
                return;

            int dx, dy;
            switch (MDir)
            {
                case Direction.Down:
                    dx = 0;
                    dy = 1;
                    break;
                case Direction.Left:
                    dx = -1;
                    dy = 0;
                    break;
                case Direction.Right:
                    dx = 1;
                    dy = 0;
                    break;
                default:
                    dx = 0;
                    dy = -1;
                    break;
            }
            dx += MX;
            dy += MY;

            if (dx >= 0 && dx < w && dy >= 0 && dy < h)
            {
                bt[MY, MX].Missiles.Remove(this);
                this.MX = dx;
                this.MY = dy;
                bt[dy, dx].Missiles.Add(this);
                
            }
            else
            {
                bt[MY, MX].Missiles.Remove(this);
                //dispose?
            }
            HasMoved = true;
        }
        private bool SomeoneInRange(Tile[,] bt, int h, int w)
        {
            int dx, dy;
            if (bt[MY, MX].Warrior != null && bt[MY, MX].Warrior.IsAlive && bt[MY, MX].Warrior.UTeam != this.MTeam)
            {
                SetTarget(bt[MY, MX].Warrior);
                return true;
            }

            switch (MDir)
            {
                case Direction.Down:
                    dx = 0;
                    dy = 1;
                    break;
                case Direction.Left:
                    dx = -1;
                    dy = 0;
                    break;
                case Direction.Right:
                    dx = 1;
                    dy = 0;
                    break;
                default:
                    dx = 0;
                    dy = -1;
                    break;
            }

            dx += MX;
            dy += MY;
            if (dx >= 0 && dx < w && dy >= 0 && dy < h && bt[dy, dx].Warrior != null && bt[dy, dx].Warrior.IsAlive && bt[dy, dx].Warrior.UTeam != this.MTeam)
            {
                SetTarget(bt[dy, dx].Warrior);
                return true;
            }

            return false;
        }
        private void SetTarget(Unit t)//найти подходящую цель
        {
            Target = t;
        }
        public void UnsetMove()
        {
            HasMoved = false;
        }
        /***********METHODS*********************/
    }
}
