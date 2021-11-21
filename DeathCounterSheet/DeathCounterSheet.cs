using System;
using System.IO;
using System.Collections.Generic;

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
            string room = GetRoomCp(session);
            File.AppendAllText(filePath, $"Chapter: {GetChapterName(session)}{Environment.NewLine}");
            File.AppendAllText(filePath, $"Room: {room}{Environment.NewLine}");
            File.AppendAllText(filePath, $"GrabGolden: {session.GrabbedGolden}{Environment.NewLine}");

            if (session.GrabbedGolden && ToUpdateSheet(session))
            {
                DeathCounterSheetUpdater.Update(
                    room, Settings.SpreadsheetId, Settings.RoomColumn, Settings.DeathCountColumn);
            }
        }

        private bool ToUpdateSheet(Session session)
        {
            return GetChapterName(session) == Settings.TargetChapter;
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

        private string GetRoomCp(Session session)
        {
            HashSet<string> flags = session.Flags;

            if (flags.Count == 0)
            {
                return session.Level;
            }

            if (GetChapterName(session) == "7B" && session.Level == "g-03")
            {
                return session.Level;
            }

            string prefix = "summit_checkpoint_";
            int min = 20;
            foreach (string flag in flags)
            {
                if (flag.StartsWith(prefix))
                {
                    string num = flag.Substring(prefix.Length);
                    min = Math.Min(min, Int32.Parse(num));
                }
            }
            return $"cp{min}";
        }
    }
}
