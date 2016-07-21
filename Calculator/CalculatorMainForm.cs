//-----------------------------------------------------------------------
// <copyright file="CalculatorMainForm.cs" company="Proxiad Bulgaria">
//     Copyright (c) Proxiad Bulgaria. All rights reserved.
// </copyright>
// <author>Ilia Anastassov</author>
//-----------------------------------------------------------------------
namespace Calculator
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using Contracts;
    using Factories;

    // TODO: 

    /// <summary>
    /// Main form for the calculator
    /// </summary>
    public partial class CalculatorMainForm : Form
    {
        private const int MaxInputLenth = 18;
        private const int MaxResultLenth = 14;
        private Font bigFont = new Font("Microsoft Sans Serif", 32F, FontStyle.Bold, GraphicsUnit.Point, 204);
        private Font midFont = new Font("Microsoft Sans Serif", 28F, FontStyle.Bold, GraphicsUnit.Point, 204);
        private Font smallFont = new Font("Microsoft Sans Serif", 26F, FontStyle.Bold, GraphicsUnit.Point, 204);

        private List<double> valueList = new List<double>(8); // Estimation: average use of less than 8 values per application run
        private List<string> operatorList = new List<string>(7); // Estimation: average operators = average values - 1
        private double valueOne = 0;
        private double valueTwo = 0;
        private double tempResult = 0;
        private string currentOperator = string.Empty;
        private bool operatorPressed = false;
        private bool equalsPressed = false;
        private bool invalidOperation = false;
        private StringBuilder equation = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorMainForm" /> class
        /// </summary>
        public CalculatorMainForm()
        {
            InitializeComponent();
        }

        // Event handler of a number button
        private void ButtonNum_Click(object sender, EventArgs e)
        {
            if (resultBox.Text == "0" || operatorPressed || equalsPressed)
            {
                resultBox.Clear();
                operatorPressed = false;
                equalsPressed = false;
                invalidOperation = false;
            }

            // Cast the object sender as Button in order to get the value of the current button
            Button currentButton = sender as Button;

            // Append the button value if the value entered does not exceed the max allowed
            if (resultBox.Text.Length < MaxInputLenth)
            {
                resultBox.Text += currentButton.Text;
            }

            UpdateFontSize();
        }

        // Event handler of a operator button
        private void ButtonOperator_Click(object sender, EventArgs e)
        {
            Button currentOperatorBtn = sender as Button;
            currentOperator = currentOperatorBtn.Text;

            // Only if the operation before is valid
            if (!invalidOperation)
            {
                // Do when operator button is pressed for the first time
                if (!operatorPressed)
                {
                    valueList.Add(double.Parse(resultBox.Text));
                    operatorList.Add(currentOperator);
                    equation.Append(resultBox.Text)
                            .Append(" ")
                            .Append(currentOperator)
                            .Append(" ");

                    // Calculate the result only for the first value and update font size
                    if (valueList.Count > 1)
                    {
                        CalculateResult();
                        UpdateFontSize();
                    }
                }
                else
                {
                    // If clicked multiple times replace old operator by new one
                    operatorList[operatorList.Count - 1] = currentOperator;
                    equation.Remove(equation.Length - 2, 2);
                    equation.Append(currentOperator)
                            .Append(" ");
                }

                // Mark that an operator is pressed and operation as valid
                operatorPressed = true;

                // Show the current equation on the screen
                equationTextBox.Text = equation.ToString();
            }
        }

        // Event handler of the equals button
        private void ButtonEquals_Click(object sender, EventArgs e)
        {
            // Only if the operation before is valid
            if (!invalidOperation)
            {
                equalsPressed = true;

                valueList.Add(double.Parse(resultBox.Text));

                if (valueList.Count > 1)
                {
                    CalculateResult();
                }

                ResetCalculator();
            }
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            resultBox.Text = "0";
            ResetCalculator();
            invalidOperation = false;
        }

        private void ButtonClearEntry_Click(object sender, EventArgs e)
        {
            resultBox.Text = "0";
            invalidOperation = false;
        }

        private void ButtonPoint_Click(object sender, EventArgs e)
        {
            if (!resultBox.Text.Contains(".") && !invalidOperation && resultBox.Text.Length < MaxResultLenth)
            {
                resultBox.Text += ".";
            }
        }

        private void ButtonPlusMinus_Click(object sender, EventArgs e)
        {
            // Only if the operation before is valid
            if (!invalidOperation && resultBox.Text != "0")
            {
                // Inverts the sign of the current value
                if (resultBox.Text[0] == '-')
                {
                    resultBox.Text = resultBox.Text.Remove(0, 1);
                }
                else
                {
                    resultBox.Text = "-" + resultBox.Text;
                }
            }
        }

        private void ButtonDel_Click(object sender, EventArgs e)
        {
            // Only if the operation before is valid
            if (!invalidOperation && !equalsPressed)
            {
                if (resultBox.Text.Length > 2)
                {
                    resultBox.Text = resultBox.Text.Remove(resultBox.Text.Length - 1, 1);
                }
                else
                {
                    if (resultBox.Text.Contains("-"))
                    {
                        resultBox.Text = "0";
                    }
                    else
                    {
                        if (resultBox.Text.Length > 1)
                        {
                            resultBox.Text = resultBox.Text.Remove(resultBox.Text.Length - 1, 1);
                        }
                        else
                        {
                            resultBox.Text = "0";
                        }
                    }
                }

                UpdateFontSize();
            }
        }

        private void CalculateResult()
        {
            // Calculate only if there are at least two values in the values list
            if (valueList.Count >= 2)
            {
                valueOne = valueList[valueList.Count - 2];
                valueTwo = valueList[valueList.Count - 1];
                currentOperator = operatorList[valueList.Count - 2]; // The operators are allways one less then the values

                // In case of division by zero
                if (valueTwo == 0 && currentOperator == @"/")
                {
                    resultBox.Text = "Invalid operation";
                    invalidOperation = true;
                    ResetCalculator();
                }
                else
                {
                    // Gets the operation based upon the current operator
                    var operation = OperationFactory.GetOperation(currentOperator);
                    tempResult = operation.Calculate(valueOne, valueTwo);

                    // Show the result on the screen in the right format and update font size
                    resultBox.Text = FormatOutput(tempResult);
                    UpdateFontSize();

                    // Set the last value to the result of the last operation
                    // On the next iteration valueOne will be set to this value, and valueTwo - to the new last value
                    valueList[valueList.Count - 1] = tempResult;
                }
            }
        }

        // Formats the output
        private string FormatOutput(double result)
        {
            if (result.ToString().Length > MaxResultLenth)
            {
                return result.ToString("e7");
            }
            else
            {
                // In case of a decimal number
                if (result.ToString().Contains("."))
                {
                    return string.Format($"{result:N10}").TrimEnd('0');
                }
                else
                {
                    return string.Format($"{result:N0}");
                }
            }
        }

        // Reset the calculator to initial state
        private void ResetCalculator()
        {
            valueList.Clear();
            operatorList.Clear();
            valueOne = 0;
            valueTwo = 0;
            tempResult = 0;
            currentOperator = string.Empty;
            operatorPressed = false;
            equation.Clear();
            equationTextBox.Clear();
            UpdateFontSize();
        }

        // Update the fint size
        private void UpdateFontSize()
        {
            if (resultBox.Text.Length <= 14)
            {
                resultBox.Font = bigFont;
            }
            else if (resultBox.Text.Length > 14 && resultBox.Text.Length <= 16)
            {
                resultBox.Font = midFont;
            }
            else
            {
                resultBox.Font = smallFont;
            }
        }

        // Key map
        private void CalculatorMainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '0':
                    button0.PerformClick();
                    break;
                case '1':
                    button1.PerformClick();
                    break;
                case '2':
                    button2.PerformClick();
                    break;
                case '3':
                    button3.PerformClick();
                    break;
                case '4':
                    button4.PerformClick();
                    break;
                case '5':
                    button5.PerformClick();
                    break;
                case '6':
                    button6.PerformClick();
                    break;
                case '7':
                    button7.PerformClick();
                    break;
                case '8':
                    button8.PerformClick();
                    break;
                case '9':
                    button9.PerformClick();
                    break;
                case '.':
                    buttonPoint.PerformClick();
                    break;
                case '+':
                    buttonPlus.PerformClick();
                    break;
                case '-':
                    buttonMinus.PerformClick();
                    break;
                case '*':
                    buttonMuliplication.PerformClick();
                    break;
                case '/':
                    buttonDivision.PerformClick();
                    break;
                case '=':
                case (char)Keys.Enter:
                    buttonEquals.PerformClick();
                    break;
                case (char)Keys.Escape:
                    buttonClear.PerformClick();
                    break;
                case (char)Keys.Back:
                    buttonDel.PerformClick();
                    break;
            }
        }
    }
}