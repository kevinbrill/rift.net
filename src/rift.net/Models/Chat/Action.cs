using System;
using rift.net.Models;

namespace rift.net.Models
{
    public class Action
    {
        public Contact Character { get; set; }

        public StateAction State { get; set; }

        public bool InGame { get; set; }

        public bool InWeb
        {
            get { return !InGame; }
        }
    }
}