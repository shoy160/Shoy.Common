using System;

namespace RemotingModels
{
    public class Person : MarshalByRefObject
    {
        public string GetInfo()
        {
            return "Remoting Test";
        }

        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
