﻿namespace SudokuSolver.Extensions
{

    public static class ObjectExtension
    {
        public static T Cast<T>(this object o)
        {
            return (T)o;
        }

    }
}
