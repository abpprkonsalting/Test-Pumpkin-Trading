using System;
using System.Collections.Generic;

using PumpkinTrade.Model;

namespace PumpkinTrade.Dal.Implementations
{
    public class ClientsRepository
    {
        private List<Client> Clients;
        private readonly object ClientsLock = new object();

        public ClientsRepository()
        {
            Clients = new List<Client>();
            Clients.Add(new Client("Default Client"));
        }
        public Guid Add(String name)
        {
            lock (ClientsLock)
            {
                Client newClient = new Client(name);
                Clients.Add(newClient);
                return newClient.Id;
            }
        }

        public Client GetById (Guid id)
        {
            return Clients.Find(client => client.Id == id);
        }

        public bool Exist (Guid id)
        {
            return Clients.Exists(client => client.Id == id);
        }
    }
}
