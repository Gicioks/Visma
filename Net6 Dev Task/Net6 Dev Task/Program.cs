using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Net6_Dev_Task
{
    class program
    {
        ///////////////////////////////////////////////////////////////////////
        /// Globals

        static Session session = null;
        static List<Person> people = null;
        static List<Meeting> meetings = null;

        const string peopleFilePath = "..\\..\\..\\PEOPLE.json";
        const string meetingsFilePath = "..\\..\\..\\MEETINGS.json";

        static bool programLoopDone;
        static int currentScreen;

        static int maxID = -1;

        ///////////////////////////////////////////////////////////////////////
        /// Main

        static void Main(string[] args)
        {
            people = LoadPeople(peopleFilePath);
            meetings = LoadMeetings(meetingsFilePath);

            maxID = GetMaxMeetID();

            programLoopDone = false;
            currentScreen = 0;

            while (!programLoopDone)
            {
                switch (currentScreen)
                {
                    case 0:
                        switch (WelcomeScreen())
                        {
                            case 1:
                                currentScreen = 11; //login screen
                                break;
                            case 2:
                                currentScreen = 12; //register screen
                                break;
                            case 3:
                                Exit();
                                break;
                            default:
                                Console.WriteLine("Welcome screen returned unexpected value.");
                                Exit();
                                break;
                        }
                        break;
                    case 11:
                        //login screen
                        switch (LogIn())
                        {
                            case true:
                                currentScreen = 2;
                                Console.Clear();
                                break;
                            default:
                                currentScreen = 0;
                                Console.Clear();
                                break;
                        }
                        break;
                    case 12:
                        //register screen
                        switch (Register())
                        {
                            case true:
                                WritePeople(peopleFilePath);
                                currentScreen = 2;
                                Console.Clear();
                                break;
                            default:
                                currentScreen = 0;
                                Console.Clear();
                                break;
                        }
                        break;
                    case 2:
                        //main screen
                        switch (MainPageScreen())
                        {
                            case 1:
                                ListMeetingsDefault();
                                break;
                            case 2:
                                Console.Clear();
                                currentScreen = 21;
                                break;
                            case 3:
                                AddMeeting();
                                WriteMeetings(meetingsFilePath);
                                break;
                            case 4:
                                RemoveMeeting();
                                WriteMeetings(meetingsFilePath);
                                break;
                            case 5:
                                AddPerson();
                                WriteMeetings(meetingsFilePath);
                                break;
                            case 6:
                                RemovePerson();
                                WriteMeetings(meetingsFilePath);
                                break;
                            case 7:
                                Exit();
                                break;
                            case 8:
                                Console.Clear();
                                LogOut();
                                currentScreen = 0;
                                break;
                        }
                        break;
                    case 21:
                        //filter screen
                        switch (FilterPageScreen())
                        {
                            case 1:
                                ListMeetingsByDescription();
                                break;
                            case 2:
                                ListMeetingsByResponsiblePerson();
                                break;
                            case 3:
                                ListMeetingsByCategory();
                                break;
                            case 4:
                                ListMeetingsByType();
                                break;
                            case 5:
                                ListMeetingsByDate();
                                break;
                            case 6:
                                ListMeetingsByAttending();
                                break;
                            case 99:
                                Exit();
                                break;
                            case 10:
                                Console.Clear();
                                currentScreen = 2;
                                break;
                        }
                        break;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////
        /// File operations

        private static List<Person> LoadPeople(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            var array = JArray.Parse(jsonString);

            List<Person> list = new List<Person>();

            foreach (var item in array)
            {
                try
                {
                    list.Add(JsonSerializer.Deserialize<Person>(item.ToString()));
                }
                catch (Exception)
                { }
            }

            return list;
        }
        private static List<Meeting> LoadMeetings(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            var array = JArray.Parse(jsonString);

            List<Meeting> list = new List<Meeting>();

            foreach (var item in array)
            {
                try
                {
                    list.Add(JsonSerializer.Deserialize<Meeting>(item.ToString()));

                }
                catch (Exception)
                { }
            }

            return list;
        }

        private static void WritePeople(string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = System.Text.Json.JsonSerializer.Serialize(people, options);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(json);
            }
        }
        private static void WriteMeetings(string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = System.Text.Json.JsonSerializer.Serialize(meetings, options);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(json);
            }
        }

        ///////////////////////////////////////////////////////////////////////
        /// Screens

        /// <summary>
        /// Welcome Screen
        /// </summary>
        /// <returns>Return code</returns>

        private static int WelcomeScreen()
        {
            string[] commands = { "login", "register", "exit" };

            Console.WriteLine("Meeting Manager");
            Console.WriteLine("Type \"login\", \"register\" (if you are new user), or type \"exit\"");

            bool sucess = false;
            string command = WaitForInput();

            while (!sucess)
            {
                if (command.ToLower() == "login")
                {
                    sucess = true;
                    return 1;
                }
                else if (command.ToLower() == "register")
                {
                    sucess = true;
                    return 2;
                }
                else if (command.ToLower() == "exit")
                {
                    sucess = true;
                    return 3;
                }
                else
                {
                    ThrowError(command);
                    command = WaitForInput();
                }
            }

            return -1;
        }

        /// <summary>
        /// Main Screen
        /// </summary>
        /// <returns>Return code</returns>
        private static int MainPageScreen()
        {
            Console.WriteLine("Type \"list\", \"listF\", \"addM\", \"removeM\", \"addP\", \"removeP\", \"exit\", \"logout\"\r\n");
            Console.WriteLine("\"list\" - lists all meetings, \"listF\" - lists all meetings by filter");
            Console.WriteLine("\"addM\", \"removeM\" - adds/removes meeting");
            Console.WriteLine("\"addP\", \"removeP\" - adds/removes person to/from meeting");

            bool sucess = false;
            string command = WaitForInput();

            while (!sucess)
            {
                switch (command.ToLower())
                {
                    case "list":
                        sucess = true;
                        return 1; //list default
                    case "listf":
                        sucess = true;
                        return 2; //list filtered
                    case "addm":
                        sucess = true;
                        return 3; //add meeting
                    case "removem":
                        sucess = true;
                        return 4; //remove meeting
                    case "addp":
                        sucess = true;
                        return 5; //add person
                    case "removep":
                        sucess = true;
                        return 6; //remove person
                    case "exit":
                        sucess = true;
                        return 7; //exit
                    case "logout":
                        return 8;
                    default:
                        ThrowError(command);
                        command = WaitForInput();
                        break;
                }
            }

            return -1;
        }

        private static int FilterPageScreen()
        {
            Console.WriteLine("Type \"byDes\", \"byRes\", \"byCat\", \"byType\", \"byDate\", \"byAtt\", \"cancel\", \"exit\"\r\n");
            
            Console.WriteLine("\"byDes\" - Lists meetings by given description.");
            Console.WriteLine("\"byRes\" - Lists meetings by given responsible person.");
            Console.WriteLine("\"byCat\" - Lists meetings by given category.");
            Console.WriteLine("\"byType\" - Lists meetings by given type.");
            Console.WriteLine("\"byDate\" - Lists meetings by given date interval.");
            Console.WriteLine("\"byAtt\" - Lists meetings by given attendee number or more.");

            bool sucess = false;
            string command = WaitForInput();

            while (!sucess)
            {
                switch (command.ToLower())
                {
                    case "bydes":
                        sucess = true;
                        return 1; //by Descritpion
                    case "byres":
                        sucess = true;
                        return 2; //by Owner
                    case "bycat":
                        sucess = true;
                        return 3; //by Category
                    case "bytype":
                        sucess = true;
                        return 4; //by Type
                    case "bydate":
                        sucess = true;
                        return 5; //by Dates
                    case "byatt":
                        sucess = true;
                        return 6; //by People Count
                    case "exit":
                        sucess = true;
                        return 99; //exit
                    case "cancel":
                        return 10; //cancel
                    default:
                        ThrowError(command);
                        command = WaitForInput();
                        break;
                }
            }

            return -1;
        }

        ///////////////////////////////////////////////////////////////////////
        /// Meeting

        /// <summary>
        /// List non-sorted (default by id)
        /// </summary>
        private static void ListMeetingsDefault()
        {
            Console.Clear();

            PrintMeetingList(meetings);
            Console.WriteLine();
        }
        private static void PrintMeetingList(List<Meeting> list)
        {
            PrintMeetingTop();

            foreach (var meeting in list)
            {
                Console.WriteLine(meeting.ToString());
            }
        }
        private static void PrintMeetingTop()
        {
            string line = new string('-', 126);
            //4, 20, 20, 15, 10, 10, 10 | 111 + 12 + 3 | 126
            string top = String.Format("| {0,-4} | {1,-20} | {2,-20} | {3,-15} | {4,-10} | {5,-12} | {6,-10} | {7,-10} |",
                "ID", "Responsible Person", "Description", "Category", "Type", "Participants", "Start Date", "End Date");

            Console.WriteLine(line);
            Console.WriteLine(top);
            Console.WriteLine(line);
        }

        /// <summary>
        /// Add meeting sub-screen
        /// </summary>
        private static void AddMeeting()
        {
            int counter = 0;

            bool nameCheck = false;
            bool descriptionCheck = false;
            bool categoryCheck = false;
            bool typeCheck = false;
            bool startCheck = false;
            bool endCheck = false;

            string dateFormat = "yyyy-MM-dd";

            Console.Clear();

            string name = "";
            Person owner = GetPersonWithData(session.person);
            string description = "";
            int category = -1;
            int type = -1;
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;

            Console.WriteLine("Meeting Name: ");
            name = WaitForInput();

            while (!nameCheck && counter <= 5)
            {
                counter++;

                if (name.Length == 0)
                {
                    Console.WriteLine("Name cannot be empty.");
                    name = WaitForInput();
                }
                else
                {
                    nameCheck = true;
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            counter = 0;

            Console.WriteLine("Meeting Description:");
            description = WaitForInput();

            while (!descriptionCheck && counter <= 5)
            {
                counter++;

                if (description.Length == 0)
                {
                    Console.WriteLine("Description cannot be empty.");
                    description = WaitForInput();
                }
                else
                {
                    descriptionCheck = true;
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            counter = 0;

            Console.WriteLine("Choose category: CodeMonkey(1) / Hub(2) / Short(3) / TeamBuilding(4)");
            string catSTR = WaitForInput();

            while (!categoryCheck && counter <= 5)
            {
                counter++;

                if (Regex.IsMatch(catSTR, @"^\d+$")) //if numbers only
                {
                    category = int.Parse(catSTR);
                    if (category <= 4 && category >= 1)
                    {
                        categoryCheck = true;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect category.");

                        Console.WriteLine("Choose category: CodeMonkey(1) / Hub(2) / Short(3) / TeamBuilding(4)");
                        catSTR = WaitForInput();
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input. Enter numbers only");

                    Console.WriteLine("Choose category: CodeMonkey(1) / Hub(2) / Short(3) / TeamBuilding(4)");
                    catSTR = WaitForInput();
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            counter = 0;

            Console.WriteLine("Choose type: Live(0) / InPerson(1)");
            string typeSTR = WaitForInput();

            while (!typeCheck && counter <= 5)
            {
                counter++;

                if (Regex.IsMatch(typeSTR, @"^\d+$")) //if numbers only
                {
                    type = int.Parse(typeSTR);
                    if (type == 1 || type == 0)
                    {
                        typeCheck = true;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect type.");

                        Console.WriteLine("Choose type: Live(0) / InPerson(1)");
                        typeSTR = WaitForInput();
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input. Enter numbers only");

                    Console.WriteLine("Choose category: CodeMonkey(1) / Hub(2) / Short(3) / TeamBuilding(4)");
                    typeSTR = WaitForInput();
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            Console.WriteLine("Start date (yyyy-MM-dd):");
            string startDateSTR = WaitForInput();
            
            counter = 0;

            while (!startCheck && counter <= 5)
            {
                counter++;

                try
                {
                    startDate = DateTime.ParseExact(startDateSTR, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
                    startCheck = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Error parsing date, check syntax.");

                    Console.WriteLine("Start date (yyyy-MM-dd):");
                    startDateSTR = WaitForInput();
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            Console.WriteLine("End date (yyyy-MM-dd):");
            string endDateSTR = WaitForInput();

            counter = 0;

            while (!endCheck && counter <= 5)
            {
                counter++;

                try
                {
                    endDate = DateTime.ParseExact(endDateSTR, dateFormat, System.Globalization.CultureInfo.InvariantCulture);

                    if (endDate.Ticks <= startDate.Ticks)
                    {
                        Console.WriteLine("Wrong date (End date must be later than Start date).");

                        Console.WriteLine("End date (yyyy-MM-dd):");
                        endDateSTR = WaitForInput();
                    }
                    else
                    {
                        endCheck = true;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error parsing date, check syntax.");

                    Console.WriteLine("End date (yyyy-MM-dd):");
                    endDateSTR = WaitForInput();
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            maxID++;
            Meeting meeting = new Meeting(maxID, name, owner, description, category, type, startDate, endDate);
            meetings.Add(meeting);

            Console.Clear();
            Console.WriteLine("Successfully added meeting.");
            Console.WriteLine();
        }
        /// <summary>
        /// Remove meeting sub-screen
        /// </summary>
        private static void RemoveMeeting()
        {
            int counter = 0;

            bool meetingConfirm = false;

            Console.Clear();
            Console.WriteLine("Select meeting by ID to be deleted:");
            ListMeetingsDefault();
            Console.WriteLine();

            Console.WriteLine("Select meeting by ID:");
            PrintMeetingList(meetings);

            Meeting meetingTmp = null;
            string meetingIdSTR = WaitForInput();

            while (!meetingConfirm && counter <= 5)
            {
                counter++;

                if (Regex.IsMatch(meetingIdSTR, @"^\d+$")) //if numbers only
                {
                    int meetingId = int.Parse(meetingIdSTR);

                    meetingTmp = new Meeting(meetingId);
                    if (!CheckExistant(meetingTmp, meetings)) //if meeting doesnt exists
                    {
                        Console.WriteLine("Meeting with ID \"" + meetingId + "\" does not exist.");
                        Console.Write("Select meeting by ID: ");

                        meetingIdSTR = WaitForInput();
                    }
                    else
                    {
                        meetingTmp = GetMeetingWithData(meetingTmp);
                        if (!CheckMeetingOwner(session.person, meetingTmp)) //if user owns meeting
                        {
                            Console.WriteLine("You are not responsible for \"" + meetingId + "\".");
                            Console.Write("Select meeting by ID: ");

                            meetingIdSTR = WaitForInput();
                        }
                        else
                        {
                            meetingConfirm = true;

                            Console.Clear();

                            Console.WriteLine("People in \"" + meetingIdSTR + "\" meeting:");
                            meetingTmp.PrintPeople();
                            Console.WriteLine();

                            Console.WriteLine("All existing people:");
                            PrintPeopleList(people);
                            Console.WriteLine();
                        }
                    }
                }
                else //if not numbers
                {
                    Console.WriteLine("Incorrect input. Enter numbers only");
                    Console.Write("Select meeting by ID: ");

                    meetingIdSTR = WaitForInput();
                }

                if(counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            counter = 0;

            meetings.Remove(meetingTmp);

            Console.Clear();
            Console.WriteLine("Sucessfully removed meeting.");
            Console.WriteLine();
        }

        ///////////////////////////////////////////////////////////////////////
        /// People

        /// <summary>
        /// Print people list
        /// </summary>
        private static void PrintPeopleList(List<Person> list)
        {
            PrintPeopleTop();

            foreach(Person person in list)
            {
                Console.WriteLine(person.ToString());
            }
        }
        private static void PrintPeopleTop()
        {
            string line = new string('-', 37);
            string top = String.Format("| {0,-10} | {1,-20} |",
                "Username", "Name");

            Console.WriteLine(line);
            Console.WriteLine(top);
            Console.WriteLine(line);
        }

        /// <summary>
        /// Add Person
        /// </summary>
        private static void AddPerson()
        {
            int counter = 0;

            bool meetingConfirm = false;
            bool personConfirm = false;
            bool hrsConfirm = false;

            bool correctHrs = false;
            bool correctMins = false;

            Console.Clear();
            Console.WriteLine("Select meeting by ID:");
            PrintMeetingList(meetings);

            Meeting meetingTmp = null;
            string meetingIdSTR = WaitForInput();

            while (!meetingConfirm && counter <= 5)
            {
                counter++;

                if (Regex.IsMatch(meetingIdSTR, @"^\d+$")) //if numbers only
                {
                    int meetingId = int.Parse(meetingIdSTR);

                    meetingTmp = new Meeting(meetingId);
                    if (!CheckExistant(meetingTmp, meetings)) //if meeting doesnt exists
                    {
                        Console.WriteLine("Meeting with ID \"" + meetingId + "\" does not exist.");
                        Console.Write("Select meeting by ID: ");

                        meetingIdSTR = WaitForInput();
                    }
                    else
                    {
                        meetingTmp = GetMeetingWithData(meetingTmp);
                        if (!CheckMeetingOwner(session.person, meetingTmp)) //if user owns meeting
                        {
                            Console.WriteLine("You are not responsible for \"" + meetingId + "\".");
                            Console.Write("Select meeting by ID: ");

                            meetingIdSTR = WaitForInput();
                        }
                        else
                        {
                            meetingConfirm = true;

                            Console.Clear();

                            Console.WriteLine("People in \"" + meetingIdSTR + "\" meeting:");
                            meetingTmp.PrintPeople();
                            Console.WriteLine();

                            Console.WriteLine("All existing people:");
                            PrintPeopleList(people);
                            Console.WriteLine();
                        }
                    }
                }
                else //if not numbers
                {
                    Console.WriteLine("Incorrect input. Enter numbers only");
                    Console.Write("Select meeting by ID: ");

                    meetingIdSTR = WaitForInput();
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            counter = 0;

            Person personTmp = null;

            Console.WriteLine("Select Person to add to meeting \"" + meetingIdSTR + "\" by username:");
            string personUser = WaitForInput();
            personTmp = new Person(personUser, null, null);

            while (!personConfirm && counter <= 5)
            {
                counter++;

                if (!CheckExistant(personTmp, people) || CheckExistant(personTmp, meetingTmp.people)) //if person doesnt exist or already is in meeting
                {
                    Console.WriteLine("Username \"" + personUser + "\" does not exist or already is in the meeting.");

                    Console.WriteLine("Select Person to add to meeting \"" + meetingIdSTR + "\" by username:");

                    personUser = WaitForInput();
                    personTmp = new Person(personUser, null, null);
                }
                else
                {
                    personConfirm = true;
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            counter = 0;

            DateTime start = meetingTmp.startDate;
            DateTime end = meetingTmp.endDate;

            int hours = -1;
            int minutes = -1;

            while (!hrsConfirm && counter <= 5)
            {
                counter++;

                Console.WriteLine("Time to start:");
                Console.Write("hours: ");
                string hoursStr = WaitForInput();

                while (!correctHrs)
                {
                    if (Regex.IsMatch(hoursStr, @"^\d+$")) //if numbers only
                    {
                        hours = int.Parse(hoursStr);
                        if (hours >= 0 && hours < 24)
                        {
                            correctHrs = true;
                        }
                        else
                        {
                            Console.WriteLine("Incorrect hours.");

                            Console.Write("hours: ");
                            hoursStr = WaitForInput();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect input. Enter numbers only");

                        Console.Write("hours: ");
                        hoursStr = WaitForInput();
                    }

                    if (counter == 5)
                    {
                        Console.Clear();
                        Console.WriteLine("Too Many attempts");
                        Console.WriteLine();

                        return;
                    }
                }

                counter = 0;

                Console.Write("minutes: ");
                String minutesStr = WaitForInput();

                while (!correctMins && counter <= 5)
                {
                    counter++;

                    if (Regex.IsMatch(minutesStr, @"^\d+$")) //if numbers only
                    {
                        minutes = int.Parse(minutesStr);
                        if (minutes >= 0 && minutes < 60)
                        {
                            correctMins = true;
                        }
                        else
                        {
                            Console.WriteLine("Incorrect minutes.");

                            Console.Write("minutes:");
                            minutesStr = WaitForInput();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect input. Enter numbers only");

                        Console.Write("minutes: ");
                        minutesStr = WaitForInput();
                    }

                    if (counter == 5)
                    {
                        Console.Clear();
                        Console.WriteLine("Too Many attempts");
                        Console.WriteLine();

                        return;
                    }
                }

                DateTime startAdded = start;
                startAdded = start.AddHours(hours);
                startAdded = start.AddMinutes(minutes);
                personTmp = GetPersonWithData(personTmp);

                if (BusyCheck(personTmp, startAdded, end)) //incorrect time
                {
                    Console.WriteLine("Incorrect time.");
                    Console.Write("Returning to Main page.");

                    System.Threading.Thread.Sleep(1000);
                    Console.Write(".");
                    System.Threading.Thread.Sleep(1000);
                    Console.WriteLine(".");
                    System.Threading.Thread.Sleep(1000);

                    Console.Clear();
                }
                else
                {
                    meetingTmp.AddPerson(personTmp, startAdded);

                    Console.Clear();
                    Console.WriteLine("Sucessfully added person to meeting.");
                    Console.WriteLine();

                    hrsConfirm = true;
                }

                hrsConfirm = true; //bad design, rushed UI
            }
        }
        /// <summary>
        /// Remove person sub-screen
        /// </summary>
        private static void RemovePerson()
        {
            int counter = 0;

            bool meetingConfirm = false;
            bool personConfirm = false;

            Console.Clear();
            Console.WriteLine("Select meeting by ID:");
            PrintMeetingList(meetings);

            Meeting meetingTmp = null;
            string meetingIdSTR = WaitForInput();

            while (!meetingConfirm && counter <= 5)
            {
                counter++;

                if (Regex.IsMatch(meetingIdSTR, @"^\d+$")) //if numbers only
                {
                    int meetingId = int.Parse(meetingIdSTR);

                    meetingTmp = new Meeting(meetingId);
                    if (!CheckExistant(meetingTmp, meetings)) //if meeting doesnt exists
                    {
                        Console.WriteLine("Meeting with ID \"" + meetingId + "\" does not exist.");
                        Console.Write("Select meeting by ID: ");

                        meetingIdSTR = WaitForInput();
                    }
                    else
                    {
                        meetingTmp = GetMeetingWithData(meetingTmp);
                        if (!CheckMeetingOwner(session.person, meetingTmp)) //if user owns meeting
                        {
                            Console.WriteLine("You are not responsible for \"" + meetingId + "\".");
                            Console.Write("Select meeting by ID: ");

                            meetingIdSTR = WaitForInput();
                        }
                        else
                        {
                            meetingConfirm = true;

                            Console.Clear();

                            Console.WriteLine("People in \"" + meetingIdSTR + "\" meeting:");
                            meetingTmp.PrintPeople();
                            Console.WriteLine();
                        }
                    }
                }
                else //if not numbers
                {
                    Console.WriteLine("Incorrect input. Enter numbers only");
                    Console.Write("Select meeting by ID: ");

                    meetingIdSTR = WaitForInput();
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            counter = 0;

            meetingTmp = GetMeetingWithData(meetingTmp);

            Console.WriteLine("Select Person to add to meeting \"" + meetingIdSTR + "\" by username:");
            string personUser = WaitForInput();
            Person personTmp = new Person(personUser, null, null);

            while (!personConfirm && counter <= 5)
            {
                counter++;

                if (!CheckExistant(personTmp, people) && CheckExistant(personTmp, meetingTmp.people) && CheckMeetingOwner(personTmp, meetingTmp)) //if person doesnt exist, is in a meeting and is owner
                {
                    Console.WriteLine("Username \"" + personUser + "\" does not exist or already is in the meeting, or is owner.");

                    Console.WriteLine("Select Person to add to meeting \"" + personUser + "\" by username:");

                    personUser = WaitForInput();
                    personTmp = new Person(personUser, null, null);
                }
                else
                {
                    personConfirm = true;
                    personTmp = meetingTmp.GetPerson(personTmp);
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            meetingTmp.RemovePerson(personTmp);

            Console.Clear();
            Console.WriteLine("Sucessfully removed person from meeting.");
        }
        private static Person GetPersonWithData(Person person)
        {
            foreach (var p in people)
            {
                if (p.Equals(person))
                    return p;
            }

            return person;
        }

        ///////////////////////////////////////////////////////////////////////
        /// Filtering

        /// <summary>
        /// Filters by description
        /// </summary>
        private static void ListMeetingsByDescription()
        {
            int counter = 0;

            bool correct = false;

            Console.WriteLine("Enter meeting description or part of it:");
            string desc = WaitForInput();

            while(!correct && counter <= 5)
            {
                counter++;

                if (desc.Length == 0)
                {
                    Console.WriteLine("Incorrect input. Description should not be empty");
                    Console.WriteLine("Enter meeting description or part of it:");
                    desc = WaitForInput();
                }
                else
                {
                    correct = true;
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            Console.Clear();

            List<Meeting> filtered = new List<Meeting>();

            foreach (var m in meetings)
            {
                if (m.description.ToLower().Contains(desc.ToLower()))
                {
                    filtered.Add(m);
                }
            }

            Console.WriteLine("Meetings that contain \"" + desc + "\" in their description:");
            PrintMeetingList(filtered);
        }
        /// <summary>
        /// Filters by owner
        /// </summary>
        private static void ListMeetingsByResponsiblePerson()
        {
            int counter = 0;

            bool correct = false;

            PrintPeopleList(people);
            Console.WriteLine("Enter responsible person username:");

            string user = WaitForInput();
            Person person = new Person(user, null, null);

            while (!correct && counter <= 5)
            {
                counter++;

                if (user.Length == 0)
                {
                    Console.WriteLine("Incorrect input. Username should not be empty");
                    Console.WriteLine("Enter responsible person username:");
                    user = WaitForInput();
                }
                else
                {
                    person = new Person(user, null, null);
                    if (!CheckExistant(person, people))
                    {
                        Console.WriteLine("\"" + user + "\" user does not exist.");
                        user = WaitForInput();
                    }
                    else
                        correct = true;
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            Console.Clear();

            List<Meeting> filtered = new List<Meeting>();

            foreach (var m in meetings)
            {
                if (CheckMeetingOwner(person, m))
                {
                    filtered.Add(m);
                }
            }

            Console.WriteLine("Meetings managed by \"" + user + "\" user:");
            PrintMeetingList(filtered);
        }
        /// <summary>
        /// Filters by category
        /// </summary>
        private static void ListMeetingsByCategory()
        {
            int counter = 0;

            bool correct = false;

            Console.WriteLine("Select meeting category: CodeMonkey(1) / Hub(2) / Short(3) / TeamBuilding(4)");
            string catSTR = WaitForInput();

            while (!correct && counter <= 5)
            {
                counter++;

                if (Regex.IsMatch(catSTR, @"^\d+$"))
                {
                    int catTMP = int.Parse(catSTR);
                    if (catTMP <= 4 && catTMP >= 1)
                    {
                        correct = true;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect category.");
                        Console.WriteLine("Select meeting category: CodeMonkey(1) / Hub(2) / Short(3) / TeamBuilding(4)");
                        catSTR = WaitForInput();
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input. Enter numbers only.");
                    Console.WriteLine("Select meeting category: CodeMonkey(1) / Hub(2) / Short(3) / TeamBuilding(4)");
                    catSTR = WaitForInput();
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            Console.Clear();

            string[] categories = { "CodeMonkey", "Hub", "Short", "TeamBuilding" };
            List<Meeting> filtered = new List<Meeting>();
            int cat = int.Parse(catSTR);

            foreach (var m in meetings)
            {
                if (m.category == cat)
                {
                    filtered.Add(m);
                }
            }

            Console.WriteLine("Meetings of " + categories[cat - 1] + " category:");
            PrintMeetingList(filtered);
        }
        /// <summary>
        /// Filters by type
        /// </summary>
        private static void ListMeetingsByType()
        {
            int counter = 0;

            bool correct = false;

            Console.WriteLine("Select meeting type: Live(0) / InPerson(1)");
            string typeSTR = WaitForInput();

            while (!correct && counter <= 5)
            {
                counter++;

                if (Regex.IsMatch(typeSTR, @"^\d+$"))
                {
                    int typeTmp = int.Parse(typeSTR);
                    if (typeTmp == 1 || typeTmp == 0)
                    {
                        correct = true;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect type.");
                        Console.WriteLine("Select meeting type: Live(0) / InPerson(1)");
                        typeSTR = WaitForInput();
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input. Enter numbers only.");
                    Console.WriteLine("Select meeting type: Live(0) / InPerson(1)");
                    typeSTR = WaitForInput();
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            Console.Clear();

            string[] types = { "Live", "InPerson" };
            List<Meeting> filtered = new List<Meeting>();
            int type = int.Parse(typeSTR);

            foreach (var m in meetings)
            {
                if (m.type == type)
                {
                    filtered.Add(m);
                }
            }

            Console.WriteLine("Meetings of " + types[type] + " type:");
            PrintMeetingList(filtered);
        }
        /// <summary>
        /// Filters by dates
        /// </summary>
        private static void ListMeetingsByDate()
        {
            int counter = 0;

            bool startCheck = false;
            bool endCheck = false;

            string dateFormat = "yyyy-MM-dd";
            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MinValue;

            Console.WriteLine("Enter date interval (yyyy-MM-dd):");

            Console.WriteLine("From:");

            string startSTR = WaitForInput();

            while (!startCheck && counter <= 5)
            {
                counter++;

                counter++;

                try
                {
                    start = DateTime.ParseExact(startSTR, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
                    startCheck = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Error parsing date, check syntax.");

                    Console.WriteLine("Start date (yyyy-MM-dd):");
                    startSTR = WaitForInput();
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            Console.WriteLine("End date (yyyy-MM-dd):");
            string endSTR = WaitForInput();

            counter = 0;

            while (!endCheck && counter <= 5)
            {
                counter++;

                counter++;

                try
                {
                    end = DateTime.ParseExact(endSTR, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
                    endCheck = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Error parsing date, check syntax.");

                    Console.WriteLine("End date (yyyy-MM-dd):");
                    endSTR = WaitForInput();
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            Console.Clear();

            List<Meeting> filtered = new List<Meeting>();

            foreach (var m in meetings)
            {
                if (m.startDate.Ticks >= start.Ticks && m.endDate.Ticks <= end.Ticks)
                {
                    filtered.Add(m);
                }
            }

            Console.WriteLine("Meetings in date interval of \"from " + startSTR + " to " + endSTR + "\":");
            PrintMeetingList(filtered);
        }
        /// <summary>
        /// Filters by number of participants
        /// </summary>
        private static void ListMeetingsByAttending()
        {
            int counter = 0;

            bool correct = false;

            Console.WriteLine("Type participant count:");
            string cntSTR = WaitForInput();
            int cnt = -1;

            while (!correct && counter <= 5)
            {
                counter++;

                if (Regex.IsMatch(cntSTR, @"^\d+$"))
                {
                    cnt = int.Parse(cntSTR);
                    if (cnt > 0)
                    {
                        correct = true;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect count.");
                        Console.WriteLine("Type participant count:");
                        cntSTR = WaitForInput();
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input. Enter numbers only.");
                    Console.WriteLine("Type participant count:");
                    cntSTR = WaitForInput();
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return;
                }
            }

            Console.Clear();

            List<Meeting> filtered = new List<Meeting>();

            foreach (var m in meetings)
            {
                if (m.people.Count >= cnt)
                {
                    filtered.Add(m);
                }
            }

            Console.WriteLine("Meetings of at least " + cnt + " participant(s):");
            PrintMeetingList(filtered);
        }

        ///////////////////////////////////////////////////////////////////////
        /// Utility

        /// <summary>
        /// Checks if given object exist in given list
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="other">Object to check</param>
        /// <param name="list">Given list</param>
        /// <returns>If object exists in given list</returns>
        private static bool CheckExistant<T>(T other, List<T> list)
        {
            foreach (object o in list)
            {
                if (o.Equals(other))
                    return true;
            }

            return false;
        }

        private static bool CheckMeetingOwner(Person person, Meeting meeting)
        {
            return person.Equals(meeting.responsiblePerson);
        }

        private static Meeting GetMeetingWithData(Meeting meeting)
        {
            foreach (var m in meetings)
            {
                if (m.Equals(meeting))
                    return m;
            }

            return meeting;
        }

        /// <summary>
        /// Check if person is avaliable during given time
        /// </summary>
        /// <param name="person">Person to check</param>
        /// <param name="startTime">Time interval start</param>
        /// <param name="endTime">Time interval ending</param>
        /// <returns></returns>
        private static bool BusyCheck(Person person, DateTime startTime, DateTime endTime)
        {
            foreach (var m in meetings)
            {
                Person temp = m.GetPerson(person);
                if (temp != null)
                {
                    DateTime start = m.dateTimes[m.people.IndexOf(temp)];
                    DateTime end = m.endDate;

                    if (!(startTime.Ticks >= end.Ticks || endTime.Ticks <= start.Ticks))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static int GetMaxMeetID()
        {
            int max = -1;

            foreach (var m in meetings)
            {
                if (m.id > max)
                    max = m.id;
            }

            return max;
        }

        ///////////////////////////////////////////////////////////////////////
        /// Login / Register / Logout

        /// <summary>
        /// LogIn screen
        /// </summary>
        /// <returns>If operation is successful</returns>
        private static bool LogIn()
        {
            int counter = 0;

            string user = "";
            string password = "";

            bool correctUser = false;
            bool correctPass = false;

            Person temp = null;

            Console.Clear();

            while (!correctUser && counter <= 5)
            {
                counter++;

                Console.Write("Username: ");
                user = WaitForInput();

                temp = new Person(user, null, null);

                if (CheckExistant(temp, people))
                {
                    correctUser = true;
                } 

                if (!correctUser)
                {
                    Console.WriteLine("user \"" + user + "\" does not exist.");
                    temp = null;
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return false;
                }
            }

            while (!correctPass)
            {
                counter++;

                Console.Write("Password: ");
                password = ReadPassword();

                foreach (var person in people)
                {
                    if (person.password == password)
                    {
                        correctPass = true;
                    }
                }

                if (!correctPass)
                {
                    Console.WriteLine("Password is incorrect.");
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return false;
                }
            }

            session = new Session(temp);
            return true;
        }
        /// <summary>
        /// Register screen
        /// </summary>
        /// <returns>If operation is successful</returns>
        private static bool Register()
        {
            int counter = 0;

            string user = "";
            string password = "";
            string name = "";

            bool correctUsername = false;
            bool correctPass = false;
            bool correctName = false;

            Person temp = null;

            while (!correctUsername)
            {
                counter++;

                Console.Write("Username: ");
                user = Console.ReadLine();

                if (user == null || user.Length == 0)
                {
                    Console.WriteLine("Incorrect username.");
                }
                else
                {
                    temp = new Person(user, null, null);

                    if (CheckExistant(temp, people))
                    {
                        Console.WriteLine("Username \"" + user + "\" already taken.");
                        temp = null;
                    }
                    else
                        correctUsername = true;
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return false;
                }
            }

            counter = 0;

            while (!correctPass)
            {
                counter++;

                Console.Write("Password: ");
                password = ReadPassword();

                if (password.Length >= 8)
                {
                    correctPass = true;
                }
                else
                {
                    Console.WriteLine("Password must be at leas 8 symbols long");
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return false;
                }
            }

            counter = 0;

            while (!correctName && counter <= 5)
            {
                counter++;

                Console.Write("Display name: ");
                name = WaitForInput();

                if (name == null || name.Length == 0)
                {
                    Console.WriteLine("Incorrect display name.");
                }
                else
                {
                    correctName = true;
                }

                if (counter == 5)
                {
                    Console.Clear();
                    Console.WriteLine("Too Many attempts");
                    Console.WriteLine();

                    return false;
                }
            }

            temp = new Person(user, password, name);
            CreateNewUser(temp);

            session = new Session(temp);
            return true;
        }
        private static void LogOut()
        {
            session = null;
        }

        private static void Exit()
        {
            Console.WriteLine("Exiting...");
            session = null;
            programLoopDone = true;
            Environment.Exit(0);
        }

        /// <summary>
        /// Reads password with UI
        /// </summary>
        /// <returns>Password</returns>
        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring(0, password.Length - 1);
                        int pos = Console.CursorLeft;
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }
            Console.WriteLine();
            return password;
        }

        private static void CreateNewUser(Person person)
        {
            people.Add(person);
        }

        ///////////////////////////////////////////////////////////////////////
        /// Other

        /// <summary>
        /// Reads input line
        /// </summary>
        /// <returns>Input line</returns>
        private static string WaitForInput()
        {
            string input = Console.ReadLine();

            return input;
        }
        /// <summary>
        /// Displays error message
        /// </summary>
        /// <param name="command">Command that gives error</param>
        private static void ThrowError(string command)
        {
            if (command.Length > 0)
            {
                Console.WriteLine("command \"" + command + "\" does not exist, check your syntax.");
            }
        }
    }
}
