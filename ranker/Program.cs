using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DictionaryRanker
{
    class Program
    {
        const int MaxRuns = 1000;

        public static int Iterations = MaxRuns;

        public const int MaxLoaded = 100000;

        public static List<SubmissionInfo> Submissions = new List<SubmissionInfo>();
        public static WordList WordList = new WordList();
        public static List<string> WordsToUse = new List<string>();

        static void Main()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            PrepareSubmitters();
            PrepareWordLists();

            while (true)
            {
                Console.WriteLine("Ready to start. Hit <ENTER> or write the number of iterations to run followed by <ENTER>...");
                var s = Console.ReadLine();

                try
                {
                    Iterations = Convert.ToInt32(s);
                }
                catch (Exception)
                {
                    // Don't change iterations.
                }

                foreach (var submission in Submissions)
                {
                    RankImplementation(submission, Submissions[0].Ranking);
                    if (Console.KeyAvailable)
                        break;
                }

                if (!Console.KeyAvailable)
                {
                    foreach (var submission in Submissions)
                    {
                        Console.WriteLine("=== {0} === : {1}", submission.TeamName, Score(Submissions[0].Ranking, submission.Ranking));
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Done. Press <ENTER> to finish or enter 'more' to execute ranking again.");
                s = Console.ReadLine();
                if (string.IsNullOrEmpty(s) || s.ToLower() != "more")
                    break;
            }
        }

        static decimal Score(Ranking baseRank, Ranking newRank)
        {
            if (newRank.HasExceptions)
                return 0;

            if (newRank.FalsePositives > 0.01m)
                return 0;

            var score = (0.01m - newRank.FalsePositives) * 1000 + ((decimal)baseRank.MillisecondsTime / (decimal)newRank.MillisecondsTime) * 10;
            Console.WriteLine("{0}/{1}/{2}", newRank.FalsePositives, newRank.MillisecondsTime, score);
            return score;
        }

        static void RankImplementation(SubmissionInfo submission, Ranking baseRanking)
        {
            var lastWord = string.Empty;

            var checker = submission.DictionaryChecker;

            try
            {
                Console.WriteLine();
                Console.WriteLine("Initializing");

                var bitStorage = new BitStorage();

                foreach (var s in WordsToUse)
                {
                    lastWord = s;
                    checker.Initialize(s, bitStorage);
                }

                var found = 0;
                var notfound = 0;
                foreach (var s in WordsToUse)
                {
                    lastWord = s;
                    if (checker.IsWordPresent(s, bitStorage))
                    {
                        found++;
                    }
                    else
                    {
                        notfound++;
                    }
                }

                Console.WriteLine("Fixed check: {0} found, {1} not found", found, notfound);

                var originalFound = found;
                found = 0;
                notfound = 0;
                var restList = WordList.GetList();
                restList.AddRange(WordsToUse);

                GC.Collect();
                GC.Collect();
                GC.SuppressFinalize(checker);

                var sw = new Stopwatch();
                sw.Start();

                var maxLoop = submission.IsSlow ? Math.Min(Iterations, 1) : Iterations;

                for (var count = 1; count <= maxLoop; count++)
                {
                    foreach (var s in restList)
                    {
                        lastWord = s;
                        if (checker.IsWordPresent(s, bitStorage))
                        {
                            if (count == 1)
                                found++;
                        }
                        else
                        {
                            if (count == 1)
                                notfound++;
                        }
                    }
                }
                sw.Stop();

                var elapsed = sw.ElapsedMilliseconds;
                if (submission.IsSlow)
                {
                    elapsed = elapsed * MaxRuns / maxLoop;
                }

                Console.WriteLine("Complete check: {0} found, {1} not found", found, notfound);
                Console.WriteLine("False positives: {0}%", (decimal)(found - originalFound) * 100 / ((found - originalFound) + notfound));

                submission.Ranking = new Ranking
                {
                    HasExceptions = false,
                    MillisecondsTime = elapsed,
                    FoundWords = found,
                    NotFoundWords = notfound,
                    FalsePositives = (decimal)(found - originalFound) * 100 / ((found - originalFound) + notfound),
                    OriginalFoundWords = originalFound
                };
            }
            catch (Exception ex)
            {
                submission.Ranking = new Ranking
                {
                    ExceptionMessage = ex.ToString(),
                    HasExceptions = true,
                    LastWordUsed = lastWord
                };
            }

            if (baseRanking != null)
            {
                Score(baseRanking, submission.Ranking);
            }
        }

        private static void PrepareSubmitters()
        {
            Submissions.Add(new SubmissionInfo
            {
                DictionaryChecker = new Submissions.Base.Dictionary(),
                TeamName = "base"
            });

            Submissions.Add(new SubmissionInfo
            {
                DictionaryChecker = new Submissions.camanatidis.CrazyBloomFilterX(),
                TeamName = "c.amanatidis"
            });

            Submissions.Add(new SubmissionInfo
            {
                DictionaryChecker = new Submissions.igavriilidis.BloomDictionary(),
                TeamName = "i.gavriilidis"
            });

            Submissions.Add(new SubmissionInfo
            {
                DictionaryChecker = new Submissions.ikaradimas.BloomDictionary(),
                TeamName = "i.karadimas"
            });

            Submissions.Add(new SubmissionInfo
            {
                DictionaryChecker = new Submissions.lpappas.DictionaryChecker(),
                TeamName = "l.pappas"
            });

            Submissions.Add(new SubmissionInfo
            {
                DictionaryChecker = new Submissions.ptrapatsas.BloomFilterDictionary(),
                TeamName = "p.trapatsas"
            });

            Submissions.Add(new SubmissionInfo
            {
                DictionaryChecker = new Submissions.kkentzoglanakis.DummyDictionary(),
                TeamName = "k.kentzoglanakis"
            });

            Submissions.Add(new SubmissionInfo
            {
                DictionaryChecker = new Submissions.ggkogkolis.DictionaryChecker(),
                TeamName = "g.gkogkolis"
            });

            Submissions.Add(new SubmissionInfo
            {
                DictionaryChecker = new Submissions.asavvas.OneTimeBloomFilterDictionaryChecker(),
                TeamName = "a.savvas"
            });
        }

        private static void PrepareWordLists()
        {
            Console.Write("Loading list and indexing...");

            WordList.LoadWordList();

            Console.WriteLine("\rLoaded word list: {0} words", WordList.NumberOfWordsLoaded());

            for (var i = 1; i <= MaxLoaded; i++)
            {
                var randomWord = WordList.GetRandomWord();
                WordsToUse.Add(randomWord);
                WordList.Remove(randomWord);

                if (i % 1000 == 0)
                {
                    var x = Console.CursorLeft;
                    var y = Console.CursorTop;
                    Console.Write(i);
                    Console.SetCursorPosition(x, y);
                }
            }
        }
    }

    internal class Ranking
    {
        public bool HasExceptions { get; set; }
        public string ExceptionMessage { get; set; }
        public long MillisecondsTime { get; set; }
        public decimal FalsePositives { get; set; }
        public int FoundWords { get; set; }
        public int OriginalFoundWords { get; set; }
        public int NotFoundWords { get; set; }
        public string LastWordUsed { get; set; }
    }

    internal class SubmissionInfo
    {
        public IDictionaryChecker DictionaryChecker { get; set; }

        public bool IsSlow { get; set; }

        public string TeamName { get; set; }

        public Ranking Ranking { get; set; }
    }
}
