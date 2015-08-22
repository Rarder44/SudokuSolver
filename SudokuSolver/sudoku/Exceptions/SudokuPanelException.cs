using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver.Exceptions
{
    class SudokuPanelException : Exception
    {
        public SudokuPanelException() { }
        public SudokuPanelException(string message) : base(message) { }
        public SudokuPanelException(string message, Exception inner) : base(message, inner) { }
        protected SudokuPanelException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
            { }
    }

}
