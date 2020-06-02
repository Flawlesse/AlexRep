using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace DivineRebellion
{
    public partial class Form1 : Form
    {
        public BattleField BattleF { get; private set; }

        public Form1()
        {
            InitializeComponent();
            UnitChoiceBox.Items.Add("Melee");
            UnitChoiceBox.Items.Add("Ranged");
            UnitChoiceBox.SelectedItem = UnitChoiceBox.Items[0];
            UnitChoiceBox.SelectedIndex = 0;

            FullClearBtn.Visible = false;
            StartFightBtn.Visible = false;
            UnitChoiceBox.Visible = false;
        }

        private void FullClearBtn_Click(object sender, EventArgs e)//очищает поле целиком
        {
            if (BattleF != null)
                BattleF.ClearFullField(true);
            else
                MessageBox.Show("Initialize battle field first!");
        }

        private void InitializeBattleFieldBtn_Click(object sender, EventArgs e)
        {
            BattleF = new BattleField();
            BattleF.UnitChoice = (string)UnitChoiceBox.SelectedItem;
            InitializeBattleFieldBtn.Visible = false;
            FullClearBtn.Visible = true;
            StartFightBtn.Visible = true;
            UnitChoiceBox.Visible = true;
        }

        private void StartFightBtn_Click(object sender, EventArgs e)
        {
            if (BattleF != null)
                BattleF.StartFight();
            else
                MessageBox.Show("Initialize battle field first!");
        }

        private void UnitChoiceBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BattleF != null)
                BattleF.UnitChoice = (string)UnitChoiceBox.SelectedItem;
        }
        public void ToggleButtons()
        {
            FullClearBtn.Enabled = !FullClearBtn.Enabled;
            StartFightBtn.Enabled = !StartFightBtn.Enabled;
            UnitChoiceBox.Enabled = !UnitChoiceBox.Enabled;
        }
    }
}
