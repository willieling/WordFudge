using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using WordFudge.DataBase;

namespace WordFudge.CloudService
{
    /// <summary>
    /// Logs in to PlayFab and gets updates the title data if needed
    /// </summary>
    public class DatabaseLoader
    {
        private const string DB_VERSION = "DatabaseVersion";

        private DatabaseVersions localVersion;

        public void LoadDatabase()
        {

            //need to make savefile system.
            TextAsset wordAsset = Resources.Load<TextAsset>("Database/Words");
            Assert.IsNotNull(wordAsset);

            HashSet<string> words = new HashSet<string>(wordAsset.text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));

            PlayFabRequests.GetTitleData(new List<string>() { DB_VERSION }, OnGetTitleDataVersionSuccess, OnGetTitleDataVersionFailure);

        }

        private void OnGetTitleDataVersionSuccess(GetTitleDataResult result)
        {
            localVersion = JsonUtility.FromJson<DatabaseVersions>(result.Data[DB_VERSION]);
        }

        private void OnGetTitleDataVersionFailure(PlayFabError error)
        {
            throw new NotImplementedException();
        }
    }
}
