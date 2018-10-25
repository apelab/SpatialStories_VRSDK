using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Enum<T> where T : struct, IConvertible
{
    public static int Count
    {
        get
        {
            return Enum.GetNames(typeof(T)).Length;
        }
    }
}
