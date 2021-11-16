using System;
using System.IO;

namespace Celeste.Mod.DeathCounterSheet
{
    public class DeathCounterSheet : EverestModule
    {
        public static DeathCounterSheet Instance;

        public static readonly string DeathInfoDir = Path.Combine("Saves", "DeathCounterSheet");
        public static string DeathInfoSlotDir => Path.Combine(DeathInfoDir, SaveData.Instance?.FileSlot.ToString() ?? "-1");

        private static Level level;


        public DeathCounterSheet()
        {
            Instance = this;
        }

        public override Type SettingsType => typeof(DeathCounterSheetSettings);
        public static DeathCounterSheetSettings Settings => (DeathCounterSheetSettings)Instance._Settings;

        public override void Load()
        {
            Everest.Events.Level.OnLoadLevel += LevelOnLoad;
            Everest.Events.Player.OnDie += PlayerOnDie;
        }

        public override void Unload()
        {
            Everest.Events.Level.OnLoadLevel -= LevelOnLoad;
            Everest.Events.Player.OnDie -= PlayerOnDie;

        }

        private void LevelOnLoad(Level _level, Player.IntroTypes playerIntro, bool isFormLoader)
        {
            level = _level;
        }

        private void PlayerOnDie(Player player)
        {
            if (!Settings.Enabled) return;

            string filePath = Path.Combine(DeathInfoSlotDir, $"{DateTime.Now.Ticks}.txt");
            if (!Directory.Exists(DeathInfoSlotDir))
            {
                Directory.CreateDirectory(DeathInfoSlotDir);
            }

            Session session = level.Session;
            File.AppendAllText(filePath, $"Chapter: {GetChapterName(session)}{Environment.NewLine}");
            File.AppendAllText(filePath, $"Room: {session.Level}{Environment.NewLine}");
            File.AppendAllText(filePath, $"GrabGolden: {session.GrabbedGolden}{Environment.NewLine}");

            if (session.GrabbedGolden && ToUpdateSheet(session))
            {
                DeathCounterSheetUpdater.Update(session.Level);
            }
        }

        private bool ToUpdateSheet(Session session)
        {
            return GetChapterName(session) == "8B";
        }

        private string GetChapterName(Session session)
        {
            string levelName = Dialog.Get(AreaData.Get(session).Name, Dialog.Languages["english"]);
            string levelMode = ((char)(session.Area.Mode + 'A')).ToString();

            switch (levelName)
            {
                case "Forsaken City":
                    levelName = "1";
                    break;
                case "Old Site":
                    levelName = "2";
                    break;
                case "Celestial Resort":
                    levelName = "3";
                    break;
                case "Golden Ridge":
                    levelName = "4";
                    break;
                case "Mirror Temple":
                    levelName = "5";
                    break;
                case "Reflection":
                    levelName = "6";
                    break;
                case "The Summit":
                    levelName = "7";
                    break;
                case "Core":
                    levelName = "8";
                    break;
            }

            if (levelName.Length == 1)
            {
                return levelName + levelMode;
            }

            if (AreaData.Get(session).Interlude)
            {
                return levelName;
            }

            if (levelName == "Farewell")
            {
                return levelName;
            }

            return levelName + " " + levelMode;
        }
    }
}
