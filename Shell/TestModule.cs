using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell
{
    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get["tests"] = args => "Hit tests!";
        }
    }
}
