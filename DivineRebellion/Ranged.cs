using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivineRebellion
{

    public class Ranged: Unit
    {

        public Ranged(Team team, int x, int y): base(team, x, y)
        {
            Image img = Image.FromFile(@"D:\VS2020\DivineRebellion\goose.png");//will be specific
            Bitmap bmp = BattleField.ResizeImage(img, BattleField.resolution, BattleField.resolution); ;
            Texture = bmp;

            UTeam = team;
            UX = x;
            UY = y;
        }
        
        public override bool SomeoneInRange(Tile[,] bt, int h, int w)
        {
            for (int y = UY - 1; y >= 0; y--)// SEARCH UPWARDS
                if (!bt[y, UX].IsFree && bt[y, UX].Warrior != null && bt[y, UX].Warrior.UTeam != this.UTeam)
                {
                    SetTarget(bt[y, UX].Warrior);
                    Dir = Direction.Up;
                    return true;
                }
            for (int y = UY + 1; y < h; y++)// SEARCH DOWNWARDS
                if (!bt[y, UX].IsFree && bt[y, UX].Warrior != null && bt[y, UX].Warrior.UTeam != this.UTeam)
                {
                    SetTarget(bt[y, UX].Warrior);
                    Dir = Direction.Down;
                    return true;
                }
            for (int x = UX - 1; x >= 0; x--)// SEARCH LEFTWARDS
                if (!bt[UY, x].IsFree && bt[UY, x].Warrior != null && bt[UY, x].Warrior.UTeam != this.UTeam)
                {
                    SetTarget(bt[UY, x].Warrior);
                    Dir = Direction.Left;
                    return true;
                }
            for (int x = UX + 1; x < w; x++)// SEARCH RIGHTWARDS
                if (!bt[UY, x].IsFree && bt[UY, x].Warrior != null && bt[UY, x].Warrior.UTeam != this.UTeam)
                {
                    SetTarget(bt[UY, x].Warrior);
                    Dir = Direction.Right;
                    return true;
                }

            return false;
        }
        public override void Move(Tile[,] bt)
        {
            if (HasMoved)
                return;

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
                switch (Dir)
                {
                    case Direction.Down:
                        bf.BattleTiles[UY + 1, UX].Missiles.Add(new Missile(this.UTeam, Dir, 1, UX, UY + 1, DmgType, AADmg));
                        break;
                    case Direction.Up:
                        bf.BattleTiles[UY - 1, UX].Missiles.Add(new Missile(this.UTeam, Dir, 1, UX, UY - 1, DmgType, AADmg));
                        break;
                    case Direction.Left:
                        bf.BattleTiles[UY, UX - 1].Missiles.Add(new Missile(this.UTeam, Dir, 1, UX - 1, UY, DmgType, AADmg));
                        break;
                    default:
                        bf.BattleTiles[UY, UX + 1].Missiles.Add(new Missile(this.UTeam, Dir, 1, UX + 1, UY, DmgType, AADmg));
                        break;
                }
            }
        }
        private double Distance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
    }
}
