﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GoogleSheet.Helper;

class GoogleSheetHelper {

    static string[] Scopes = { SheetsService.Scope.Spreadsheets };
    static string ApplicationName = "GoogleSheetHelper";
    static string SheetID = "";
    static string credentials = "";
    static GoogleCredential c;
    SheetsService service;

    public GoogleSheetHelper(string sheetID, string credentialsPath, string applicationName) {

        SheetID = sheetID;
        credentials = credentialsPath;
        ApplicationName = applicationName;
        c = GoogleCredential.FromFile(credentials).CreateScoped(Scopes);
        service = InitializeSheetsService();

    }

    public bool WriteToCell(string writeValue, string index) {

        var service = new SheetsService(new BaseClientService.Initializer() {

            HttpClientInitializer = c,
            ApplicationName = ApplicationName,

        });

        var range = new ValueRange();
        var objectList = new List<object>() { writeValue };
        range.Values = new List<IList<object>> { objectList };
        var r = service.Spreadsheets.Values.Update(range, SheetID, index);
        r.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        
        try {

            var response = r.Execute();

        } catch (Google.GoogleApiException e) {

            Console.WriteLine("Error updating values: {0}", e.Message);
            Console.WriteLine("Error status code: {0}", e.Error.Code);
            Console.WriteLine("Error message: {0}", e.Error.Message);
            return false;
        }
        return true;
    }

    public string? SearchAtIndex(string searchFor, string indexRange) {

        var service = new SheetsService(new BaseClientService.Initializer() {

            HttpClientInitializer = c,
            ApplicationName = ApplicationName,

        });

        var r = service.Spreadsheets.Values.Get(SheetID, indexRange);
        r.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.FORMATTEDVALUE;
        var response = r.Execute();
        var values = response.Values;

        if (values != null && values.Count > 0) {
        
            for (int i = 0; i < values.Count; i++) {

                if (values[i][0].ToString() == searchFor) {
                    
                    return values[i][0].ToString();

                }

            }

            
        }
        else {

            return "No data found.";

        }
        return null;

    }

    public string? ReadFromCell(string readIndex) {

        var service = new SheetsService(new BaseClientService.Initializer() {

            HttpClientInitializer = c,
            ApplicationName = ApplicationName,

        });

        var r = service.Spreadsheets.Values.Get(SheetID, readIndex);
        r.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.FORMATTEDVALUE;
        try {

            var Response = r.Execute();

        } catch (Google.GoogleApiException e) {

            Console.WriteLine("Error updating values: {0}", e.Message);
            Console.WriteLine("Error status code: {0}", e.Error.Code);
            Console.WriteLine("Error message: {0}", e.Error.Message);
            return null;

        }
        var response = r.Execute();

        var values = response.Values;

        if (values != null && values.Count > 0) {

            return values[0][0].ToString();

        } else {

            return null;

        }

    }


    private static SheetsService InitializeSheetsService()
    {

            // Replace the following variables with your own values.
            string appName = ApplicationName;

            // Load the client secrets file from the specified path.
            GoogleCredential credential;
            using (var stream = new System.IO.FileStream(credentials, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(SheetsService.Scope.Spreadsheets);
            }

            // Initialize the Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = appName
            });

            return service;
    }
}


