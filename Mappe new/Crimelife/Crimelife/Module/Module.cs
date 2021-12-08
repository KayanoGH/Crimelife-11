using System;
using System.Collections.Generic;
using System.Text;
using GVMP;

namespace Crimelife.Module
{
    public abstract class Module<T> : BaseModule where T : Crimelife.Module.Module<T>
    {
        public static T Instance { get; private set; }

        protected Module() => Crimelife.Module.Module<T>.Instance = (T)this;
    }
}
