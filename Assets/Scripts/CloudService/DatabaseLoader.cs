using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Text;
using TinyJSON;
using UnityEngine;
using UnityEngine.Assertions;
using WordFudge.DataBase;
using WordFudge.Save;

namespace WordFudge.CloudService
{
    /// <summary>
    /// Logs in to PlayFab and gets updates the title data if needed
    /// </summary>
    public class DatabaseLoader
    {
        public event Action<bool> FinishedLoadingDatabase;

        private const string DATABASE_PATH_PREFIX = "Database";
        private const string DB_VERSION_KEY = "DatabaseVersion";
        private const string WORDS_KEY = "Words";

        //private DatabaseVersions localVersion;

        public void LoadDatabase()
        {
            Debug.Log("Checking PlayFab for versioning.");
            PlayFabRequests.GetTitleData(new List<string>() { DB_VERSION_KEY, "asf" }, OnGetTitleDataVersionSuccess, OnGetTitleDataVersionFailure);
        }

        private void OnGetTitleDataVersionSuccess(GetTitleDataResult result)
        {
            Debug.Log("Version data fetched.");

            string versionPath = GetDatabaseFilePath(DB_VERSION_KEY);
            string savedVersionsJson = SaveSystem.Load(versionPath);
            if (string.IsNullOrEmpty(savedVersionsJson))
            {
                Debug.Log("No local version data found.  Copying packaged version data to save folder.");

                TextAsset wordAsset = Resources.Load<TextAsset>(versionPath);
                Assert.IsNotNull(wordAsset);
                savedVersionsJson = wordAsset.text;
                SaveSystem.Save(versionPath, savedVersionsJson);
            }
            DatabaseVersions localVersion = JSON.Load(savedVersionsJson).Make<DatabaseVersions>();

            DatabaseVersions cloudVersion = JSON.Load(result.Data[DB_VERSION_KEY]).Make<DatabaseVersions>();

            List<string> staleData = new List<string>(cloudVersion.versions.Count);
            foreach(KeyValuePair<string, int> kvp in cloudVersion.versions)
            {
                if(kvp.Value > localVersion.versions[kvp.Key])
                {
                    staleData.Add(kvp.Key);
                }
            }

            // normally we'd look at each version and compare them but we're doing a simple game
            // also we need to get multiple datas and we don't know the exact number
            // so just pass in null to get all title data
            if (staleData.Count > 0)
            {
                Debug.Log("Data is stale.  Fetching updated data from PlayFab.");

                PlayFabRequests.GetTitleData(null, OnGetWordsSuccess, OnGetWordsFailure);
            }
            else
            {
                LoadSavedWordList();
            }
        }

        private void OnGetTitleDataVersionFailure(PlayFabError error)
        {
            Debug.LogError($"Could not get versioning data.\n{error.GenerateErrorReport()}");
            FinishedLoadingDatabase.Invoke(false);
        }

        private void OnGetWordsSuccess(GetTitleDataResult result)
        {
            SaveSystem.Save(GetDatabaseFilePath(DB_VERSION_KEY), result.Data[DB_VERSION_KEY]);

            List<string> wordListJsons = new List<string>(result.Data.Count);
            foreach(KeyValuePair<string, string> kvp in result.Data)
            {
                if(kvp.Key.StartsWith(WORDS_KEY, StringComparison.OrdinalIgnoreCase))
                {
                    wordListJsons.Add(kvp.Value);
                }
            }

            Assert.IsTrue(wordListJsons.Count > 0, "Tried to update words lists but didn't get anything back.");
            Debug.Log($"Got an update for the word list.  It's split into {wordListJsons.Count} parts.");

            HashSet<string> uniqueWords = new HashSet<string>();
            foreach(string json in wordListJsons)
            {
                List<string> words = JSON.Load(json).Make<List<string>>();
                foreach(string word in words)
                {
                    uniqueWords.Add(word);
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach(string word in uniqueWords)
            {
                sb.AppendLine(word);
            }

            SaveSystem.Save(GetDatabaseFilePath(WORDS_KEY), sb.ToString());

            LoadSavedWordList();
        }

        private void OnGetWordsFailure(PlayFabError error)
        {
            Debug.LogError($"Could not get title data.\n{error.GenerateErrorReport()}");
            FinishedLoadingDatabase.Invoke(false);
        }

        private string GetDatabaseFilePath(string filename)
        {
            return $"{DATABASE_PATH_PREFIX}/{filename}";
        }

        private void LoadSavedWordList()
        {
            string[] wordList = SaveSystem.LoadMultiLine(GetDatabaseFilePath(WORDS_KEY));
            if(wordList.Length == 0)
            {
                Debug.Log("Could not find saved word list.  Copying packaged words list into save folder.");
                TextAsset wordsAsset = Resources.Load<TextAsset>(GetDatabaseFilePath(WORDS_KEY));
                Assert.IsNotNull(wordsAsset);

                wordList = wordsAsset.text.Split(new string[]{ "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                SaveSystem.Save(GetDatabaseFilePath(WORDS_KEY), wordsAsset.text);
            }

            Database.InitializeWordList(wordList);
            Debug.Log("Word list initialized!");

            FinishedLoadingDatabase.Invoke(true);
        }
    }
}
