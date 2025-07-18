using System.Text.Json;
using OneWare.Essentials.Helpers;

namespace FEntwumS.NetlistViewer.Services;

public class StorageService : IStorageService
{
    private static Dictionary<string, object> _storage = new Dictionary<string, object>();

    public async Task Save(string path)
    {
        try
        {
            var saveD = _storage.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            await using FileStream stream = new FileStream(path, FileMode.Create);
            await JsonSerializer.SerializeAsync(stream, saveD, saveD.GetType(), new JsonSerializerOptions(){WriteIndented = true, AllowTrailingCommas = true});
        }
        catch (Exception e)
        {
            ServiceManager.GetCustomLogger().Error(e.Message);
        }
    }

    public async Task Load(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                ServiceManager.GetCustomLogger().Error("Storage file not found: " + path);
                return;
            }

            await using FileStream stream = new FileStream(path, FileMode.Open);
            _storage = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(stream) ??
                       new Dictionary<string, object>();
        }
        catch (Exception e)
        {
            ServiceManager.GetCustomLogger().Error(e.Message);
        }
    }

    public void RegisterKeyValuePair(string key, object value)
    {
        _storage.Add(key, value);
    }

    public void RemoveKeyValuePair(string key)
    {
        _storage.Remove(key);
    }

    public void SetKeyValuePairValue(string key, object value)
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

    public IObservable<object> GetKeyValuePairObservable(string key)
    {
        _storage.TryGetValue(key, out object? value);
        return value as IObservable<object> ?? throw new InvalidOperationException($"Key {key} is not registered");
    }

    public bool KeyExists(string key)
    {
        return _storage.ContainsKey(key);
    }
}