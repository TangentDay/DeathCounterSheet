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
using System.IO;

namespace Celeste.Mod.Example
{
    public class DeathCounterSheet : EverestModule
    {
        public static DeathCounterSheet Instance;

        public static readonly string DeathInfoDir = Path.Combine("Saves", "DeathInfo");
        public static string DeathInfoSlotDir => Path.Combine(DeathInfoDir, SaveData.Instance?.FileSlot.ToString() ?? "-1");


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
            Everest.Events.Player.OnDie += PlayerOnDie;
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
            Everest.Events.Player.OnDie -= PlayerOnDie;
        }

        private void PlayerOnDie(Player player)
        {
            if (!Settings.Enabled) return;

            string filePath = Path.Combine(DeathInfoSlotDir, $"{DateTime.Now.Ticks}.txt");
            if (!Directory.Exists(DeathInfoSlotDir))
            {
                Directory.CreateDirectory(DeathInfoSlotDir);
            }
            File.Create(filePath).Close();
        }
    }
}
