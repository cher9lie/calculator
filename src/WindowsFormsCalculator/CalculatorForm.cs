using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsCalculator
{
    public partial class CalculatorForm : Form
    {
        private CalculationEngine _calculationEngine;
        private CalculationHistory _calculationHistory;
        private bool _isScientificMode;

        public CalculatorForm()
        {
            InitializeComponent();
            InitializeCalculator();
        }

        private void InitializeCalculator()
        {
            _calculationEngine = new CalculationEngine();
            _calculationHistory = new CalculationHistory();
            _isScientificMode = false;
            
            UpdateDisplay();
            UpdateModeDisplay();
        }

        private void UpdateDisplay()
        {
            displayTextBox.Text = _calculationEngine.CurrentDisplay;
            expressionLabel.Text = _calculationEngine.CurrentExpression;
        }

        private void UpdateModeDisplay()
        {
            var mode = _isScientificMode ? CalculatorMode.Scientific : CalculatorMode.Standard;
            _calculationEngine.Mode = mode;
            
            Text = $"Calculator - {(_isScientificMode ? "Scientific" : "Standard")} Mode";
            
            // Show/hide scientific buttons
            var scientificVisible = _isScientificMode;
            btnSin.Visible = scientificVisible;
            btnCos.Visible = scientificVisible;
            btnTan.Visible = scientificVisible;
            btnLog.Visible = scientificVisible;
            btnLn.Visible = scientificVisible;
            btnExp.Visible = scientificVisible;
            btnPower.Visible = scientificVisible;
            btnSqrt.Visible = scientificVisible;
            
            // Adjust form size based on mode
            if (_isScientificMode)
            {
                Size = new Size(420, 380);
            }
            else
            {
                Size = new Size(320, 380);
            }
        }

        private void DigitButton_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            _calculationEngine.InputDigit(button.Text);
            UpdateDisplay();
        }

        private void OperatorButton_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            OperationType operation = OperationType.None;

            switch (button.Text)
            {
                case "+": operation = OperationType.Add; break;
                case "−": operation = OperationType.Subtract; break;
                case "×": operation = OperationType.Multiply; break;
                case "÷": operation = OperationType.Divide; break;
                case "=": operation = OperationType.Equals; break;
                case "±": operation = OperationType.ChangeSign; break;
                case "%": operation = OperationType.Percent; break;
                case "√": operation = OperationType.Sqrt; break;
                case "1/x": operation = OperationType.Reciprocal; break;
                case "sin": operation = OperationType.Sin; break;
                case "cos": operation = OperationType.Cos; break;
                case "tan": operation = OperationType.Tan; break;
                case "log": operation = OperationType.Log10; break;
                case "ln": operation = OperationType.Log; break;
                case "exp": operation = OperationType.Exp; break;
                case "x^y": operation = OperationType.Power; break;
            }

            if (operation != OperationType.None)
            {
                var previousExpression = _calculationEngine.CurrentExpression;
                var previousResult = _calculationEngine.CurrentDisplay;
                
                _calculationEngine.PerformOperation(operation);
                UpdateDisplay();

                // Add to history if it's an equals operation
                if (operation == OperationType.Equals && !_calculationEngine.HasError)
                {
                    var expression = _calculationEngine.GetCompleteExpression();
                    if (!string.IsNullOrEmpty(expression))
                    {
                        _calculationHistory.AddCalculation(
                            expression, 
                            _calculationEngine.CurrentDisplay, 
                            _calculationEngine.Mode);
                    }
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _calculationEngine.PerformOperation(OperationType.Clear);
            UpdateDisplay();
        }

        private void btnClearEntry_Click(object sender, EventArgs e)
        {
            _calculationEngine.PerformOperation(OperationType.ClearEntry);
            UpdateDisplay();
        }

        private void btnModeSwitch_Click(object sender, EventArgs e)
        {
            _isScientificMode = !_isScientificMode;
            UpdateModeDisplay();
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            ShowHistoryDialog();
        }

        private void ShowHistoryDialog()
        {
            using (var historyForm = new Form())
            {
                historyForm.Text = "Calculation History";
                historyForm.Size = new Size(500, 400);
                historyForm.StartPosition = FormStartPosition.CenterParent;

                var listBox = new ListBox();
                listBox.Dock = DockStyle.Fill;
                listBox.Font = new Font("Consolas", 10);

                var history = _calculationHistory.GetHistory(_calculationEngine.Mode);
                foreach (var item in history)
                {
                    listBox.Items.Add(item.ToString());
                }

                var panel = new Panel();
                panel.Dock = DockStyle.Bottom;
                panel.Height = 40;

                var btnClearHistory = new Button();
                btnClearHistory.Text = "Clear History";
                btnClearHistory.Size = new Size(100, 30);
                btnClearHistory.Location = new Point(10, 5);
                btnClearHistory.Click += (s, e) =>
                {
                    _calculationHistory.ClearHistory(_calculationEngine.Mode);
                    listBox.Items.Clear();
                };

                var btnClose = new Button();
                btnClose.Text = "Close";
                btnClose.Size = new Size(80, 30);
                btnClose.Location = new Point(120, 5);
                btnClose.Click += (s, e) => historyForm.Close();

                panel.Controls.Add(btnClearHistory);
                panel.Controls.Add(btnClose);

                historyForm.Controls.Add(listBox);
                historyForm.Controls.Add(panel);

                historyForm.ShowDialog(this);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Handle keyboard input
            switch (keyData)
            {
                case Keys.D0:
                case Keys.NumPad0:
                    _calculationEngine.InputDigit("0");
                    UpdateDisplay();
                    return true;
                case Keys.D1:
                case Keys.NumPad1:
                    _calculationEngine.InputDigit("1");
                    UpdateDisplay();
                    return true;
                case Keys.D2:
                case Keys.NumPad2:
                    _calculationEngine.InputDigit("2");
                    UpdateDisplay();
                    return true;
                case Keys.D3:
                case Keys.NumPad3:
                    _calculationEngine.InputDigit("3");
                    UpdateDisplay();
                    return true;
                case Keys.D4:
                case Keys.NumPad4:
                    _calculationEngine.InputDigit("4");
                    UpdateDisplay();
                    return true;
                case Keys.D5:
                case Keys.NumPad5:
                    _calculationEngine.InputDigit("5");
                    UpdateDisplay();
                    return true;
                case Keys.D6:
                case Keys.NumPad6:
                    _calculationEngine.InputDigit("6");
                    UpdateDisplay();
                    return true;
                case Keys.D7:
                case Keys.NumPad7:
                    _calculationEngine.InputDigit("7");
                    UpdateDisplay();
                    return true;
                case Keys.D8:
                case Keys.NumPad8:
                    _calculationEngine.InputDigit("8");
                    UpdateDisplay();
                    return true;
                case Keys.D9:
                case Keys.NumPad9:
                    _calculationEngine.InputDigit("9");
                    UpdateDisplay();
                    return true;
                case Keys.Decimal:
                case Keys.OemPeriod:
                    _calculationEngine.InputDigit(".");
                    UpdateDisplay();
                    return true;
                case Keys.Add:
                    _calculationEngine.PerformOperation(OperationType.Add);
                    UpdateDisplay();
                    return true;
                case Keys.Subtract:
                    _calculationEngine.PerformOperation(OperationType.Subtract);
                    UpdateDisplay();
                    return true;
                case Keys.Multiply:
                    _calculationEngine.PerformOperation(OperationType.Multiply);
                    UpdateDisplay();
                    return true;
                case Keys.Divide:
                    _calculationEngine.PerformOperation(OperationType.Divide);
                    UpdateDisplay();
                    return true;
                case Keys.Enter:
                case Keys.Oemplus when (ModifierKeys & Keys.Shift) == Keys.Shift: // = key
                    _calculationEngine.PerformOperation(OperationType.Equals);
                    UpdateDisplay();
                    return true;
                case Keys.Escape:
                    _calculationEngine.PerformOperation(OperationType.Clear);
                    UpdateDisplay();
                    return true;
                case Keys.Delete:
                    _calculationEngine.PerformOperation(OperationType.ClearEntry);
                    UpdateDisplay();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}