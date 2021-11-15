using Celeste;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Celeste.Mod.Example
{
    public class DeathCounterSheetSettings : EverestModuleSettings
    {
        public bool Enabled { get; set; } = true;
    }
}
