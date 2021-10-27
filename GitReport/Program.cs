using Octokit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitReport
{
    class Program
    {
        static List<GitUser> users = new List<GitUser>();
        static List<Repository> repos = new List<Repository>();

        public static async Task<int> Main(string[] args)
        {
            DateTime start = DateTime.Parse("15/10/2021");

            if (!Connector.getInstance().Init())
            {
                return 0;
            }



            // create list of users
            users.Add(await Connector.getInstance().getUser("ferre0160"));
            users.Add(await Connector.getInstance().getUser("ThijmThijm22"));
            users.Add(await Connector.getInstance().getUser("HarmanSingh9630"));
            users.Add(await Connector.getInstance().getUser("SeppeVanbedts"));
            users.Add(await Connector.getInstance().getUser("VanWoenselBram"));
            users.Add(await Connector.getInstance().getUser("chiswex"));
            users.Add(await Connector.getInstance().getUser("massej6it"));
            users.Add(await Connector.getInstance().getUser("sonnew"));
            users.Add(await Connector.getInstance().getUser("StanVDL"));
            //users.Add(await Connector.getInstance().getUser(""));
            //users.Add(await Connector.getInstance().getUser(""));
            //users.Add(await Connector.getInstance().getUser(""));
            //users.Add(await Connector.getInstance().getUser(""));
            //users.Add(await Connector.getInstance().getUser(""));
            //users.Add(await Connector.getInstance().getUser(""));
            //users.Add(await Connector.getInstance().getUser(""));
            //users.Add(await Connector.getInstance().getUser(""));

            // get all repositories, from all users5
            foreach (var user in users)
            {
                var result = await Connector.getInstance().getRepositories(user);
                foreach(var r in result)
                {
                    repos.Add(r);
                }
            }

            {
                // get commits from repositories
                foreach (var repo in repos)
                {
                    try
                    {
                        Console.WriteLine("Looking at Repository {0}", repo.Name);
                        var commits = await Connector.getInstance().getCommits(repo);
                        foreach (var commit in commits)
                        {
                            // only add commits after start date
                            if (commit.Commit.Author.Date >= start)
                            {
                                Console.WriteLine("Adding commit for evaluation: {0}", commit.Commit.Message);
                                await AddCommit(commit.Author.Login, commit.Commit.Author.Date.DateTime, commit.Sha, repo);
                            }
                        }
                    } catch(Octokit.ApiException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            // calculate results
            foreach(var user in users)
            {
                user.Evaluate();
            }

            Connector.getInstance().printLimits();

            return 0;
        }

        private static async Task AddCommit(string userName, DateTime date, string sha, Repository repo)
        {
            foreach(var user in users)
            {
                if (user.Login.Equals(userName))
                {
                    var result = await Connector.getInstance().loadCommit(repo, sha);
                    user.Commits.Add(new Commit(date, result));
                    return;
                }
            }
        }
    }
}
