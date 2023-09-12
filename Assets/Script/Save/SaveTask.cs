
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

    public class SaveTask : BaseTask
    {
        public object Data { get; set; }
        public Action OnFinish;

        public override async Task Do()
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, GameManager.Save.directory, Path);
            string directory = System.IO.Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    string serialize = JsonConvert.SerializeObject(Data);
                    await sw.WriteAsync(serialize);
                }
            }
            OnFinish?.Invoke();
        }
    }