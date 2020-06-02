using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DivineRebellion
{
    public class Missile
    {
        /***********PROPERTIES******************/
        public Bitmap Texture { get; private set; }
        public Direction MDir { get; private set; }
        public DamageType DmgType { get; private set; }
        public Team MTeam { get; private set; }
        private int Damage { get; set; }
        private int MX { get; set; }
        private int MY { get; set; }
        public bool HasMoved { get; private set; }
        private Unit Target { get; set; }
        /***********PROPERTIES******************/
        /***********METHODS*********************/
        public Missile(Team team, Direction dir, int x, int y, DamageType dmgt, int dmg)
        {
            string path = Environment.CurrentDirectory + @"\Textures\Missile";
            path += (team == Team.Blue) ? "Blue.png" : "Red.png";

            Image img = Image.FromFile(path);//will be specific
            Bitmap bmp = BattleField.ResizeImage(img, BattleField.resolution, BattleField.resolution * 4);
            Texture = bmp;
            img.Dispose();

            MTeam = team;
            MDir = dir;
            MX = x;
            MY = y;
            DmgType = dmgt;
            Damage = dmg;
            Target = null;
            HasMoved = true;
        }
        public void Act(BattleField bf, Tile[,] bt)
        {
            if (HasMoved)
                return;

            int h = BattleField.height / BattleField.resolution, w = BattleField.width / BattleField.resolution;
            if (SomeoneInRange(bt, h, w))
            {
                Attack(bf);
                bt[MY, MX].Missiles.Remove(this);
            }
            else
                Move(bt, h, w);
            HasMoved = true;

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
                bt[MY, MX].Missiles.Remove(this);
            
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
