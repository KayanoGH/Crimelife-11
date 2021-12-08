using System;
using System.Collections.Generic;
using System.Text;

namespace Crimelife
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DisabledModuleAttribute : Attribute
    {
    }
}
