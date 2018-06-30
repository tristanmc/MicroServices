using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public interface IRepository
    {
        Task<string> Test { get; }
    }

    public class Repository : IRepository
    {

        public Task<string> Test => Task.FromResult("Tested");
    }
}
