using Celeste.Mod.UI;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.Example
{
    public class DeathCounterSheet : EverestModule
    {

        // Only one alive module instance can exist at any given time.
        public static DeathCounterSheet Instance;

        public DeathCounterSheet()
        {
            Instance = this;
        }

        public override Type SettingsType => typeof(DeathCounterSheetSettings);
        public static DeathCounterSheetSettings Settings => (DeathCounterSheetSettings)Instance._Settings;

        // Set up any hooks, event handlers and your mod in general here.
        // Load runs before Celeste itself has initialized properly.
        public override void Load()
        {
        }

        // Optional, initialize anything after Celeste has initialized itself properly.
        public override void Initialize()
        {
        }

        // Optional, do anything requiring either the Celeste or mod content here.
        public override void LoadContent(bool firstLoad)
        {
        }

        // Unload the entirety of your mod's content. Free up any native resources.
        public override void Unload()
        {
        }
    }
}
