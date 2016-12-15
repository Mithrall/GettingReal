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

        public Customer DoesUserExist() {
            Customer currentCustomer = null;
            int phone;
            bool canConvert;
            string currentCustomerPhone = "";
            Console.Clear();
            Console.WriteLine("Indtast telefonnummer");

            while (currentCustomer == null) {
                currentCustomerPhone = Console.ReadLine();
                if (currentCustomerPhone == "exit") {
                    Console.Clear();
                    customer = null;
                    return customer;
                }

                canConvert = Int32.TryParse(currentCustomerPhone, out phone);
                if (currentCustomerPhone.Length == 8 && canConvert == true && !currentCustomerPhone.Contains(' ')) {
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
                                RegisterUser(phone);
                                break;

                            case "nej":
                                return DoesUserExist();

                            case "exit":
                                customer = null;
                                Console.Clear();
                                return customer;

                            default:
                                Console.WriteLine("Skriv enten 'ja' eller 'nej'");
                                break;

                        }
                        currentCustomer = FindCustomerByPhone(phone);
                    }
                } else {
                    Console.Clear();
                    Console.WriteLine("Telefonnummer skal indeholde 8 tal og kun tal");
                }
            }
            return currentCustomer;
        }

        public void RegisterUser(int phone = 0) {
            Console.Clear();
            Console.WriteLine("Fornavn på bruger");
            customer = new Customer();
            do {
                customer.Name = Console.ReadLine();
                Console.Clear();
                Console.WriteLine("Fornavn på bruger, må ikke indholde tal eller være blankt");
                if (customer.Name == "exit") {
                    Console.Clear();
                    customer = null;
                    return;
                }
            } while (customer.Name == "" || customer.Name.Any(Char.IsDigit) || customer.Name.Contains(' '));

            Console.Clear();
            Console.WriteLine("Efternavn på bruger");
            do {
                customer.LastName = Console.ReadLine();
                Console.Clear();
                Console.WriteLine("Efternavn på bruger, må ikke indholde tal eller være blankt");
                if (customer.LastName == "exit") {
                    Console.Clear();
                    customer = null;
                    return;
                }
            } while (customer.LastName == "" || customer.LastName.Any(Char.IsDigit) || customer.LastName.Contains(' '));

            customer.Phone = phone;
            Console.Clear();
            Console.WriteLine("Bruger oprettet: ");
            Console.WriteLine("Navn: " + customer.Name + " " + customer.LastName + " - tlf: " + phone);
            customers.Add(customer);
        }

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
    }
}