namespace FEntwumS.NetlistViewer.Services;

public interface IStorageService
{
    public Task Save(string path);
    public Task Load(string path);
    public void RegisterKeyValuePair(string key, object value);
    public void RemoveKeyValuePair(string key);
    public void SetKeyValuePairValue(string key, object value);
    public IObservable<object> GetKeyValuePairObservable(string key);
    public bool KeyExists(string key);
}