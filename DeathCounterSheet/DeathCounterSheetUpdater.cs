﻿using Celeste.Mod.UI;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        static string ApplicationName = "DeathCounterSheet";
        static string SecretPath = Path.Combine("Mods", "DeathCounterSheet", "Code", "client_secret.json");
        static string SpreadsheetId = "1jxfCVubwmqMIpt77G9E86R3uRGYaWPGahqo-nAGA1n0";
        static string SheetName = "Sheet1";
        static string RoomColumn = "B";
        static string DeathCountColumn = "E";

        public static void Update(string room)
        {
            UserCredential credential;

            // DownloadしてきたJsonファイル
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
            string range = $"{RoomColumn}1:{RoomColumn}";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(SpreadsheetId, range);
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
            string cell = $"{DeathCountColumn}{i + 1}";
            request = service.Spreadsheets.Values.Get(SpreadsheetId, cell);
            response = request.Execute();
            values = response.Values;
            int cnt = Int32.Parse(values[0][0].ToString());

            // update death count
            ValueRange valueRange = new ValueRange();
            valueRange.MajorDimension = "COLUMNS";
            var oblist = new List<object>() { (cnt + 1).ToString() };
            valueRange.Values = new List<IList<object>> { oblist };
            SpreadsheetsResource.ValuesResource.UpdateRequest update = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, cell);
            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            update.Execute();
        }

    }
}