using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net6_Dev_Task
{
    public class Session
    {
        public Person person { get; private set; }

        public Session()
        {
            person = null;
        }

        public Session(Person user)
        {
            person = user;
        }
    }
}
