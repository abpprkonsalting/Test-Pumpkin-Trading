using System;

namespace PumpkinTrade.Model
{
    public class Client
    {
        public Client(String name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
        public Guid Id { get; }
        public string Name { get; private set; }
    }
}
