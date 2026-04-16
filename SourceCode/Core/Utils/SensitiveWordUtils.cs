using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SensitiveWordUtils
{
    class SensitiveWordNode
    {
        public bool IsEnd { get; private set; }
        private Dictionary<char, SensitiveWordNode> childMap = new Dictionary<char, SensitiveWordNode>();

        public SensitiveWordNode AddChild(char c)
        {
            SensitiveWordNode node = null;
            if (!childMap.TryGetValue(c, out node))
            {
                node = new SensitiveWordNode();
                childMap.Add(c, node);
            }

            return node;
        }

        public SensitiveWordNode GetChild(char c)
        {
            if (childMap.TryGetValue(c, out var val))
            {
                return val;
            }

            return null;
        }


        public void SetIsEnd(bool isEnd)
        {
            IsEnd = isEnd;
        }
    }

    private static HashSet<char> interferenceChars = new HashSet<char>()
        { ' ', '.', ',', ';', '!', '?', '~', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '-', '=' };

    private static SensitiveWordNode root;
    private static StringBuilder wordBuffer = new StringBuilder();
    private static StringBuilder strBuffer = new StringBuilder();
    private static HashSet<string> wordSet = new HashSet<string>();

    public static void SetSensitiveWords(IEnumerable<string> words)
    {
        root = new SensitiveWordNode();
        if (null != words)
        {
            foreach (var word in words)
            {
                AddSensitiveWord(word);
            }
        }
    }

    public static void AddSensitiveWord(string word)
    {
        if (null == root || string.IsNullOrEmpty(word))
        {
            return;
        }

        SensitiveWordNode node = root;
        foreach (var item in word)
        {
            node = node.AddChild(item);
        }

        node.SetIsEnd(true);
    }

    public static bool ContainsSensitiveWord(string word, int beginIndex)
    {
        if (null == root || string.IsNullOrEmpty(word))
        {
            return false;
        }

        int length = word.Length;
        for (int i = beginIndex; i < length; i++)
        {
            SensitiveWordNode node = root;
            for (int j = i; j < length; j++)
            {
                if (interferenceChars.Contains(word[j]))
                {
                    continue;
                }

                node = node.GetChild(word[j]);
                if (node == null)
                {
                    break;
                }

                if (node.IsEnd)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool ContainsSensitiveWord(string word)
    {
        return ContainsSensitiveWord(word, 0);
    }

    public static bool TryFindSensitiveWords(string word, int beginIndex, ref HashSet<string> result)
    {
        if (null == root || string.IsNullOrEmpty(word))
        {
            return false;
        }

        int length = word.Length;
        for (int i = beginIndex; i < length; i++)
        {
            wordBuffer.Clear();
            SensitiveWordNode node = root;
            for (int j = i; j < length; j++)
            {
                if (interferenceChars.Contains(word[j]))
                {
                    wordBuffer.Append(word[j]);
                    continue;
                }

                node = node.GetChild(word[j]);
                if (node == null)
                {
                    break;
                }

                wordBuffer.Append(word[j]);
                if (node.IsEnd)
                {
                    if (null == result)
                    {
                        result = new HashSet<string>();
                    }

                    result.Add(wordBuffer.ToString());
                }
            }
        }

        wordBuffer.Clear();
        return null != result && result.Count > 0;
    }

    public static bool TryFindSensitiveWords(string word, ref HashSet<string> result)
    {
        return TryFindSensitiveWords(word, 0, ref result);
    }


    private static string ReplaceWord(string word, char symbol)
    {
        wordBuffer.Clear();
        for (int i = 0; i < word.Length; i++)
        {
            wordBuffer.Append(symbol);
        }

        return wordBuffer.ToString();
    }

    public static string ReplaceSensitiveWords(string word, int beginIndex, char symbol)
    {
        if (TryFindSensitiveWords(word, beginIndex, ref wordSet))
        {
            strBuffer.Append(word);
            foreach (var item in wordSet)
            {
                strBuffer.Replace(item, ReplaceWord(item, symbol));
            }

            string res = strBuffer.ToString();
            strBuffer.Clear();
            return res;
        }
        else
        {
            return word;
        }


        // if (null == root || string.IsNullOrEmpty(word))
        // {
        //     return word;
        // }
        //
        // strBuffer.Clear();
        // for (int i = 0; i < beginIndex; i++)
        // {
        //     strBuffer.Append(word[i]);
        // }
        //
        // bool hasSensitive = false;
        // int length = word.Length;
        // for (int i = beginIndex; i < length; i++)
        // {
        //     wordBuffer.Clear();
        //     SensitiveWordNode node = root;
        //     for (int j = i; j < length; j++)
        //     {
        //         if (interferenceChars.Contains(word[j]))
        //         {
        //             wordBuffer.Append(word[j]);
        //             continue;
        //         }
        //         
        //         node = node.GetChild(word[j]);
        //         if (node == null)
        //         {
        //             break;
        //         }
        //
        //         wordBuffer.Append(word[j]);
        //         if (node.IsEnd)
        //         {
        //             break;
        //         }
        //     }
        //
        //     int wordLen = wordBuffer.Length;
        //     if (wordLen > 0)
        //     {
        //         hasSensitive = true;
        //         for (int j = 0; j < wordLen; j++)
        //         {
        //             strBuffer.Append(symbol);
        //         }
        //     }
        //     else
        //     {
        //         if (strBuffer.Length <= i)
        //         {
        //             strBuffer.Append(word[i]);
        //         }
        //     }
        // }
        //
        // wordBuffer.Clear();
        // if (hasSensitive)
        // {
        //     string res = strBuffer.ToString();
        //     strBuffer.Clear();
        //     return res;
        // }
        //
        // return word;
    }

    public static string ReplaceSensitiveWords(string word, char symbol)
    {
        return ReplaceSensitiveWords(word, 0, symbol);
    }

    public static string ReplaceSensitiveWords(string word)
    {
        return ReplaceSensitiveWords(word, 0, '*');
    }
}