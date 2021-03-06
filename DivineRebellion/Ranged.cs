﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace DivineRebellion
{

    public class Ranged: Unit, ICloneable
    {
        /***********IDISPOSABLE*****************/
        bool _disposed = false;
        private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~Ranged() => Dispose(false);
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
        /***********ICLONEABLE******************/
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        /***********ICLONEABLE******************/
        /***********PROPERTIES******************/
        private int movesOnReload;
        private int moveCount;
        /***********PROPERTIES******************/
        /***********METHODS*********************/
        public Ranged(Team team, int x, int y): base(team, x, y)
        {
            string path = Environment.CurrentDirectory + @"\Textures\RangeMagic";
            path += (team == Team.Blue) ? "Blue.png" : "Red.png";

            Image img = Image.FromFile(path);//will be specific
            Bitmap bmp = BattleField.ResizeImage(img, BattleField.resolution * 2, BattleField.resolution * 4); ;
            Texture = bmp;
            img.Dispose();

            DmgType = DamageType.Magic;
            PhysDef = 30;
            MagDef = 20;
            AADmg = 45;
            MaxHealth = Health = 200;

            UTeam = team;
            UX = x;
            UY = y;
            movesOnReload = 3;
            moveCount = movesOnReload - 1;//сначала заряд уже есть
        }
        
        public override bool SomeoneInRange(Tile[,] bt, int h, int w)
        {
            for (int y = UY - 1; y >= 0; y--)// SEARCH UPWARDS
                if (!bt[y, UX].IsFree && bt[y, UX].Warrior != null && bt[y, UX].Warrior.UTeam != this.UTeam)
                {
                    SetTarget(bt[y, UX].Warrior);
                    Dir = Direction.Up;
                    IsAttacking = true;
                    return true;
                }
            for (int y = UY + 1; y < h; y++)// SEARCH DOWNWARDS
                if (!bt[y, UX].IsFree && bt[y, UX].Warrior != null && bt[y, UX].Warrior.UTeam != this.UTeam)
                {
                    SetTarget(bt[y, UX].Warrior);
                    Dir = Direction.Down;
                    IsAttacking = true;
                    return true;
                }
            for (int x = UX - 1; x >= 0; x--)// SEARCH LEFTWARDS
                if (!bt[UY, x].IsFree && bt[UY, x].Warrior != null && bt[UY, x].Warrior.UTeam != this.UTeam)
                {
                    SetTarget(bt[UY, x].Warrior);
                    Dir = Direction.Left;
                    IsAttacking = true;
                    return true;
                }
            for (int x = UX + 1; x < w; x++)// SEARCH RIGHTWARDS
                if (!bt[UY, x].IsFree && bt[UY, x].Warrior != null && bt[UY, x].Warrior.UTeam != this.UTeam)
                {
                    SetTarget(bt[UY, x].Warrior);
                    Dir = Direction.Right;
                    IsAttacking = true;
                    return true;
                }

            return false;
        }
        private bool InPossibleRange(Tile[,] bt, int h, int w, int rx, int ry)
        {
            for (int y = ry - 1; y >= 0; y--)// SEARCH UPWARDS
                if (!bt[y, rx].IsFree && bt[y, rx].Warrior != null && bt[y, rx].Warrior.UTeam != this.UTeam)           
                    return true;
           
            for (int y = ry + 1; y < h; y++)// SEARCH DOWNWARDS
                if (!bt[y, rx].IsFree && bt[y, rx].Warrior != null && bt[y, rx].Warrior.UTeam != this.UTeam)
                    return true;
          
            for (int x = rx - 1; x >= 0; x--)// SEARCH LEFTWARDS
                if (!bt[ry, x].IsFree && bt[ry, x].Warrior != null && bt[ry, x].Warrior.UTeam != this.UTeam)
                    return true;
            for (int x = rx + 1; x < w; x++)// SEARCH RIGHTWARDS
                if (!bt[ry, x].IsFree && bt[ry, x].Warrior != null && bt[ry, x].Warrior.UTeam != this.UTeam)
                    return true;

            return false;
        }
        public override void Move(Tile[,] bt)
        {
            if (HasMoved)
                return;

            if ((moveCount + 1) % movesOnReload != 0)
                moveCount++;

            IsAttacking = false;
            List<int> listCoordsX = new List<int>();
            List<int> ListCoordsY = new List<int>();
            int rx = UX, ry = UY;

            int h = BattleField.height / BattleField.resolution, w = BattleField.width / BattleField.resolution;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    if (!bt[y, x].IsFree && bt[y, x].Warrior != null && bt[y, x].Warrior.UTeam != this.UTeam)
                    {
                        listCoordsX.Add(x);
                        ListCoordsY.Add(y);
                    }

            double mindist = 1000;
            int[] coordsX = listCoordsX.ToArray(), coordsY = ListCoordsY.ToArray();
            listCoordsX.Clear();
            ListCoordsY.Clear();

            int n = coordsX.Length;
            double tmp;
            int bestx = UX, besty = UY;
            for (int i = 0; i < n; i++)
            {
                tmp = Distance(rx, ry, coordsX[i], coordsY[i]);
                if (tmp < mindist)
                    mindist = tmp;

            }//ON PLACE

            ry = UY - 1; //UPWARDS
            rx = UX;
            if (ry >= 0 && ry < h && bt[ry, rx].IsFree)
            {
                if (InPossibleRange(bt, BattleField.height / BattleField.resolution, BattleField.width / BattleField.resolution, rx, ry))
                {
                    bt[ry, rx].Warrior = this;
                    bt[ry, rx].IsFree = false;
                    bt[UY, UX].Warrior = null;
                    bt[UY, UX].IsFree = true;
                    UY = ry;
                    UX = rx;
                    HasMoved = true;
                    Dir = Direction.Up;
                    return;
                }
                for (int i = 0; i < n; i++)
                {
                    tmp = Distance(rx, ry, coordsX[i], coordsY[i]);
                    if (tmp < mindist)
                    {
                        mindist = tmp;
                        bestx = rx;
                        besty = ry;
                        Dir = Direction.Up;
                    }
                }
            }

            ry = UY + 1; //DOWNWARDS
            rx = UX;
            
            if (ry >= 0 && ry < h && bt[ry, rx].IsFree)
            {
                if (InPossibleRange(bt, BattleField.height / BattleField.resolution, BattleField.width / BattleField.resolution, rx, ry))
                {
                    bt[ry, rx].Warrior = this;
                    bt[ry, rx].IsFree = false;
                    bt[UY, UX].Warrior = null;
                    bt[UY, UX].IsFree = true;
                    UY = ry;
                    UX = rx;
                    HasMoved = true;
                    Dir = Direction.Down;
                    return;
                }
                for (int i = 0; i < n; i++)
                {
                    tmp = Distance(rx, ry, coordsX[i], coordsY[i]);
                    if (tmp < mindist)
                    {
                        mindist = tmp;
                        bestx = rx;
                        besty = ry;
                        Dir = Direction.Down;
                    }
                }
            }

            ry = UY; //LEFTWARDS
            rx = UX - 1;
            if (rx >= 0 && rx < w && bt[ry, rx].IsFree)
            {
                if (InPossibleRange(bt, BattleField.height / BattleField.resolution, BattleField.width / BattleField.resolution, rx, ry))
                {
                    bt[ry, rx].Warrior = this;
                    bt[ry, rx].IsFree = false;
                    bt[UY, UX].Warrior = null;
                    bt[UY, UX].IsFree = true;
                    UY = ry;
                    UX = rx;
                    HasMoved = true;
                    Dir = Direction.Left;
                    return;
                }
                for (int i = 0; i < n; i++)
                {
                    tmp = Distance(rx, ry, coordsX[i], coordsY[i]);
                    if (tmp < mindist)
                    {
                        mindist = tmp;
                        bestx = rx;
                        besty = ry;
                        Dir = Direction.Left;
                    }
                }
            }

            ry = UY; //RIGHTWARDS
            rx = UX + 1;
            if (rx >= 0 && rx < w && bt[ry, rx].IsFree)
            {
                if (InPossibleRange(bt, BattleField.height / BattleField.resolution, BattleField.width / BattleField.resolution, rx, ry))
                {
                    bt[ry, rx].Warrior = this;
                    bt[ry, rx].IsFree = false;
                    bt[UY, UX].Warrior = null;
                    bt[UY, UX].IsFree = true;
                    UY = ry;
                    UX = rx;
                    HasMoved = true;
                    Dir = Direction.Right;
                    return;
                }
                for (int i = 0; i < n; i++)
                {
                    tmp = Distance(rx, ry, coordsX[i], coordsY[i]);
                    if (tmp < mindist)
                    {
                        mindist = tmp;
                        bestx = rx;
                        besty = ry;
                        Dir = Direction.Right;
                    }
                }

            }

            if (bestx != UX || besty != UY)
            {
                bt[besty, bestx].Warrior = this;
                bt[besty, bestx].IsFree = false;
                bt[UY, UX].Warrior = null;
                bt[UY, UX].IsFree = true;
                UY = besty;
                UX = bestx;
            }

            HasMoved = true;
            //будем находить клетку с минимальным весом с помощью дистанции (простой алгоритм длины между двух точек)
        }
        public override void AttackTarget(BattleField bf)
        {
            //выпустить снаряд с нужными характеристиками в нужном направлении
            if (Target != null)
            {
                if ((moveCount + 1) % movesOnReload != 0)
                {
                    moveCount++;
                    return;
                }

                moveCount = 0;
                switch (Dir)
                {
                    case Direction.Down:
                        bf.BattleTiles[UY + 1, UX].Missiles.Add(new Missile(this.UTeam, Dir, UX, UY + 1, DmgType, AADmg));
                        break;
                    case Direction.Up:
                        bf.BattleTiles[UY - 1, UX].Missiles.Add(new Missile(this.UTeam, Dir, UX, UY - 1, DmgType, AADmg));
                        break;
                    case Direction.Left:
                        bf.BattleTiles[UY, UX - 1].Missiles.Add(new Missile(this.UTeam, Dir, UX - 1, UY, DmgType, AADmg));
                        break;
                    default:
                        bf.BattleTiles[UY, UX + 1].Missiles.Add(new Missile(this.UTeam, Dir, UX + 1, UY, DmgType, AADmg));
                        break;
                }
            }
        }
        private double Distance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
        /***********METHODS*********************/
    }
}
