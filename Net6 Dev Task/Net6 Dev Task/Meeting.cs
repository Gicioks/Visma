using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Net6_Dev_Task
{
    internal class Meeting
    {
        public string name { get; private set; }
        public Person responsiblePerson { get; private set; }
        public string description { get; private set; }
        public int category { get; private set; }
        public int type { get; private set; }
        public DateTime startDate { get; private set; }
        public DateTime endDate { get; private set; }

        public int id { get; private set; }
        public List<Person> people { get; private set; }
        public List<DateTime> dateTimes { get; private set; }

        public Meeting()
        {
            name = null;
            responsiblePerson = null;
            description = null;
            category = -1;
            type = -1;
            startDate = DateTime.MinValue;
            endDate = DateTime.MinValue;

            id = -1;
            people = new List<Person>();
        }

        public Meeting(int lastID, string name, Person responsiblePerson, string description, int category, int type, DateTime startDate, DateTime endDate)
        {
            this.name = name;
            this.responsiblePerson = responsiblePerson;
            this.description = description;
            this.category = category;
            this.type = type;
            this.startDate = startDate;
            this.endDate = endDate;

            id = lastID;

            people = new List<Person>();
            people.Add(responsiblePerson);

            dateTimes = new List<DateTime>();
            dateTimes.Add(startDate);
        }

        public Meeting(int id)
        {
            this.id = id;

            this.name = null;
            this.responsiblePerson = null;
            this.responsiblePerson = null;
            this.type = -1;
            this.startDate = DateTime.MinValue;
            this.endDate = DateTime.MinValue;
            people = null;
        }

        [JsonConstructor]
        public Meeting(string name, Person responsiblePerson, string description, int category, int type, DateTime startDate, DateTime endDate, int id, List<Person> people, List<DateTime> dateTimes)
        {
            this.name = name;
            this.responsiblePerson = responsiblePerson;
            this.description = description;
            this.category = category;
            this.type = type;
            this.startDate = startDate;
            this.endDate = endDate;
            this.id = id;
            this.people = people;
            this.dateTimes = dateTimes;
        }

        public bool CheckOwner(Person person)
        {
            return responsiblePerson.Equals(person);
        }

        public void AddPerson(Person person, DateTime time)
        {
            people.Add(person);
            dateTimes.Add(time);
        }
        public Person GetPerson(Person person)
        {
            foreach (var p in people)
            {
                if (p.Equals(person))
                    return p;
            }

            return null;
        }

        public void RemovePerson(Person person)
        {
            dateTimes.RemoveAt(people.IndexOf(person));
            people.Remove(person);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Meeting;

            if (other == null)
                return false;
            
            return this.id == other.id;
        }

        public override string ToString()
        {
            string typeStr = "Error";
            string categoryStr = "Error";

            if (category == 1)
                categoryStr = "CodeMonkey";
            else if (category == 2)
                categoryStr = "Hub";
            else if (category == 3)
                categoryStr = "Short";
            else if (category == 4)
                categoryStr = "TeamBuilding";

            if (type == 0)
                typeStr = "Live";
            else if (type == 1)
                typeStr = "InPerson";

            return String.Format("| {0,-4} | {1,-20} | {2,-20} | {3,-15} | {4,-10} | {5,-12} | {6,-10} | {7,-10} |",
                id, responsiblePerson.name, description, categoryStr, typeStr, people.Count, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
        }

        public void PrintPeople()
        {
            string line = new string('-', 37);
            string top = String.Format("| {0,-10} | {1,-20} |",
                "Username", "Name");

            Console.WriteLine(line);
            Console.WriteLine(top);
            Console.WriteLine(line);

            foreach (Person person in people)
            {
                Console.WriteLine(person.ToString());
            }
        }
    }
}
