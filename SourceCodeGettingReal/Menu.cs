using System;
using System.Collections.Generic;

namespace SourceCodeGettingReal {
    public class Menu {
        public UserFunctions userFunctions;
        HairdresserFunctions hairDresserFunctions;
        public static List<Hairdresser> hairdressers;
        
        public void Init() {
            //temp
            hairdressers = new List<Hairdresser>();
            Hairdresser hairdresser = new Hairdresser("Louise");
            hairdressers.Add(hairdresser);
            hairDresserFunctions = new HairdresserFunctions();

            userFunctions = new UserFunctions();
            userFunctions.Init();
        }

        public void MainMenu() {
            MainMenuText();
            ConsoleKeyInfo cki;
            do {
                cki = Console.ReadKey(false);
                switch (cki.KeyChar.ToString()) {
                    case "1":
                        UserMenu();
                        MainMenu();
                        break;

                    case "2":
                        Console.Clear();
                        HairdresserMenu();
                        MainMenu();
                        break;

                    default:
                        MainMenuText();
                        WrongInput();
                        break;
                }
            } while (cki.Key != ConsoleKey.Escape);
            DBCon dbcon = new DBCon();
            dbcon.DatabaseUpdate();
            Environment.Exit(1);
        }

        public void UserMenu(Customer tempcurrentcustomer = null) {
            ConsoleKeyInfo cki;
            Customer currentCustomer;
            currentCustomer = tempcurrentcustomer;
            if (currentCustomer == null) {
                currentCustomer = userFunctions.DoesUserExist();
                Console.Clear();
            }
            if (currentCustomer == null) {
                MainMenu();
            }

            UserMenuText(currentCustomer);
            cki = Console.ReadKey(false);
            switch (cki.KeyChar.ToString()) {
                case "1":
                    Console.Clear();
                    userFunctions.ChooseDate(currentCustomer);
                    UserMenu(currentCustomer);
                    break;

                case "2":
                    Console.Clear();
                    userFunctions.ShowTimes(currentCustomer);
                    UserMenu(currentCustomer);
                    break;

                case "3":
                    Console.Clear();
                    UserMenu();
                    break;

                case "4":
                    Console.Clear();
                    MainMenu();
                    break;

                default:
                    Console.WriteLine();
                    WrongInput();
                    MainMenu();
                    break;
            }
        }

        public void HairdresserMenu() {
            ConsoleKeyInfo cki;
            HairdresserMenuText();
            cki = Console.ReadKey(false);
            switch (cki.KeyChar.ToString()) {
                case "1":
                    Console.Clear();
                    hairDresserFunctions.ListCustomers();
                    HairdresserMenu();
                    break;
                case "2":
                    ShowNextCustomer();
                    HairdresserMenu();
                    break;

                case "3":
                    ShowAvailebleTimesForToday();
                    HairdresserMenu();
                    break;

                case "5":
                    Console.Clear();
                    MainMenu();
                    break;

                default:
                    Console.WriteLine();
                    WrongInput();
                    HairdresserMenu();
                    break;
            }        
        }

        public void ShowNextCustomer() {
            Console.Clear();
            //asuming signed in as Louise
            DateTime nextTime = hairdressers.Find(x => x.Name == "Louise").NextTime();
            Customer foundCustomer = hairDresserFunctions.FindCustomerByTime(nextTime, "Louise");
            hairDresserFunctions.PrintCustomer(foundCustomer);
            Console.WriteLine();
        }

        public void ShowAvailebleTimesForToday() {
            Console.Clear();
            List<AvailableTimes> tempDates = hairDresserFunctions.getAvailableTimes();
            AvailableTimes Today = tempDates.Find(x => x.Date == DateTime.Now.Date.ToString());
            string[] tempTimes = Today.ShowTimes().Split(' ');

            if (tempTimes.Length > 1) {
                Console.WriteLine("Du har ledig tid: ");
                for (int i = 0; i < tempTimes.Length; i += 2) {
                    Console.WriteLine("Fra: " + tempTimes[i] + " Til: " + tempTimes[i + 1]);
                    if (i + 2 != tempTimes.Length) {
                        Console.WriteLine("&");
                    }
                }
            } else {
                Console.WriteLine("Du har ingen ledige tider.");
            }
            Console.WriteLine();
        }

        public void MainMenuText() {
            Console.WriteLine("MENU:");
            Console.WriteLine("tryk '1' for kunde");
            Console.WriteLine("tryk '2' for frisør");
        }

        public void UserMenuText(Customer currentCustomer) {
            Console.WriteLine("Du er logget på som: " + currentCustomer.Name + ' ' + currentCustomer.LastName);
            Console.WriteLine("Tryk '1' hvis du ønsker at oprette en ny tid");
            Console.WriteLine("Tryk '2' hvis du ønsker at se liste over dine tider");
            Console.WriteLine("Tryk '3' hvis du ønsker at logge på som en anden");
            Console.WriteLine("Tryk '4' hvis du ønsker at logge ud og vende tilbage til hovedmenu'en");
        }

        public void HairdresserMenuText() {
            Console.WriteLine("Du er logget på som: Frisør");
            Console.WriteLine("Tryk '1' hvis du ønsker at se liste over kunder");
            Console.WriteLine("Tryk '2' hvis du ønsker at se næste kunde og deres tid");
            Console.WriteLine("Tryk '3' hvis du ønsker at se om du har ledige tider idag");
            Console.WriteLine("Tryk '4' hvis du ønsker at logge på som en anden (TBD)");
            Console.WriteLine("Tryk '5' hvis du ønsker at logge ud og vende tilbage til hovedmenu'en");
        }

        public void WrongInput() {
            Console.Clear();
            Console.WriteLine("Forkert input");
            Console.WriteLine();
        }
    }
}