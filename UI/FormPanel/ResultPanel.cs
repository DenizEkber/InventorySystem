using InventorySystem.DATABASE.CodeFirst.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace InventorySystem.UI.FormPanel
{
    public class ResultPanel : Panel
    {
        private DataGridView dgvResults;

        public ResultPanel()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.BackColor = Color.White;

            // DataGridView initialization
            this.dgvResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };

            // Create a layout to hold DataGridView
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1
            };

            layout.Controls.Add(dgvResults, 0, 0);

            this.Controls.Add(layout);
        }

        public void DisplayResults(IEnumerable<object> results)
        {
            dgvResults.DataSource = results.ToList();
        }
        
    }
}
