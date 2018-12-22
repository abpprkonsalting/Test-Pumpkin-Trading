using System;

namespace PumpkinTrade.Model
{
    public class Client
    {
        public Client(String name)
        {
            Name = name;
        }
        public string Name { get; private set; }
    }
}
