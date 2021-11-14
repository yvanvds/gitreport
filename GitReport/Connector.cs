using Octokit;
using Octokit.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitReport
{
    class Connector
    {
        private GitHubClient client;

        private static Connector instance = null;
        private Connector() 
        {
            
        }

        public bool Init()
        {
            
            var file = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "token");

            if (System.IO.File.Exists(file))
            {
                try
                {
                    string token = System.IO.File.ReadAllText("token");
                    InMemoryCredentialStore credentials = new InMemoryCredentialStore(new Credentials(token));
                    client = new GitHubClient(new ProductHeaderValue("GitReport"), credentials);

                } catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
                
                return true;
            }
            else
            {
                Console.WriteLine("No Github token found.");
                return false;
            }
        }

        public static Connector getInstance()
        {
            if (instance == null)
            {
                instance = new Connector();
            }
            return instance;
        }

        public async Task<GitUser> getUser(string username, string fullname)
        {
            var user = await client.User.Get(username);
            return new GitUser(user, fullname);
        }

        public async Task<List<Repository>> getRepositories(GitUser user)
        {
            var repos = await client.Repository.GetAllForUser(user.Login);
            List<Repository> result = new List<Repository>();
            foreach(var repo in repos)
            {
                result.Add(new Repository(repo));
            }
            return result;
        }

        public async Task<CommitActivity> getCommitActivity(Repository repo)
        {
            return await client.Repository.Statistics.GetCommitActivity(repo.ID);
        }

        public async Task<IReadOnlyList<GitHubCommit>> getCommits(Repository repo)
        {
            
            var result = await client.Repository.Commit.GetAll(repo.ID);
            return result;
        }

        public async Task<GitHubCommit> loadCommit(Repository repo, string sha)
        {
            return await client.Repository.Commit.Get(repo.ID, sha);
        }

        public void printLimits()
        {
            var info = client.GetLastApiInfo();

            Console.WriteLine();
            Console.WriteLine("=== github stats ===");

            if (info == null)
            {
                Console.WriteLine("Make an API call before using this function");
            } else
            {
                Console.WriteLine("You can make {0} requests per hour.", info.RateLimit.Limit);
                Console.WriteLine("You have {0} requests left.", info.RateLimit.Remaining);
                Console.WriteLine("This limit will reset at {0}", info.RateLimit.Reset.LocalDateTime);
            }
        }
    }
}
