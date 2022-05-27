using System;
using UnityEngine;

[Serializable]
public class SerializableWorldStructure {

    // Variables are public because otherwise the FromJson method wouldn't set the values
    public  int width;
    public  int height;

    public  int[] world;
    public float[] spawn_team1;
    public float[] spawn_team2;
    public float[] spawn_food;

    public static SerializableWorldStructure CreateFromJson(string jsonString) {
        return JsonUtility.FromJson<SerializableWorldStructure>(jsonString);
    }

    public int[] GetWorld() {
        return world;
    }
}
