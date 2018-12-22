using System;
using System.Collections.Generic;

using PumpkinTrade.Model;

namespace PumpkinTrade.Dal.Implementations
{
    public class ClientsRepository
    {
        private List<Client> Clients;

        public ClientsRepository()
        {
            Clients.Add(new Client("Default Client"));
        }
        public void Add(String name)
        {
            Clients.Add(new Client(name));
        }

        public Client GetById (uint id)
        {
            return Clients[(int)id];
        }

        public bool Exist (uint id)
        {
            if (id <= Clients.Count - 1) return true;
            else return false;
        }
    }
}
