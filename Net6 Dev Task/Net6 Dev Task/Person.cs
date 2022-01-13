using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Net6_Dev_Task
{
    public class Person
    {
        public string user { get; private set; }
        public string password { get; private set; } //DO NOT SAVE ACTUAL PASSWD IN PLAINTEXT!!!!!!!
        public string name { get; private set; }

        public Person()
        {
            user = null;
            password = null;
            name = null;
        }

        [JsonConstructor]
        public Person(string user, string password, string name)
        {
            this.user = user;
            this.password = password;
            this.name = name;
        }
        public override bool Equals(object obj)
        {
            var other = obj as Person;

            if (other == null)
                return false;

            return this.user == other.user;
        }

        public override string ToString()
        {
            return String.Format("| {0,-10} | {1,-20} |",
                user, name);
        }
    }
}
