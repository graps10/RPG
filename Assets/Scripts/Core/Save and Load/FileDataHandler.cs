using System;
using System.IO;
using UnityEngine;

namespace Core.Save_and_Load
{
    public class FileDataHandler
    {
        private const string Code_Word = "codeWord";
        
        private string _dataDirPath;
        private string _dataFileName;

        private bool _encryptData;
        
        public FileDataHandler(string dataDirPath, string dataFileName, bool encryptData)
        {
            _dataDirPath = dataDirPath;
            _dataFileName = dataFileName;
            _encryptData = encryptData;
        }

        public void Save(GameData data)
        {
            string fullPath = Path.Combine(_dataDirPath, _dataFileName);

            try{
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? throw new InvalidOperationException());

                string dataToStore = JsonUtility.ToJson(data, true);

                if(_encryptData)
                    dataToStore = EncryptDecrypt(dataToStore);

                using(FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using(StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToStore);
                    }
                }
            }
            catch(Exception e){
                Debug.LogError("Error on trying to save data to file: " + fullPath + "\n" + e);
            }
        }

        public GameData Load()
        {
            string fullPath = Path.Combine(_dataDirPath, _dataFileName);

            GameData loadData = null;

            if(File.Exists(fullPath))
            {
                try{
                    string dataToLoad;

                    using(FileStream stream = new FileStream(fullPath, FileMode.Open))
                    {
                        using(StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    if(_encryptData)
                        dataToLoad = EncryptDecrypt(dataToLoad);
                    
                    loadData = JsonUtility.FromJson<GameData>(dataToLoad);
                }
                catch(Exception e){
                    Debug.LogError("Error on trying to load data from file: " + fullPath + "\n" + e);
                }
            }

            return loadData;
        }

        public void Delete()
        {
            string fullPath = Path.Combine(_dataDirPath, _dataFileName);

            if(File.Exists(fullPath))
                File.Delete(fullPath);
        }

        private string EncryptDecrypt(string _data)
        {
            string modifiedData = string.Empty;

            for (int i = 0; i < _data.Length; i++)
            {
                modifiedData += (char)(_data[i] ^ Code_Word[i % Code_Word.Length]);
            }

            return modifiedData;
        }
    }
}
