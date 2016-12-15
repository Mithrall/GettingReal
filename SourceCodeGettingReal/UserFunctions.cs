using System;
using System.Collections.Generic;
using System.Linq;

namespace SourceCodeGettingReal {
    public class UserFunctions {
        public static List<Customer> customers;
        public static int listStartLenght;
        public static List<AvailableTimes> availableDates;
        Customer customer;

        public void Init() {
            customers = new List<Customer>();
            availableDates = new List<AvailableTimes>();
            DBCon dbcon = new DBCon();
            dbcon.spGetAllCustomers();
            listStartLenght = customers.Count();
        }

        public Customer DoesUserExist(int phone) {
            Customer currentCustomer = null;

            while (currentCustomer == null) {
                if (FindCustomerByPhone(phone) != null) {
                    currentCustomer = FindCustomerByPhone(phone);
                } else {
                    Console.WriteLine();
                    Console.WriteLine("Systemet genkender ikke dette nummer, øsnker de at registrere dem?");
                    Console.WriteLine("'nej' hvis du skrev forkert og vil prøve igen");
                    Console.WriteLine("'ja' for at komme til registrering af ny bruger");

                    string newUser;
                    newUser = Console.ReadLine();
                    switch (newUser) {
                        case "ja":
                            return RegisterUser(phone);

                        case "nej":
                            Console.Clear();
                            return customer = null;

                        default:
                            Console.Clear();
                            Console.WriteLine("Skriv enten 'ja' eller 'nej'");
                            break;
                    }
                    currentCustomer = FindCustomerByPhone(phone);
                }
            }
            return currentCustomer;
        }

        public Customer RegisterUser(int phone) {
            customer = new Customer();

            customer.Phone = phone;

            customer.Name = GetName();
            if (customer.Name == "exit") {
                Console.Clear();
                customer = null;
                return customer;
            }

            customer.LastName = GetLastName();
            if (customer.LastName == "exit") {
                Console.Clear();
                customer = null;
                return customer;
            }
            
            Console.Clear();
            Console.WriteLine("Bruger oprettet: ");
            Console.WriteLine("Navn: " + customer.Name + " " + customer.LastName + " - tlf: " + phone);
            customers.Add(customer);
            return customer;
        }

        public bool IsPhoneNumber(string phoneInput) {
            int phone;
            bool canConvert = Int32.TryParse(phoneInput, out phone);
            if (phoneInput.Length == 8 && canConvert == true && !phoneInput.Contains(' ')) {
                return true;
            } else {
                return false;
            }
        }

        public string GetName() {
            string Name;
            Console.Clear();
            Console.WriteLine("Fornavn på bruger");
            do {
                Name = Console.ReadLine();
                Console.Clear();
                Console.WriteLine("Fornavn på bruger, må ikke indholde tal eller være blankt");
            } while (Name == "" || Name.Any(char.IsDigit) || Name.Contains(' '));
            return Name;
        }

        public string GetLastName() {
            string LastName;
            Console.Clear();
            Console.WriteLine("Efternavn på bruger");
            do {
                LastName = Console.ReadLine();
                Console.Clear();
                Console.WriteLine("Efternavn på bruger, må ikke indholde tal eller være blankt");

            } while (LastName == "" || LastName.Any(char.IsDigit) || LastName.Contains(' '));
            return LastName;
        }

        public static Customer FindCustomerByPhone(int searchedPhone) {
            Customer result = customers.Find(x => x.Phone == searchedPhone);
            return result;
        }

        public void ShowTimes(Customer currentCustomer) {
            Console.WriteLine("Liste af tider:");
            for (int i = 0; i < currentCustomer.Times.Count; i++) {
                Console.WriteLine(i + 1 + ": " + currentCustomer.Times[i].ToString());
            }
            Console.WriteLine();
        }

        //TEMP
        public void ChooseDate(Customer thisCustomer) {
            Console.Clear();
            Console.WriteLine("Hvornår vil du klippes");
            Console.WriteLine("Hvilket år");
            string year = Console.ReadLine();
            Console.Clear();

            Console.WriteLine("Hvilken måned i tal");
            string month = Console.ReadLine();
            Console.Clear();

            Console.WriteLine("Hvilken dag i tal");
            string day = Console.ReadLine();
            Console.Clear();

            Console.WriteLine("Hvilken tid");
            string hour = Console.ReadLine();
            Console.Write(':');
            string minute = Console.ReadLine();
            Console.Clear();
            thisCustomer.BookATime(day, month, year, hour, minute, "00");

            AvailableTimes tempday;
            if ((availableDates.Find(x => x.Date.ToString() == day + "-" + month + "-" + year + " " + "00:00:00")) == null) {
                tempday = new AvailableTimes(day + "-" + month + "-" + year + " " + "00:00:00", thisCustomer.Times.Last().DayOfWeek.ToString());
                tempday.Init();
                availableDates.Add(tempday);
                tempday.BookTime(hour + ":" + minute, 60);
            } else {
                availableDates.Find(x => x.Date.ToString() == day + "-" + month + "-" + year + " " + "00:00:00").BookTime(hour + ":" + minute, 60);
            }

            //show booked date
            int lastTime = thisCustomer.Times.Count() - 1;
            string timeString = thisCustomer.Times[lastTime].ToString();
            Console.WriteLine("Du har booket en tid den:");
            Console.WriteLine(timeString);
            Console.WriteLine();
        }
    }
}