using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Celeste.Mod.DeathCounterSheet
{
    public class DeathCounterSheetUpdater
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "DeathCounterSheet";
        static readonly string SecretPath = Path.Combine("Mods", "DeathCounterSheet", "Code", "client_secret.json");

        public static void IncrementDeathCount(string room, string spreadsheetId, string roomColumn, string deathCountColumn)
        {
            Task.Run(() =>
            {
                UserCredential credential;
                using (var stream = new FileStream(SecretPath, FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)
                    ).Result;
                }

                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // search room row
                string range = $"{roomColumn}1:{roomColumn}";
                SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(spreadsheetId, range);
                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;
                if (values == null || values.Count == 0) return;
                var i = 0;
                foreach (var row in values)
                {
                    if (row[0].ToString() == room)
                    {
                        break;
                    }
                    i++;
                }
                if (i == values.Count) return;

                // get death count
                string cell = $"{deathCountColumn}{i + 1}";
                request = service.Spreadsheets.Values.Get(spreadsheetId, cell);
                response = request.Execute();
                values = response.Values;
                int cnt = Int32.Parse(values[0][0].ToString());

                // update death count
                ValueRange valueRange = new ValueRange
                {
                    MajorDimension = "COLUMNS"
                };
                var oblist = new List<object>() { (cnt + 1).ToString() };
                valueRange.Values = new List<IList<object>> { oblist };
                SpreadsheetsResource.ValuesResource.UpdateRequest update = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, cell);
                update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                update.Execute();
            });
        }

        public static void UpdateCell(string cell, string text, string spreadsheetId)
        {
            Task.Run(() =>
            {
                UserCredential credential;
                using (var stream = new FileStream(SecretPath, FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)
                    ).Result;
                }

                var service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                ValueRange valueRange = new ValueRange
                {
                    MajorDimension = "COLUMNS"
                };
                var oblist = new List<object>() { text };
                valueRange.Values = new List<IList<object>> { oblist };
                SpreadsheetsResource.ValuesResource.UpdateRequest update = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, cell);
                update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                update.Execute();
            });
        }
    }
}
