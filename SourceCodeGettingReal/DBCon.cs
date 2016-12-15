using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SourceCodeGettingReal {
    class DBCon {
        private static string connectioString = "Server=ealdb1.eal.local; Database=ejl48_db; User Id=ejl48_usr; Password=Baz1nga48;";

        public void spGetAllCustomers() {
            using (SqlConnection con = new SqlConnection(connectioString)) {
                try {
                    con.Open();

                    SqlCommand cmd2 = new SqlCommand("spGetAllCustomers", con);
                    cmd2.CommandType = CommandType.StoredProcedure;

                    SqlDataReader reader = cmd2.ExecuteReader();

                    List<DateTime> tempTimes = new List<DateTime>();

                    if (reader.HasRows) {
                        string firstName = null;
                        string lastName = null;
                        int phone = 0;
                        while (reader.Read()) {
                            firstName = reader["FirstName"].ToString().Trim();
                            lastName = reader["LastName"].ToString().Trim();
                            phone = Convert.ToInt32(reader["Phone"].ToString().Trim());
                            UserFunctions.customers.Add(new Customer(firstName, lastName, phone));
                        }
                        if (reader.NextResult()) {
                            while (reader.Read()) {
                                DateTime datevalue;
                                DateTime.TryParse(reader["BookingDateTime"].ToString(), out datevalue);
                                phone = Convert.ToInt32(reader["BookingId"].ToString().Trim());
                                tempTimes.Add(datevalue);
                                Menu.hairdressers[0].Times.Add(datevalue);
                                UserFunctions.FindCustomerByPhone(phone).Times.Add(datevalue);

                                AvailableTimes tempday;
                                if ((UserFunctions.availableDates.Find(x => x.Date.Contains(datevalue.Date.ToString()))) == null) {
                                    tempday = new AvailableTimes(datevalue.Date.ToString(), datevalue.DayOfWeek.ToString());
                                    tempday.Init();
                                    UserFunctions.availableDates.Add(tempday);
                                    tempday.BookTime(datevalue.TimeOfDay.ToString(), 60);
                                } else {
                                    UserFunctions.availableDates.Find(x => x.Date.Contains(datevalue.Date.ToString())).BookTime(datevalue.TimeOfDay.ToString(), 60);
                                }

                            }
                        }
                    }
                    con.Close();
                } catch (SqlException e) {
                    Console.WriteLine("Exception: " + e.Message);
                    Console.WriteLine();
                }
            }
        }

        public void DatabaseUpdate() {
            using (SqlConnection con = new SqlConnection(connectioString)) {
                try {
                    con.Open();

                    for (int i = UserFunctions.listStartLenght; i < UserFunctions.customers.Count; i++) {
                        SqlCommand cmd1 = new SqlCommand("spInsertCustomer", con);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.Add(new SqlParameter("FirstName", UserFunctions.customers[i].Name));
                        cmd1.Parameters.Add(new SqlParameter("LastName", UserFunctions.customers[i].LastName));
                        cmd1.Parameters.Add(new SqlParameter("Phone", UserFunctions.customers[i].Phone));
                        cmd1.ExecuteNonQuery();

                        if (UserFunctions.customers[i].Times.Count > 0) {
                            for (int j = 0; j < UserFunctions.customers[i].Times.Count; j++) {
                                SqlCommand cmd2 = new SqlCommand("spInsertBooking", con);
                                cmd2.CommandType = CommandType.StoredProcedure;
                                cmd2.Parameters.Add(new SqlParameter("Phone", UserFunctions.customers[i].Phone));
                                cmd2.Parameters.Add(new SqlParameter("Booking", UserFunctions.customers[i].Times[j]));
                                cmd2.ExecuteNonQuery();
                            }
                        }
                    }
                    con.Close();

                } catch (SqlException e) {
                    Console.WriteLine("Exception: " + e.Message);
                    Console.WriteLine();
                }
            }
        }
    }
}
