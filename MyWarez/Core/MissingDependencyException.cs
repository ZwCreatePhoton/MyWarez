using System;
using System.Collections.Generic;
using System.Text;

namespace MyWarez.Core
{
    public class MissingDependencyException : Exception
    {
        public MissingDependencyException(string dependency) : base($"[!] The dependency '{dependency}' is not found.") { }
    }
}
