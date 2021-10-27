using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitReport
{
    class Commit
    {
        public DateTime Date { get;  }
        public GitHubCommit Git { get; }

        public Commit(DateTime date, GitHubCommit commit)
        {
            Date = date;
            Git = commit;
        }

        public int Additions
        {
            get => Git.Stats.Additions;
        }
    }
}
