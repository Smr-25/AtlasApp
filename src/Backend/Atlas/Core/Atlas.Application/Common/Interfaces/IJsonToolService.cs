namespace Atlas.Application.Common.Interfaces;

public interface IJsonToolService
{
    string FormatJson(string json);

    string MinifyJson(string json);

    string DiffJson(string json1, string json2);
}