using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivineRebellion
{
    public class Tile
    {
        public Unit Warrior { get; set; }
        public bool IsFree { get; set; }
        public Tile()
        {
            Warrior = null;
            IsFree = true;
        }
        
    }
}
