
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class LoadTask : BaseTask
{
    public Type ObjectType { get; set; }
    public Action<object> OnFinish;

    public override async Task Do()
    {
        object data = null;
        string path = System.IO.Path.Combine(Application.persistentDataPath, GameManager.Save.directory, Path);
        if (!File.Exists(path))
        {
            // UnityLog.Error($"Load fail, file not found:{path}");
            OnFinish?.Invoke(null);
            return;
        }

        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string json = await sr.ReadToEndAsync();
                    data = JsonConvert.DeserializeObject(json, ObjectType);
                }
            }
        }
        finally
        {
            OnFinish?.Invoke(data);
        }
    }
}
