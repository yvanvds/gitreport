using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitReport
{
    class GitUser
    {
        User user;
        public string Login { get => user.Login; }
        public string Name { get => user.Name !=  null ? user.Name : Login; }

        public List<Commit> Commits { get; set; } = new List<Commit>();

        public int CommitScore { get; set; }
        public int ChangesScore { get; set; }

        public float TotalScore { get; set; }

        public GitUser(User user)
        {
            this.user = user;
        }

        public void PrintDetails()
        {
            Console.WriteLine();
            Console.WriteLine("=== User Info ===");
            Console.WriteLine("Name               : " + user.Name);
            Console.WriteLine("Public repositories: " + user.PublicRepos);
        }


        public void Evaluate()
        {
            Console.WriteLine();
            Console.WriteLine("#########");
            Console.WriteLine("# " + Name);
            Console.WriteLine("#########");

            int commits = CommitDays();
            if (commits >= 7) CommitScore = 5;
            else if (commits >= 5) CommitScore = 4;
            else if (commits >= 3) CommitScore = 3;
            else if (commits >= 2) CommitScore = 2;
            else if (commits >= 1) CommitScore = 1;
            else CommitScore = 0;

            int changes = Changes();
            if (changes >= 500) ChangesScore = 5;
            else if (changes >= 300) ChangesScore = 4;
            else if (changes >= 100) ChangesScore = 3;
            else if (changes >= 50) ChangesScore = 2;
            else if (changes >= 20) ChangesScore = 1;
            else ChangesScore = 0;

            TotalScore = (CommitScore + ChangesScore) / 2.0f;

            Console.WriteLine();
            Console.WriteLine("Resultaat voor {0}", Name);
            Console.WriteLine("{0} commits voor een score van {1}", commits, CommitScore);
            Console.WriteLine("{0} wijzigingen voor een score van {1}", changes, ChangesScore);
            Console.WriteLine("Resultaat: {0}/5", TotalScore);
        }

        

        public int CommitDays()
        {
            List<DateTime> dates = new List<DateTime>();

            foreach(var commit in Commits)
            {
                //if (commit.Additions > 100)
                //{
                //    // probably boilerplate code
                //    continue;
                //}
                if (!dates.Contains(commit.Date.Date))
                {
                    dates.Add(commit.Date.Date);
                }
            }
            return dates.Count;
        }

        public int Changes()
        {
            int result = 0;
            foreach(var commit in Commits)
            {
                //if (commit.Additions < 100)
                foreach(var file in commit.Git.Files)
                {
                    if (File.Acceptable(file))
                    {
                        result += file.Additions;
                        Console.WriteLine("  => Adding file {0}", file.Filename);
                    } else
                    {
                        
                    }
                }
            }
            return result;
        }
    }
}
