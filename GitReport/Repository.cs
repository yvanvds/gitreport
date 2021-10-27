using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitReport
{
    class Repository
    {
        Octokit.Repository repository;

        public long ID { get => repository.Id; }

        public string Name { get => repository.Name; }

        public string Author { get => repository.Owner.Login; }

        public Repository(Octokit.Repository repository)
        {
            this.repository = repository;
        }

        public void PrintDetails()
        {
            Console.WriteLine();
            Console.WriteLine("Repository: " + repository.Name);
        }
    }
}
