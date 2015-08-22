using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver.Exceptions
{
    class SudokuSolverException : Exception
    {
        public SudokuSolverException() { }
        public SudokuSolverException(string message) : base(message) { }
        public SudokuSolverException(string message, Exception inner) : base(message, inner) { }
        protected SudokuSolverException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }

}
