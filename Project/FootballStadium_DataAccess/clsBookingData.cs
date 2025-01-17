﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballStadium_DataAccess
{
    public class clsBookingData
    {
        public static int AddNewBooking(int TeamID, DateTime BookingDate, TimeSpan FromTime, TimeSpan ToTime, string DayName, int CreatedByUserID)
        {
            int BookingID = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"INSERT INTO Bookings (TeamID, BookingDate, FromTime, ToTime, DayName, CreatedByUserID)
                             VALUES (@TeamID, @BookingDate, @FromTime, @ToTime, @DayName, @CreatedByUserID)
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@TeamID", TeamID);
            command.Parameters.AddWithValue("@BookingDate", BookingDate);
            command.Parameters.AddWithValue("@FromTime", FromTime);
            command.Parameters.AddWithValue("@ToTime", ToTime);
            command.Parameters.AddWithValue("@DayName", DayName);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    BookingID = insertedID;
                }
            }

            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);

            }

            finally
            {
                connection.Close();
            }

            return BookingID;
        }
        public static DataTable GetAllBookings()
        {

            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"select BookingID, Bookings.TeamID, Teams.TeamName, Teams.PlayersCount, BookingDate, FromTime, ToTime  from Bookings
                             Inner join teams on Teams.TeamID = bookings.TeamID";


            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                }

                reader.Close();


            }

            catch (Exception ex)
            {
                // Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dt;

        }
        public static DataTable GetNewBookings()
        {

            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"select BookingID, Bookings.TeamID, Teams.TeamName, BookingDate, DayName, FromTime, ToTime  from Bookings
                             Inner join teams on Teams.TeamID = bookings.TeamID 
                             WHERE BookingDate >= Convert(date, getdate())";


            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                }

                reader.Close();


            }

            catch (Exception ex)
            {
                // Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dt;

        }
        public static DataTable GetOldBookings()
        {

            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"select BookingID, Bookings.TeamID, Teams.TeamName, BookingDate, DayName, FromTime, ToTime  from Bookings
                             Inner join teams on Teams.TeamID = bookings.TeamID
                             WHERE BookingDate < Convert(date, getdate())";
           

            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                }

                reader.Close();


            }

            catch (Exception ex)
            {
                // Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dt;

        }
        public static bool IsTimeBooked(DateTime BookingDate, TimeSpan FromTime, TimeSpan ToTime)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"SELECT FOUND = 1 FROM Bookings WHERE BookingDate = @BookingDate 
                             AND 
                             ((@FromTime between FromTime and ToTime) OR (@ToTime between FromTime and ToTime) OR (@ToTime >= ToTime and @FromTime <= FromTime));";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@BookingDate", BookingDate);
            command.Parameters.AddWithValue("@FromTime", FromTime);
            command.Parameters.AddWithValue("@ToTime", ToTime);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                isFound = reader.HasRows;

                reader.Close();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }
            finally
            {
                connection.Close();
            }

            return isFound;
        }
        public static bool IsTimeBooked(string DayName, TimeSpan FromTime, TimeSpan ToTime)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"SELECT FOUND = 1 FROM Bookings WHERE DayName = @DayName 
                             AND 
                             ((@FromTime between FromTime and ToTime) OR (@ToTime between FromTime and ToTime) OR (@ToTime >= ToTime and @FromTime <= FromTime))
                             AND    
                             BookingDate >= Convert(date, getdate())";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@DayName", DayName);
            command.Parameters.AddWithValue("@FromTime", FromTime);
            command.Parameters.AddWithValue("@ToTime", ToTime);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                isFound = reader.HasRows;

                reader.Close();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }
            finally
            {
                connection.Close();
            }

            return isFound;
        }
        public static bool DeleteBooking(int BookingID)
        {

            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Delete Bookings 
                                where BookingID = @BookingID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@BookingID", BookingID);

            try
            {
                connection.Open();

                rowsAffected = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                // Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {

                connection.Close();

            }

            return (rowsAffected > 0);

        }
    }
}
