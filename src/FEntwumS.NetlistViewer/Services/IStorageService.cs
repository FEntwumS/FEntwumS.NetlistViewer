namespace FEntwumS.NetlistViewer.Services;

public interface IStorageService
{
	public Task SaveAsync();
	public Task LoadAsync();
	public void RegisterKeyValuePair(string key, string value);
	public void RemoveKeyValuePair(string key);
	public void SetKeyValuePairValue(string key, string value);
	public string? GetKeyValuePairValue(string key);
	public bool KeyExists(string key);
}