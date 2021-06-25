using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TranslateWord
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            long initialMemory = GC.GetTotalMemory(false);
            Console.WriteLine("Before allocations: {0:N0}", initialMemory);

            var shakespeareFile = @"D:\TranslateWordsChallenge\t8.shakespeare.txt";
            var shakespeareText = File.ReadAllText(shakespeareFile);
            var findWordsFile = @"D:\TranslateWordsChallenge\find_words.txt";
            var findWords = File.ReadAllLines(findWordsFile);

            var replaceWords = ConvertCSVToList();
            var wordsCount = new List<string>();
            foreach (var englishWord in findWords)
            {
                int wordCount = Regex.Matches(shakespeareText, "\\b" + Regex.Escape(englishWord) + "\\b", RegexOptions.IgnoreCase).Count;
                wordsCount.Add(englishWord + "," + wordCount);
                var frenchWord = replaceWords.Where(x => x.EnglishWord == englishWord).FirstOrDefault().FrenchWord;
                var englishwordUpperCase = char.ToUpper(englishWord[0]) + englishWord.Substring(1);
                shakespeareText = shakespeareText.Replace(englishWord, frenchWord);
                shakespeareText = shakespeareText.Replace(englishwordUpperCase, frenchWord);
            }
            string csv = String.Join("\n", wordsCount);
            File.WriteAllText(@"D:\TranslateWordsChallenge\frequency.csv", csv);
            File.WriteAllText(@"D:\TranslateWordsChallenge\t8.shakespeare.translated.txt", shakespeareText);

            long finalMemory = GC.GetTotalMemory(false);
            Console.WriteLine("After allocations: {0:N0}", finalMemory);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string result = "Time to process: " + ts.Minutes + " minutes " + ts.Seconds + " seconds" + "\n" + "Memory Used :" + ((finalMemory - initialMemory)/1024).ToString() + "MB";
            File.WriteAllText(@"D:\TranslateWordsChallenge\performance.txt", result);
            Console.Read();
        }

        private static List<Words> ConvertCSVToList()
        {
            List<Words> words = new List<Words>();
            var lines = System.IO.File.ReadAllLines(@"D:\TranslateWordsChallenge\french_dictionary.csv");

            foreach (var item in lines)
            {
                var values = item.Split(',');
                words.Add(new Words()
                {
                    EnglishWord = values[0],
                    FrenchWord = values[1]
                });
            }

            return words;
        }
    }

    public class Words
    {
        public string EnglishWord { get; set; }
        public string FrenchWord { get; set; }
    }
}