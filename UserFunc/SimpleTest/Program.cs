using System;
using System.Collections.Generic;
using System.Text;
using LuaInterface;

namespace SimpleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestCollection test = new TestCollection();
            //L.LoadFile(".\\test.lua");
            test.BeginTest();

            Console.ReadKey(true);
        }


        //Function
    }

    class TestCollection
    {
        Lua L;

        public TestCollection()
        {
            L = new Lua();
            L.RegisterFunction("FirstFunction", this, this.GetType().GetMethod("FirstFunction"));
            L.DoFile(".\\test.lua");     
        }

        public void BeginTest()
        {
            L.GetFunction("SecondTest").Call();
        }

        public void FirstFunction(string str)
        {
            Console.WriteLine("First function passed, congratulations!");
            Console.WriteLine("They say: " + str);
        }
    }
}
