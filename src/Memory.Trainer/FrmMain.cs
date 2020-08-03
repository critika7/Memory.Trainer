using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LegoCityUnderCover.Trainer.Models;

namespace LegoCityUnderCover.Trainer
{
    public partial class FrmMain : Form
    {
        private MemoryHandler _memoryHandler;
        private ProcessInfo _currentProcess;

        private Dictionary<Button,TextBox> _controls = new Dictionary<Button, TextBox>();
        public FrmMain()
        {
            InitializeComponent();

        }

        private void OnFrmMainLoad(object sender, EventArgs e)
        {
            foreach (var file in Directory.GetFiles("MemoryFiles"))
            {
                cmbProcesses.Items.Add(ProcessInfo.ReadFile(file));
            }
        }



        private readonly Size _textBoxSize = new Size(286, 20);
        private void OnBtnLoadClick(object sender, System.EventArgs e)
        {
            if (cmbProcesses.SelectedItem == null)
                return;

            foreach (var ctrl in _controls)
            {
                Controls.Remove(ctrl.Key);
                Controls.Remove((Label)ctrl.Value.Tag);
                Controls.Remove(ctrl.Value);
            }

            _controls.Clear();

            _currentProcess = cmbProcesses.SelectedItem as ProcessInfo;

            Text = _currentProcess.Name +" trainer";
            _memoryHandler = new MemoryHandler(_currentProcess);
            _memoryHandler.AttachedToProcess += OnAttachedToProcess;


            var lastYPos = 78;
            var third = 0;
            var second = 0;
            for (int i = 0; i < _currentProcess.MemoryAddresses.Count; i++)
            {
                var address = _currentProcess.MemoryAddresses[i];
                

                if (third == 2)
                {
                    lastYPos += 45;
                    third = 0;
                }

                if (second == 2)
                    second = 0;

                var label = new Label
                {
                    Size = new Size(286, 13),
                    Location = new Point(12 + second * 373, lastYPos - 18),
                    Text = address.Name + ":"
                };

                var textBox = new TextBox
                {
                    Size = _textBoxSize,
                    Location = new Point(12 + second * 373, lastYPos),
                    Tag = label
                };
                textBox.Text = _memoryHandler.GetValue(address.Name,address.Type);
                var button = new Button
                {
                    Size = new Size(75, 23),
                    Location = new Point(304 + second * 373, lastYPos - 2),
                    Text = "Set",
                    Tag = address
                };

                var cb = new CheckBox
                {
                    Size = new Size(70, 17),
                    Location = new Point(304 + second * 373, lastYPos - 18),
                    Text = "Freeze",
                    Tag = button
                };
                cb.CheckedChanged += OnCheckedChanged;
                button.Click += OnButtonClick;
                Controls.Add(cb);
                Controls.Add(label);
                Controls.Add(textBox);
                Controls.Add(button);
                _controls.Add(button,textBox);
                second++;
                third++;
            }

            btnEditLayout.Visible = true;
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox cb)
            {
                var button = (Button)cb.Tag;
                var address = (MemAddress)button.Tag;
                _memoryHandler.WriteValue(address.Name, address.Type, _controls[button].Text, cb.Checked);
            }
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                var address = (MemAddress)btn.Tag;
                _memoryHandler.WriteValue(address.Name, address.Type, _controls[btn].Text);
            }
        }

        private void OnAttachedToProcess(object sender, bool e)
        {
            Invoke((MethodInvoker) delegate { Text = _currentProcess.Name + " trainer | ATTACHED"; });
        }
    }
}
