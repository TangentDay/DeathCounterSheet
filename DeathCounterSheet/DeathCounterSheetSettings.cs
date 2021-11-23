namespace Celeste.Mod.DeathCounterSheet
{
    public class DeathCounterSheetSettings : EverestModuleSettings
    {
        public bool Enabled { get; set; } = true;

        [SettingMaxLength(8)]
        public string TargetChapter { get; set; } = "1A";

        [SettingMaxLength(50)]
        public string SpreadsheetId { get; set; } = "0123456789";

        [SettingMaxLength(2)]
        public string RoomColumn { get; set; } = "A";

        [SettingMaxLength(2)]
        public string DeathCountColumn { get; set; } = "B";

        [SettingMaxLength(2)]
        public string RoomCell { get; set; } = "";
    }
}
