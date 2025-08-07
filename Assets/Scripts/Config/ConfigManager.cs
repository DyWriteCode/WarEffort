using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager
{
    private Dictionary<string, ConfigData> loadList;

    private Dictionary<string, ConfigData> configs;

    public ConfigManager()
    {
        loadList = new Dictionary<string, ConfigData>();
        configs = new Dictionary<string, ConfigData>();
    }

    public void Register(string file,ConfigData config)
    {
        loadList[file] = config;
    }

    public void LoadAllConfigs()
    {
        foreach(var item in loadList)
        {
            TextAsset textAsset = item.Value.LoadFile();
            item.Value.Load(textAsset.text);
            configs.Add(item.Value.fileName, item.Value);
        }
        loadList.Clear();
    }

    public ConfigData GetConfigData(string file)
    {
        if (configs.ContainsKey(file))
        {
            return configs[file];
        }
        else
        {
            return null;
        }
    }
}
