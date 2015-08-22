using SudokuSolver.Util;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SudokuSolver.Extensions
{
    public class TextBoxPlus:TextBox
    {
        Dictionary<NumberType, Color> TextBoxColor = new Dictionary<NumberType, Color>();

        
        NumberType _Type = NumberType.InsertByUser;
        public NumberType Type
        {
            get { return _Type; }
            set {
                
                if (this.InvokeRequired)
                    this.Invoke((MethodInvoker)delegate { SetType(value); });
                else
                {
                    SetType(value);
                }
                
            }
        }
        
        private void SetType(NumberType n)
        {
            _Type = n;
            ForeColor = TextBoxColor[_Type];
        }
        public TextBoxPlus():base()
        {
            TextBoxColor.Add(NumberType.InsertByProgram, Color.Black);
            TextBoxColor.Add(NumberType.InsertByUser, Color.Red);

        }
    }
}
