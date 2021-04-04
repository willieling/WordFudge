using PlayFab;
using PlayFab.ServerModels;
using System;
using System.Collections.Generic;
using System.IO;
using TinyJSON;
using UnityEditor;
using UnityEngine;
using WordFudge.CloudService;

namespace WordFudgeEditor.Database
{
    public class WordListUploaderWindow : EditorWindow
    {
        private string wordListPath = @"C:\Users\willi\source\WordFudge\Assets\Resources\Database\Words.txt";
        private float parsingProgress = 0;

        private bool isUploading = false;

        private readonly WordListUploader uploader = new WordListUploader();

        [MenuItem("Word Fudge/Word List Uploader")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            WordListUploaderWindow window = (WordListUploaderWindow)EditorWindow.GetWindow(typeof(WordListUploaderWindow));
            window.Show();
        }

        private void OnEnable()
        {
            uploader.FinishedUpload += OnUploadFinished;
        }

        private void OnDestroy()
        {
            uploader.FinishedUpload -= OnUploadFinished;
        }

        private void OnGUI()
        {
            const int PROGRESS_BAR_HEIGHT = 20;
            const int UPLOAD_BUTTON_HEIGHT = 50;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Source: ", GUILayout.Width(UPLOAD_BUTTON_HEIGHT));
                wordListPath = GUILayout.TextArea(wordListPath);
            }
            GUILayout.EndHorizontal();

            if(GUILayout.Button("Upload word list to PlayFab", GUILayout.Height(UPLOAD_BUTTON_HEIGHT)) && !isUploading)
            {
                isUploading = true;
                List<WordSubset> wordLists = GenerateWordLists();
                uploader.Upload(wordLists);
            }

            Rect lastRect = new Rect();
            switch (Event.current.type)
            {
                case EventType.Repaint:
                    lastRect = GUILayoutUtility.GetLastRect();
                    lastRect.y += UPLOAD_BUTTON_HEIGHT;
                    lastRect.height = PROGRESS_BAR_HEIGHT;
                    break;
            }

            GUILayout.BeginHorizontal();
            {
                MoveRectDown(ref lastRect);
                EditorGUI.ProgressBar(lastRect, parsingProgress, "Parsing Words");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                MoveRectDown(ref lastRect);
                EditorGUI.ProgressBar(lastRect, uploader.Progress, "Uploading Title Data");
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(uploader.UploadMessage, uploader.MessageType);
        }

        private void MoveRectDown(ref Rect lastRect)
        {
            lastRect.y += lastRect.height;
        }

        private List<WordSubset> GenerateWordLists()
        {
            IEnumerable<string> wordsFromFile = File.ReadLines(@"C:\Users\willi\source\WordFudge\Assets\Resources\Database\Words.txt");
            HashSet<string> uniqueWords = new HashSet<string>(wordsFromFile);

            List<WordSubset> wordSubsets= new List<WordSubset>();
            WordSubset constructingSubset = new WordSubset();
            wordSubsets.Add(constructingSubset);

            int wordsParsed = 0;
            foreach(string word in uniqueWords)
            {
                if(!constructingSubset.TryAdd(word))
                {
                    constructingSubset = new WordSubset(word);
                    wordSubsets.Add(constructingSubset);
                }

                ++wordsParsed;
                parsingProgress = wordsParsed / uniqueWords.Count;
            }

            return wordSubsets;
        }

        private void OnUploadFinished()
        {
            isUploading = false;
        }

        private class WordSubset
        {
            //https://developer.playfab.com/en-US/13428/limits/CustomData/TitleDataIndividualValueSize
            private const int PLAYFAB_TITLE_DATA_SIZE_LIMIT = 1000000;

            //amount of characters the json encoding adds
            private const int JSON_START_END_SIZE = 4;  // [" AND "]
            private const int JSON_IN_BETWEEN_SIZE = 3; // ","

            List<string> words = new List<string>(PLAYFAB_TITLE_DATA_SIZE_LIMIT);

            public int JsonSize { get; private set; } = JSON_START_END_SIZE;

            public bool IsValid
            { 
                get 
                {
                    return words.Count > 0 && JsonSize < PLAYFAB_TITLE_DATA_SIZE_LIMIT;
                }
            }

            public WordSubset(string word = null)
            {
                if (word != null)
                {
                    words.Add(word);
                }
            }

            public bool TryAdd(string word)
            {
                int newSize = JsonSize + word.Length + JSON_IN_BETWEEN_SIZE;
                if(newSize >= PLAYFAB_TITLE_DATA_SIZE_LIMIT)
                {
                    return false;
                }

                words.Add(word);

                JsonSize = newSize;

                return true;
            }

            public string GetJson()
            {
                return JSON.Dump(words);
            }
        }

        private class WordListUploader
        {
            public event Action FinishedUpload;

            private List<WordSubset> wordLists = new List<WordSubset>();

            private int uploadingIndex = 0;
            private int uploadSuccesses = 0;
            private int uploadFailures = 0;

            public float Progress { get; private set; }

            public string UploadMessage { get; private set; } = string.Empty;
            public MessageType MessageType { get; private set; } = MessageType.None;


            private bool IsFinishedUpload { get { return uploadSuccesses + uploadFailures == wordLists.Count; } }

            public void Upload(List<WordSubset> wordLists)
            {
                uploadingIndex = 0;
                uploadSuccesses = 0;
                uploadFailures = 0;
                Progress = 0;

                if (wordLists.Count == 0)
                {
                    Progress = 1;

                    UploadMessage = $"All word subsets were invalid.  Nothing was uploaded.";
                    MessageType = MessageType.Error;
                    return;
                }
                else
                {
                    UploadMessage = string.Empty;
                    MessageType = MessageType.None;
                }

                this.wordLists = new List<WordSubset>(wordLists);

                UploadNextSubset();
            }

            private void UploadNextSubset()
            {
                PlayFabRequests.SetTitleData(uploadingIndex, wordLists[uploadingIndex].GetJson(), OnUploadedSuccess, OnUploadedFailure);
            }

            private void OnUploadedSuccess(SetTitleDataResult obj)
            {
                ++uploadSuccesses;

                Progress = (float)(uploadSuccesses + uploadFailures) / wordLists.Count;
                if (!IsFinishedUpload)
                {
                    IncrementAndStartNextUpload();
                }
                else
                {
                    UpdateMessageAndType();
                    FinishedUpload.Invoke();
                }
            }

            private void OnUploadedFailure(PlayFabError obj)
            {
                ++uploadFailures;

                Progress = (float)(uploadSuccesses + uploadFailures) / wordLists.Count;
                if (!IsFinishedUpload)
                {
                    IncrementAndStartNextUpload();
                }
                else
                {
                    UpdateMessageAndType();
                    FinishedUpload.Invoke();
                }
            }

            private void IncrementAndStartNextUpload()
            {
                ++uploadingIndex;
                UploadNextSubset();
            }

            private void UpdateMessageAndType()
            {
                if (uploadFailures > 0)
                {
                    UploadMessage = $"There were {uploadFailures} invalid word subsets.  They were skipped from being uploaded.";
                    MessageType = MessageType.Warning;
                }
                else
                {
                    UploadMessage = $"{uploadSuccesses} were uploaded successfully!";
                    MessageType = MessageType.Info;
                }
            }
        }
    }
}
