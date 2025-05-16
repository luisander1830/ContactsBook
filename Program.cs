using System;
using System.Collections.Generic;
using System.IO;

namespace ContactsBook
{
    class Program
    {
        static List<Contact> contacts = new List<Contact>();
        static bool changesMade = false;

        static void Main(string[] args)
        {
            ShowWelcome();
            while (true)
            {
                Console.Clear();
                ShowMenu();
                string option = Console.ReadLine();
                switch (option)
                {
                    case "1": LoadContacts(); break;
                    case "2": ShowContacts(); break;
                    case "3": AddContact(); break;
                    case "4": EditContact(); break;
                    case "5": DeleteContact(); break;
                    case "6": MergeDuplicates(); break;
                    case "7": SaveContacts(); break;
                    case "8": if (ExitApp()) return; break;
                    default: Console.WriteLine("Invalid option. Press ENTER."); Console.ReadLine(); break;
                }
            }
        }

        static void ShowWelcome()
        {
            Console.Clear();
            Console.WriteLine("Welcome to Contact Book!");
            Console.WriteLine("Author: Luisander Arroyo Rivera");
            Console.WriteLine("Version: 1.0 Final");
            Console.WriteLine("Last Revised: 2025-05-16");
            Console.WriteLine("\nThis program allows you to keep records of your contacts.");
            Console.WriteLine("\nPress ENTER to continue.");
            Console.ReadLine();
        }

        static void ShowMenu()
        {
            Console.WriteLine("Main Menu");
            Console.WriteLine("[1] Load contacts from a file.");
            Console.WriteLine("[2] Show all contacts.");
            Console.WriteLine("[3] Add contact.");
            Console.WriteLine("[4] Edit contact.");
            Console.WriteLine("[5] Delete contact.");
            Console.WriteLine("[6] Merge duplicate contacts.");
            Console.WriteLine("[7] Store contacts to a file.");
            Console.WriteLine("[8] Exit the application.");
            Console.Write("\nSelect an option: ");
        }

        static void LoadContacts()
        {
            Console.Clear();
            Console.WriteLine("### Load Contacts from File ###");
            Console.Write("Write the filename or nothing to cancel.\nFilename: ");
            string file = Console.ReadLine().Trim();
            if (file == "")
            {
                Console.WriteLine("Operation canceled. Press ENTER.");
                Console.ReadLine();
                return;
            }
            if (!File.Exists(file))
            {
                Console.WriteLine($"ERROR: File \"{file}\" was not found!\nPress ENTER.");
                Console.ReadLine();
                return;
            }
            try
            {
                List<Contact> temp = new List<Contact>();
                string[] lines = File.ReadAllLines(file);
                for (int i = 0; i < lines.Length; i += 4)
                {
                    string name = lines[i];
                    string last = lines[i + 1];
                    string phone = lines[i + 2];
                    string email = lines[i + 3];
                    temp.Add(new Contact(name, last, phone, email));
                }
                contacts = temp;
                changesMade = false;
                Console.WriteLine("Contacts loaded successfully!\nPress ENTER.");
            }
            catch
            {
                Console.WriteLine("ERROR: An error occurred while reading the file!\nPress ENTER.");
            }
            Console.ReadLine();
        }

        static void SaveContacts()
        {
            Console.Clear();
            Console.WriteLine("### Storing Contacts to File ###");
            Console.Write("Write the filename or nothing to cancel.\nFilename: ");
            string file = Console.ReadLine().Trim();
            if (file == "")
            {
                Console.WriteLine("Operation canceled. Press ENTER.");
                Console.ReadLine();
                return;
            }
            if (File.Exists(file))
            {
                Console.WriteLine($"WARNING: The \"{file}\" file already exists. Do you want to override it? [Y/N]");
                if (!GetYesNo())
                {
                    Console.WriteLine("Operation canceled. Press ENTER.");
                    Console.ReadLine();
                    return;
                }
            }
            try
            {
                using (StreamWriter sw = new StreamWriter(file))
                {
                    foreach (var c in contacts)
                    {
                        sw.WriteLine(c.Name);
                        sw.WriteLine(c.LastName);
                        sw.WriteLine(c.Phone);
                        sw.WriteLine(c.Email);
                    }
                }
                changesMade = false;
                Console.WriteLine("Contacts stored successfully!\nPress ENTER.");
            }
            catch
            {
                Console.WriteLine("ERROR: Could not write to file.\nPress ENTER.");
            }
            Console.ReadLine();
        }

        static void ShowContacts()
        {
            if (contacts.Count == 0)
            {
                Console.WriteLine("No contacts to show. Press ENTER.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Sort by [1] Name [2] Last Name [3] Phone [4] Email:");
            string sortOpt = Console.ReadLine();
            switch (sortOpt)
            {
                case "1": contacts.Sort((a, b) => a.Name.CompareTo(b.Name)); break;
                case "2": contacts.Sort((a, b) => a.LastName.CompareTo(b.LastName)); break;
                case "3": contacts.Sort((a, b) => a.Phone.CompareTo(b.Phone)); break;
                case "4": contacts.Sort((a, b) => a.Email.CompareTo(b.Email)); break;
                default: break;
            }

            int page = 0;
            int perPage = 10;
            int totalPages = (contacts.Count + perPage - 1) / perPage;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("### Show All Contacts ###");
                Console.WriteLine("#   Name       Last Name   Phone        E-mail");
                int start = page * perPage;
                for (int i = start; i < Math.Min(start + perPage, contacts.Count); i++)
                {
                    Console.WriteLine($"{i:000} {contacts[i].Name,-10} {contacts[i].LastName,-10} {contacts[i].Phone,-12} {contacts[i].Email}");
                }
                Console.WriteLine($"Page {page + 1} of {totalPages}");
                Console.Write("Go to [0] Main Menu [-] Prev [+] Next: ");
                string nav = Console.ReadLine().Trim();
                if (nav == "0") break;
                if (nav == "-" && page > 0) page--;
                if (nav == "+" && page < totalPages - 1) page++;
            }
        }

        static void AddContact()
        {
            Console.Clear();
            Console.WriteLine("### Add New Contact ###");
            Console.Write("Name: "); string name = Console.ReadLine();
            Console.Write("Last Name: "); string last = Console.ReadLine();
            Console.Write("Phone: "); string phone = Console.ReadLine();
            Console.Write("Email: "); string email = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(last))
            {
                Console.WriteLine("ERROR: At least name or last name must be present. Press ENTER.");
                Console.ReadLine(); return;
            }
            if (string.IsNullOrWhiteSpace(phone) && string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("ERROR: At least phone or email must be present. Press ENTER.");
                Console.ReadLine(); return;
            }

            Console.Write("Do you want to add the contact? [Y/N] ");
            if (GetYesNo())
            {
                contacts.Add(new Contact(name, last, phone, email));
                changesMade = true;
                Console.WriteLine("Contact added successfully! Press ENTER.");
            }
            else
                Console.WriteLine("Contact not added. Press ENTER.");
            Console.ReadLine();
        }

        static void EditContact()
        {
            Console.Clear();
            Console.WriteLine("### Edit Contact ###");
            Console.Write("Enter index to edit or leave blank to cancel: ");
            string input = Console.ReadLine();
            if (!int.TryParse(input, out int index) || index < 0 || index >= contacts.Count)
            {
                Console.WriteLine("Operation canceled. Press ENTER.");
                Console.ReadLine(); return;
            }
            var c = contacts[index];
            Console.WriteLine($"Current: {c.Name} {c.LastName} {c.Phone} {c.Email}");
            Console.Write("New Name (blank to keep): "); string name = Console.ReadLine();
            Console.Write("New Last Name (blank to keep): "); string last = Console.ReadLine();
            Console.Write("New Phone (blank to keep): "); string phone = Console.ReadLine();
            Console.Write("New Email (blank to keep): "); string email = Console.ReadLine();

            string newName = string.IsNullOrWhiteSpace(name) ? c.Name : name;
            string newLast = string.IsNullOrWhiteSpace(last) ? c.LastName : last;
            string newPhone = string.IsNullOrWhiteSpace(phone) ? c.Phone : phone;
            string newEmail = string.IsNullOrWhiteSpace(email) ? c.Email : email;

            if (string.IsNullOrWhiteSpace(newName) && string.IsNullOrWhiteSpace(newLast))
            {
                Console.WriteLine("ERROR: Name or last name required. Press ENTER."); Console.ReadLine(); return;
            }
            if (string.IsNullOrWhiteSpace(newPhone) && string.IsNullOrWhiteSpace(newEmail))
            {
                Console.WriteLine("ERROR: Phone or email required. Press ENTER."); Console.ReadLine(); return;
            }

            Console.Write("Save changes? [Y/N] ");
            if (GetYesNo())
            {
                contacts[index] = new Contact(newName, newLast, newPhone, newEmail);
                changesMade = true;
                Console.WriteLine("Contact updated! Press ENTER.");
            }
            else
                Console.WriteLine("Edit canceled. Press ENTER.");
            Console.ReadLine();
        }

        static void DeleteContact()
        {
            Console.Clear();
            Console.WriteLine("### Delete Contact ###");
            Console.Write("Enter index to delete or blank to cancel: ");
            string input = Console.ReadLine();
            if (!int.TryParse(input, out int index) || index < 0 || index >= contacts.Count)
            {
                Console.WriteLine("Operation canceled. Press ENTER.");
                Console.ReadLine(); return;
            }
            var c = contacts[index];
            Console.WriteLine($"Delete this contact? {c.Name} {c.LastName} {c.Phone} {c.Email} [Y/N]");
            if (GetYesNo())
            {
                contacts.RemoveAt(index);
                changesMade = true;
                Console.WriteLine("Contact deleted! Press ENTER.");
            }
            else
                Console.WriteLine("Delete canceled. Press ENTER.");
            Console.ReadLine();
        }

        static void MergeDuplicates()
        {
            Console.Clear();
            Console.WriteLine("### Merge Duplicates ###");
            var copy = new List<Contact>(contacts);
            var sets = new List<List<Contact>>();

            while (copy.Count > 0)
            {
                var dup = new List<Contact>();
                var last = copy[copy.Count - 1];
                dup.Add(last);
                copy.RemoveAt(copy.Count - 1);

                for (int i = copy.Count - 1; i >= 0; i--)
                {
                    if (last.IsDuplicate(copy[i]))
                    {
                        dup.Add(copy[i]);
                        copy.RemoveAt(i);
                    }
                }
                if (dup.Count > 1) sets.Add(dup);
            }

            foreach (var set in sets)
            {
                Console.WriteLine("Duplicates found:");
                for (int i = 0; i < set.Count; i++)
                    Console.WriteLine($"{i}: {set[i]}");

                Console.Write("Which holds correct Name (index)? ");
                int nameIdx = GetValidIndex(set.Count);
                Console.Write("Last Name (index)? ");
                int lastIdx = GetValidIndex(set.Count);
                Console.Write("Phone (index)? ");
                int phoneIdx = GetValidIndex(set.Count);
                Console.Write("Email (index)? ");
                int emailIdx = GetValidIndex(set.Count);

                string name = set[nameIdx].Name;
                string last = set[lastIdx].LastName;
                string phone = set[phoneIdx].Phone;
                string email = set[emailIdx].Email;

                Console.Write("Add merged contact? [Y/N] ");
                if (GetYesNo())
                {
                    contacts.Add(new Contact(name, last, phone, email));
                    changesMade = true;
                    Console.WriteLine("Merged contact added.");
                }

                foreach (var d in set)
                {
                    Console.Write($"Delete contact {d}? [Y/N] ");
                    if (GetYesNo())
                    {
                        contacts.Remove(d);
                        changesMade = true;
                        Console.WriteLine("Deleted.");
                    }
                }
            }
            Console.WriteLine("Done. Press ENTER.");
            Console.ReadLine();
        }

        static bool ExitApp()
        {
            Console.Clear();
            Console.WriteLine("### Exit Application ###");
            if (changesMade)
            {
                Console.WriteLine("WARNING: You have made changes that have not been saved.");
                Console.Write("Are you sure you want to exit? [Y/N] ");
            }
            else Console.Write("Are you sure you want to exit? [Y/N] ");

            if (GetYesNo())
            {
                Console.WriteLine("Thank you for using Contacts Book! Until next time!");
                return true;
            }
            return false;
        }

        static bool GetYesNo()
        {
            while (true)
            {
                string input = Console.ReadLine().Trim().ToUpper();
                if (input == "Y") return true;
                if (input == "N") return false;
                Console.Write("Invalid input. Please enter Y or N: ");
            }
        }

        static int GetValidIndex(int max)
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out int index) && index >= 0 && index < max)
                    return index;
                Console.Write($"Invalid index. Enter a number between 0 and {max - 1}: ");
            }
        }
    }

    public class Contact
    {
        public string Name;
        public string LastName;
        public string Phone;
        public string Email;

        public Contact(string name, string lastName, string phone, string email)
        {
            Name = name;
            LastName = lastName;
            Phone = phone;
            Email = email;
        }

        public override string ToString()
        {
            return $"{Name} {LastName} {Phone} {Email}";
        }

        public bool IsDuplicate(Contact other)
        {
            return (Name == other.Name && LastName == other.LastName)
                || Phone == other.Phone
                || Email == other.Email;
        }
    }
}
