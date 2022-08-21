using PulsarModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PulsarModLoader.PulsarMod;

namespace CompSearch
{
    public class Plugin : PulsarMod
    {
        public override string HarmonyIdentifier()
        {
            return "com.GX++";
        }
        public override string Author => "Rayman";
        public override string ShortDescription => "Adds (almost) all components and items to Science GX screen";
        public override string Name => "CompSearch";
        public override string Version => "0.0.4";
    }
}
