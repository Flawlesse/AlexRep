using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DivineRebellion
{
    public partial class Form1 : Form
    {
        public BattleField BattleF { get; private set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void SpawnBtn_Click(object sender, EventArgs e)
        {
            BattleF.ClearFullField(true);
        }

        private void InitializeBattleFieldBtn_Click(object sender, EventArgs e)
        {
            BattleF = new BattleField();
        }
    }
}
