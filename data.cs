using System;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json.Linq;

namespace ConsumeAPIData
{
    class Data
    {
        public static SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-72DHH9G\SQLEXPRESS; Initial Catalog=domainInformation; Integrated Security=True");
        public static SqlCommand command;
        /// <summary>
        /// Integer used to count the total amount of rows added to the database
        /// </summary>
        public static int insertCount = 0;

        /// <summary>
        /// Boolean used to determine if data has been collected,
        /// True if data has been collected, false otherwise
        /// </summary>
        public static bool gotInfo = false;

        /// <summary>
        /// if all domain information is valid it will be entered to database
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public static void validateInfo(string type, JObject data)//type will be used to properly access data 
        {

            if (type == "reg")
            {
                regInfo RegInfo = Data.GetRegInfo(data);
                try 
                { 
                    RegInfo.regCity = data["WhoisRecord"]["registrant"]["city"].ToString(); 
                }
                catch (Exception)
                {
                    try
                    {
                        RegInfo.regCity = data["WhoisRecord"]["registryData"]["registrant"]["city"].ToString();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("No city data has been provided...");
                        RegInfo.regCity = "NoCity";
                    }
                }
                Console.WriteLine("\nREGISTRAR CONTACT-----\nName: "+RegInfo.regName+"\n");
                Data.InsertSql(domain.Url, domain.createDate, RegInfo.regName,
                      RegInfo.regState, RegInfo.regEmail, RegInfo.regTelephone, RegInfo.regStreet, RegInfo.regCity);
                    
                
            }
            else if(type == "admin")
            {
                adminInfo AdminInfo = Data.GetAdminInfo(data);
                try
                {
                    AdminInfo.adminCity = data["WhoisRecord"]["administrativeContact"]["city"].ToString();
                }
                catch (Exception)
                {
                    try
                    {
                        AdminInfo.adminCity = data["WhoisRecord"]["registryData"]["administrativeContact"]["city"].ToString();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("No city data has been provided...");
                        AdminInfo.adminCity = "NoCity";
                    }
                }

                Console.WriteLine("\nADMIN CONTACT-----\nName: "+AdminInfo.adminName+"\n");
                Data.InsertSql(domain.Url, domain.createDate, AdminInfo.adminName,
                      AdminInfo.adminState, AdminInfo.adminEmail, AdminInfo.adminTelephone, AdminInfo.adminStreet, AdminInfo.adminCity);
            }
            else
            {
                techInfo techInfo = Data.getTechInfo(data);
                try
                {
                    techInfo.techCity = data["WhoisRecord"]["technicalContact"]["city"].ToString();
                }
                catch (Exception)
                {
                    try
                    {
                        techInfo.techCity = data["WhoisRecord"]["registryData"]["technicalContact"]["city"].ToString();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("No city data has been provided...");
                        techInfo.techCity = "NoCity";
                    }
                }
                Console.WriteLine("\nTECHNICAL CONTACT-----\nName: "+techInfo.techName+"\n");
                Data.InsertSql(domain.Url, domain.createDate, techInfo.techName,
                      techInfo.techState, techInfo.techEmail, techInfo.techTelephone , techInfo.techStreet, techInfo.techCity);
            }
        }

        /// <summary>
        /// Function used to connect to SQL DB and insert information
        /// </summary>
        /// <param name="url"></param>
        /// <param name="createdate"></param>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <param name="email"></param>
        /// <param name="phone"></param>
        /// <param name="street"></param>
        /// <param name="city"></param>
        public static void InsertSql(string url, string createdate, string name, string state,
            string email, string phone, string street, string city)
        {
            //open database connection
            con.Open();
            if (con.State == ConnectionState.Open)
            {
                Console.WriteLine("Connection to database has been established...\n");

                if (state == "TX" || state == "Texas")
                {
                    Console.WriteLine("Inserting data to sql table...");
                    command = new SqlCommand("use domainInformation\ninsert into domainContactInfo values(" +
                        "@url, @createdate, @street, @city, @name, @state, @email, @phone)",
                        con);
                    command.Parameters.Add("@url", SqlDbType.VarChar).Value = url;
                    command.Parameters.Add("@createdate", SqlDbType.VarChar).Value = createdate;
                    command.Parameters.Add("@street", SqlDbType.VarChar).Value = street;
                    command.Parameters.Add("@city", SqlDbType.VarChar).Value = city;
                    command.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
                    command.Parameters.Add("@state", SqlDbType.VarChar).Value = state;
                    command.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
                    command.Parameters.Add("@phone", SqlDbType.VarChar).Value = phone;

                    try
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Data succesfully written to database!");
                        insertCount++;
                        Data.gotInfo = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Data has not been written to the database\n" + e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("State is not Texas so data will not be written.");
                }
            }

            else
            {
                Console.WriteLine("Connection to database could not be established");
            }
            con.Close();
        }

        public static techInfo getTechInfo(JObject data)
        {
            techInfo tech = new techInfo();
            try
            {
                tech.techName = data["WhoisRecord"]["technicalContact"]["name"].ToString();
                tech.techState = data["WhoisRecord"]["technicalContact"]["state"].ToString();
                tech.techEmail = data["WhoisRecord"]["technicalContact"]["email"].ToString();
                tech.techTelephone = data["WhoisRecord"]["technicalContact"]["telephone"].ToString();
                tech.techStreet = data["WhoisRecord"]["technicalContact"]["street1"].ToString();
                tech.techCity = data["WhoisRecord"]["technicalContact"]["city"].ToString();
                return tech;
            }
            catch (Exception)
            {
                try
                {
                    tech.techName = data["WhoisRecord"]["registryData"]["technicalContact"]["name"].ToString();
                    tech.techState = data["WhoisRecord"]["registryData"]["technicalContact"]["state"].ToString();
                    tech.techEmail = data["WhoisRecord"]["registryData"]["technicalContact"]["email"].ToString();
                    tech.techTelephone = data["WhoisRecord"]["registryData"]["technicalContact"]["telephone"].ToString();
                    tech.techStreet = data["WhoisRecord"]["registryData"]["technicalContact"]["street1"].ToString();
                    tech.techCity = data["WhoisRecord"]["registryData"]["technicalContact"]["city"].ToString();
                    return tech;
                }
                catch
                {
                    Console.WriteLine("\nThere is insufficient technicalContact information...\n");
                    Data.gotInfo = false;
                    return tech;
                }
            }
        }

        public static adminInfo GetAdminInfo(JObject data)
        {
            adminInfo admin = new adminInfo();
            try
            {
                admin.adminName = data["WhoisRecord"]["administrativeContact"]["name"].ToString();
                admin.adminState = data["WhoisRecord"]["administrativeContact"]["state"].ToString();
                admin.adminEmail = data["WhoisRecord"]["administrativeContact"]["email"].ToString();
                admin.adminTelephone = data["WhoisRecord"]["administrativeContact"]["telephone"].ToString();
                admin.adminStreet = data["WhoisRecord"]["administrativeContact"]["street1"].ToString();
                admin.adminCity = data["WhoisRecord"]["administrativeContact"]["city"].ToString();
                return admin;
            }
            catch (Exception)
            {
                try
                {
                    admin.adminName = data["WhoisRecord"]["registryData"]["administrativeContact"]["name"].ToString();
                    admin.adminState = data["WhoisRecord"]["registryData"]["administrativeContact"]["state"].ToString();
                    admin.adminEmail = data["WhoisRecord"]["registryData"]["administrativeContact"]["email"].ToString();
                    admin.adminTelephone = data["WhoisRecord"]["registryData"]["administrativeContact"]["telephone"].ToString();
                    admin.adminStreet = data["WhoisRecord"]["registryData"]["administrativeContact"]["street1"].ToString();
                    admin.adminCity = data["WhoisRecord"]["registryData"]["administrativeContact"]["city"].ToString();
                    return admin;
                }
                catch
                {
                    Console.WriteLine("\nThere is insufficient administrativeContact information...\n");
                    Data.gotInfo = false;
                    return admin;
                }
            }
        }

        public static regInfo GetRegInfo(JObject data)
        {
            regInfo registrar = new regInfo();
            try
            { 
                registrar.regName = data["WhoisRecord"]["registrant"]["name"].ToString();
                registrar.regState = data["WhoisRecord"]["registrant"]["state"].ToString();
                registrar.regEmail = data["WhoisRecord"]["registrant"]["email"].ToString();
                registrar.regTelephone = data["WhoisRecord"]["registrant"]["telephone"].ToString();
                registrar.regStreet = data["WhoisRecord"]["registrant"]["street1"].ToString();
                
                return registrar;
            }
            catch (Exception)
            {
                try
                {
                    registrar.regName = data["WhoisRecord"]["registryData"]["registrant"]["name"].ToString();
                    registrar.regState = data["WhoisRecord"]["registryData"]["registrant"]["state"].ToString();
                    registrar.regEmail = data["WhoisRecord"]["registryData"]["registrant"]["email"].ToString();
                    registrar.regTelephone = data["WhoisRecord"]["registryData"]["registrant"]["telephone"].ToString();
                    registrar.regStreet = data["WhoisRecord"]["registryData"]["registrant"]["street1"].ToString();
                    
                    return registrar;
                }
                catch(Exception)
                {
                    Console.WriteLine("\nThere is insufficient registrantContact information...\n");
                    Data.gotInfo = false;
                    return registrar;
                }
            }
        }

    }
}


