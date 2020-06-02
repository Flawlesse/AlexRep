using System;
using System.Collections.Generic;

namespace DivineRebellion
{
    public class Tile: ICloneable
    {
        public Unit Warrior { get; set; }
        public bool IsFree { get; set; }
        public List<Missile> Missiles;
        public Tile()
        {
            Warrior = null;
            IsFree = true;
            Missiles = new List<Missile>();
        }

        public object Clone()
        {
            return new Tile
            {
                Warrior = (Unit)((this.Warrior == null) ? null : ((this.Warrior is Melee) ? ((Melee)this.Warrior).Clone() : ((Ranged)this.Warrior).Clone())),
                IsFree = this.IsFree,
                Missiles = new List<Missile>()
            };
        }
    }
}
