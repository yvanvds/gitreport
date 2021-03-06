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
            DateTime start = DateTime.Parse("01/12/2021");

            if (!Connector.getInstance().Init())
            {
                return 0;
            }



            // create list of users
            users.Add(await Connector.getInstance().getUser("ShadowFlashy", "Jasper Degreef"));
            users.Add(await Connector.getInstance().getUser("ferre0160", "Ferre Heyde"));
            users.Add(await Connector.getInstance().getUser("ThijmThijm22", "Thijmen Verstraete"));
            users.Add(await Connector.getInstance().getUser("HarmanSingh9630", "Harmanjot Singh"));
            users.Add(await Connector.getInstance().getUser("SeppeVanbedts", "Seppe Vanbedts"));
            users.Add(await Connector.getInstance().getUser("VanWoenselBram", "Bram Van Woensel"));
            users.Add(await Connector.getInstance().getUser("chiswex", "Chisse Weckx"));
            users.Add(await Connector.getInstance().getUser("massej6it", "Jarno Vermassen"));
            users.Add(await Connector.getInstance().getUser("sonnew", "Wout Versonnen"));
            users.Add(await Connector.getInstance().getUser("StanVDL", "Stan Vanderlinden"));
            users.Add(await Connector.getInstance().getUser("spookykat", "Kamiel Brees"));
            
            users.Add(await Connector.getInstance().getUser("harry2love", "Harman Sing"));
            users.Add(await Connector.getInstance().getUser("CasualSD", "Noah Maes"));
            users.Add(await Connector.getInstance().getUser("Lucidious45", "Yannick Van Den Hoof"));
            users.Add(await Connector.getInstance().getUser("Viktor6IT", "Viktor Ons")) ;
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
                            Console.WriteLine("* Commit: " + commit.Commit.Message);
                              
                            // only add commits after start date
                            if (commit.Commit.Author.Date >= start)
                            {
                                string login = FindLoginForCommit(commit);
                                if (login == null)
                                {
                                    Console.WriteLine("  => Discarding: cannot find author");
                                    Console.WriteLine("  => Assumed Author Name: " + commit.Commit.Author.Name);
                                    continue;
                                }

                                Console.WriteLine("  => Adding for evaluation.");
                                await AddCommit(login, commit.Commit.Author.Date.DateTime, commit.Sha, repo);
                            } else
                            {
                                
                                Console.WriteLine("  => Discarding commit because it's not recent enough.");
                                
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

        private static string FindLoginForCommit(GitHubCommit commit)
        {
            
            if (commit.Author != null)
            {
                return commit.Author.Login;
            } else
            {
                foreach(var user in users)
                {
                    if (user.Name.Equals(commit.Commit.Author.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return user.Login;
                    }
                }
            }
            return null;
        }
    }
}
