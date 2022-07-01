using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NameGenerator {
    private List<string> names;

    public NameGenerator() {
        names = new List<string>();
        
        StreamReader streamReader = new StreamReader("./Assets/names.txt");

        string line = "";
        while((line = streamReader.ReadLine()) != null) {
            names.Add(line);
        }

        streamReader.Close();
    }

    public string GetRandomName() {
        int rnd = Random.Range(0, names.Count);
        string name = names[rnd];
        
        names.RemoveAt(rnd);
        return name;
    }
}
