using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFrequencies : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Dialog/Create Text Frequencies")]
    public static void NewTextFrequencies()
    {
        TextFrequencies tFreq = CreateInstance<TextFrequencies>();
        List<char> chars = new List<char>() {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm','n','o','p','q','r','s','t','u','v','w','x','y','z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            '0','1','2','3','4','5','6','7','8','9',' ', ',', '.', '-'};
        tFreq.frequencies = new List<FreqPair>();
        foreach (char c in chars)
        {
            tFreq.frequencies.Add(new FreqPair() { c = c, frequency = Random.Range(0, 100) });
        }
        UnityEditor.AssetDatabase.CreateAsset(tFreq, "Assets/DialogSystem/Resources/textFreq.asset");
    }
#endif
    public static TextFrequencies Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<TextFrequencies>("textFreq");
            return instance;
        }
    }
    private static TextFrequencies instance;
    [System.Serializable]
    public class FreqPair
    {
        public char c;
        public float frequency;
    }
    public List<FreqPair> frequencies;
    private Dictionary<char, float> freqLookup;
    public float GetFreq(char character)
    {
        if (freqLookup == null)
        {
            Init();
        }
        if (freqLookup.ContainsKey(character))
            return freqLookup[character];
        else return 0;
    }
    public void Init()
    {
        freqLookup = new Dictionary<char, float>();
        for (int i = 0; i < frequencies.Count; i++)
        {
            freqLookup.Add(frequencies[i].c, frequencies[i].frequency);
        }
    }
}
