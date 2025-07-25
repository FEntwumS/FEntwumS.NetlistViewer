﻿using System.Text.Json;
using FEntwumS.NetlistViewer.Helpers;
using DynamicData.Binding;

namespace FEntwumS.NetlistViewer.Services;

public class StorageService : IStorageService
{
    public StorageService()
    {
        _ = LoadAsync();
    }
    
    private static Dictionary<string, string> _storage = new Dictionary<string, string>();

    public async Task SaveAsync()
    {
        string path = FentwumSNetlistViewerSettingsHelper.DataFilePath;
        
        try
        {
            if (!Directory.Exists(FentwumSNetlistViewerSettingsHelper.DataDirectory))
            {
                Directory.CreateDirectory(FentwumSNetlistViewerSettingsHelper.DataDirectory);
            }
            
            var saveD = _storage.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            await using FileStream stream = new FileStream(path, FileMode.Create);
            await JsonSerializer.SerializeAsync(stream, saveD, saveD.GetType(), new JsonSerializerOptions(){WriteIndented = true, AllowTrailingCommas = true});
        }
        catch (Exception e)
        {
            ServiceManager.GetCustomLogger().Error(e.Message);
        }
    }

    public async Task LoadAsync()
    {
        string path = FentwumSNetlistViewerSettingsHelper.DataFilePath;
        
        try
        {
            if (!File.Exists(path))
            {
                ServiceManager.GetCustomLogger().Log("Storage file not found: " + path);
                return;
            }
            
            _storage.Clear();

            await using FileStream stream = new FileStream(path, FileMode.Open);
            Dictionary<string, JsonElement> json = await JsonSerializer.DeserializeAsync<Dictionary<string, JsonElement>>(stream) ?? new Dictionary<string, JsonElement>();
            foreach ((string key, JsonElement value) in json)
            {
                _storage.Add(key, value.Deserialize<string>());
            }
        }
        catch (Exception e)
        {
            ServiceManager.GetCustomLogger().Error(e.Message);
        }
    }

    public void RegisterKeyValuePair(string key, string value)
    {
        _storage.Add(key, value);
    }

    public void RemoveKeyValuePair(string key)
    {
        _storage.Remove(key);
    }

    public void SetKeyValuePairValue(string key, string value)
    {
        if (!_storage.ContainsKey(key))
        {
            RegisterKeyValuePair(key, value);
        }
        else
        {
            _storage[key] = value;
        }
    }

    public string? GetKeyValuePairValue(string key)
    {
        return _storage.TryGetValue(key, out var value) ? value : null;
    }

    public bool KeyExists(string key)
    {
        return _storage.ContainsKey(key);
    }
}