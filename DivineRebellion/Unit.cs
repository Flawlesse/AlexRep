using System;
using System.Drawing;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace DivineRebellion
{
    public enum Direction{ Left, Up, Right, Down }
    public enum DamageType{ Magic, Physical }
    public enum Team { Red, Blue }
    
    public abstract class Unit: IDisposable
    {
        bool _disposed = false;
        private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);
        public virtual void Dispose()
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

        public Bitmap Texture { get; protected set; }
        public bool IsAlive { get; set; } 
        public bool HasMoved { get; protected set; }
        public Direction Dir { get; protected set; }
        public DamageType DmgType { get; protected set; }
        public Team UTeam { get; protected set; }
        public Unit Target { get; set; }
        public int Health { get; protected set; }
        public int MaxHealth { get; protected set; }
        public int PhysDef { get; protected set; }
        public int MagDef { get; protected set; }
        public int AADmg { get; protected set; }
        public bool IsAttacking { get; protected set; }
        protected int UX, UY;//координаты
        
        public Unit(Team team, int x, int y)//constructor
        {
            IsAlive = true;
            Dir = Direction.Down;
            
            Target = null;
            
            UX = x;
            UY = y;
            UTeam = team;

            
        }
        public abstract void Move(Tile[,] bt);
        public void UnsetMove()
        {
            HasMoved = false;
        }
        public abstract bool SomeoneInRange(Tile[,] bt, int h, int w);
        public abstract void AttackTarget(BattleField bf);
        public void TakeDamage(BattleField bf, int dmg, DamageType dmt)
        {
            if (dmt == DamageType.Magic)
            {
                if (dmg - MagDef <= 0)
                    Health -= 1;
                else
                    Health -= dmg - MagDef;
            }
            else
            {
                if (dmg - PhysDef <= 0)
                    Health -= 1;
                else
                    Health -= dmg - PhysDef;
            }
            if (Health <= 0)
            {
                IsAlive = false;
                bf.OnDied(this, new TileEventArgs {X = UX, Y = UY});
            }
        }
        
        protected void SetTarget(Unit t)//найти подходящую цель
        {
            Target = t;
        }
    }
}
