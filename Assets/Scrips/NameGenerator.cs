using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NameGenerator {
    private string[] names;

    public NameGenerator() {
        List<string> namesList = new List<string>();

        StreamReader streamReader = new StreamReader("./Assets/names.txt");

        string line = "";
        while((line = streamReader.ReadLine()) != null) {
            namesList.Add(line);
        }

        streamReader.Close();

        names = namesList.ToArray();
    }

    public string GetRandomName() {
        int rnd = Random.Range(0, names.Length);
        return names[rnd];
    }
}
