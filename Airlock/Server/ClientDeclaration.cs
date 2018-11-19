using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetCode;
namespace Airlock.Server
{
    [EnumerateSynchEntity]
    public class ClientDeclaration
    {
        [Synchronisable]
        public string Name { get; private set; }

        public ClientDeclaration() { }

        public ClientDeclaration(string name)
        {
            Name = name;
        }
    }
}
