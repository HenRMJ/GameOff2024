using UnityEngine;

public static class NameDatabase
{
    private static NamesSO _nameDatabase;

    private static void LoadDatabase()
    {
        if (_nameDatabase != null) return;

        _nameDatabase = Resources.Load<NamesSO>("NameDatabase");

        if (_nameDatabase == null)
        {
            Debug.LogError("NamesSO not found! Ensure a NameDatabase asset exists in a 'Resources' folder.");
        }
    }

    public static string GetRandonFirstName()
    {
        LoadDatabase();
        return _nameDatabase.FirstNames[Random.Range(0, _nameDatabase.FirstNames.Length)];
    }

    public static string GetRandomLastName()
    {
        LoadDatabase();
        return _nameDatabase.LastNames[Random.Range(0, _nameDatabase.LastNames.Length)];
    }
}
