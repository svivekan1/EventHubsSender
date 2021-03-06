﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.ServiceBus.Messaging;
using System.Data;
using System.Data.SqlClient;

namespace EventHubsSender
{
    class Program
    {
        static string eventHubName = "testsvivekanhub";
        static string connectionString = "Endpoint=sb://testsvivekanhub-ns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=LmeY4Ck7OGJ4cVjt1rc1a5MrZHlYBEyoLgqEY4NcqEo=";

        static void Main(string[] args)
        {
            Console.WriteLine("Press Ctrl-C to stop the sender process");
            Console.WriteLine("Press Enter to start now");
            Console.ReadLine();
            SendingRandomMessages();
        }

        static void SendingRandomMessages()
        {
            //Variables
            var sqlConnectionString = "Data Source=tcp:asos-an-ods-generic-live-eun.database.windows.net,1433;Initial Catalog=AzureUsageOds;Integrated Security=False;User ID=AnalyticsODSLogin@asos-an-ods-generic-live-eun;Password=pruZup2chaDras4a;Connect Timeout=60;Encrypt=True";
            var sqlCommandString = "SELECT TOP 10 * FROM [staging].[AzureUsageDetail]";


            // declare the SqlDataReader, which is used in
            // both the try block and the finally block
            SqlDataReader rdr = null;

            // create a connection object
            SqlConnection conn = new SqlConnection(sqlConnectionString);

            // create a command object
            SqlCommand cmd = new SqlCommand(sqlCommandString, conn);

            try
            {
                // open the connection
                conn.Open();

                // 1. get an instance of the SqlDataReader
                rdr = cmd.ExecuteReader();

                // 2. print necessary columns of each record
                while (rdr.Read())
                {
                    var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, eventHubName);

                    // get the results of each column
                    var accountOwnerID = (string)rdr["AccountOwnerID"];
                    var accountName = (string)rdr["AccountName"];

                    try
                    {
                        //var message = Guid.NewGuid().ToString();
                        var message = accountOwnerID + accountName;
                        Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, message);
                        eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(message)));
                    }
                    catch (Exception exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                        Console.ResetColor();
                    }

                    Thread.Sleep(200);

                }
            }
            finally
            {
                // 3. close the reader
                if (rdr != null)
                {
                    rdr.Close();
                }

                // close the connection
                if (conn != null)
                {
                    conn.Close();
                }
            }
        

        }
    }
}
