﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ProjectDB.Models
{
    public class Methods
    {
        public Methods()
        {
        }

        public IConfigurationRoot GetConnection()

        {

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appSettings.json").Build();

            return builder;

        }

        public bool VerifyAccount(out string errormsg, Person person)
        {


            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            // sqlstring och lägg till en user i databasen
            String sqlstring = "SELECT Pe_Losenord FROM Tbl_Person WHERE Pe_Anvandarnamn = @user";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("user", System.Data.SqlDbType.NVarChar, 30).Value = person.Username;

            SqlDataReader reader = null;

            string password = "";

            errormsg = "";

            try
            {
                dbConnection.Open();

                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    password = reader["Pe_Losenord"].ToString();
                    
                }
                reader.Close();



                if (person.Password == password)
                {

                    return true;
                }
                else
                {
                    errormsg = "Fel lösenord";
                    return false;
                }

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return false;
            }
            finally
            {
                dbConnection.Close();
            }
        }





        public bool CreateAccount(out string errormsg, Person person)
        {


            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            // sqlstring och lägg till en user i databasen
            String sqlstring = "INSERT INTO [Tbl_Person]([Pe_Anvandarnamn], [Pe_Losenord], [Pe_Utbildning], [Pe_Examensdatum]) VALUES (@user, @password, @education, @datetime)";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("user", System.Data.SqlDbType.NVarChar, 30).Value = person.Username;
            dbCommand.Parameters.Add("password", System.Data.SqlDbType.NVarChar, 30).Value = person.Password;
            dbCommand.Parameters.Add("education", System.Data.SqlDbType.NVarChar, 50).Value = person.Education;
            dbCommand.Parameters.Add("datetime", System.Data.SqlDbType.DateTime).Value = person.ExamDate;

            errormsg = "";
            bool success = false;

            try
            {
                dbConnection.Open();
                int i = dbCommand.ExecuteNonQuery();
                if(i == 1) {
                    success = true;
                    return success;
                }

                else
                {
                    errormsg = "Det går inte att lägga till en användare";
                    return false;
                }
               

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return false;
            }
            finally
            {
                dbConnection.Close();
            }
        }




        public List<Course> GetFailed(out string errormsg, string username)
        {


            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            // sqlstring och lägg till en user i databasen
            String sqlstring = "SELECT Ku_Namn, Ku_HP FROM Tbl_KursPerson INNER JOIN Tbl_Kurs ON Tbl_KursPerson.KP_Kurs = Tbl_Kurs.Ku_ID INNER JOIN Tbl_Person ON Tbl_KursPerson.KP_Person = Tbl_Person.Pe_ID INNER JOIN Tbl_Status ON Tbl_KursPerson.KP_Status = Tbl_Status.St_ID WHERE Pe_Anvandarnamn = @user AND St_Kursstatus = 'Oavslutad'; ";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("user", System.Data.SqlDbType.NVarChar, 30).Value = username;

            SqlDataReader reader = null;

            List<Course> courses = new List<Course>();

            errormsg = "";

            try
            {
                dbConnection.Open();

                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    Course course = new Course();
                    course.Name = reader["Ku_Namn"].ToString();
                    course.HP = Convert.ToDouble(reader["Ku_HP"]);

                    courses.Add(course);

                }
                reader.Close();
                return courses;

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public List<Course> GetOngoing(out string errormsg, string username)
        {


            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            // sqlstring och lägg till en user i databasen
            String sqlstring = "SELECT Ku_Namn, Ku_HP FROM Tbl_KursPerson INNER JOIN Tbl_Kurs ON Tbl_KursPerson.KP_Kurs = Tbl_Kurs.Ku_ID INNER JOIN Tbl_Person ON Tbl_KursPerson.KP_Person = Tbl_Person.Pe_ID INNER JOIN Tbl_Status ON Tbl_KursPerson.KP_Status = Tbl_Status.St_ID WHERE Pe_Anvandarnamn = @user AND St_Kursstatus = 'Pågående'; ";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("user", System.Data.SqlDbType.NVarChar, 30).Value = username;

            SqlDataReader reader = null;

            List<Course> courses = new List<Course>();

            errormsg = "";

            try
            {
                dbConnection.Open();

                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    Course course = new Course();
                    course.Name = reader["Ku_Namn"].ToString();
                    course.HP = Convert.ToDouble(reader["Ku_HP"]);

                    courses.Add(course);

                }
                reader.Close();
                return courses;

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                dbConnection.Close();
            }
        }



        public List<Grade> Grades(out string errormsg, string username)
        {

            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            // sqlstring och lägg till en user i databasen
            String sqlstring = "SELECT Be_Kursbetyg, COUNT(Tbl_KursPerson.KP_Betyg) AS Frequency FROM Tbl_KursPerson INNER JOIN Tbl_Betyg ON Tbl_KursPerson.KP_Betyg = Tbl_Betyg.Be_ID INNER JOIN Tbl_Person ON Tbl_KursPerson.KP_Person = Tbl_Person.Pe_ID WHERE Tbl_Person.Pe_Anvandarnamn = @user GROUP BY Tbl_Betyg.Be_Kursbetyg ORDER BY COUNT(Tbl_KursPerson.KP_Betyg) DESC ";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            SqlDataReader reader = null;

            dbCommand.Parameters.Add("user", System.Data.SqlDbType.NVarChar, 30).Value = username;

            List<Grade> stats = new List<Grade>();
            errormsg = "";



            try
            {
                dbConnection.Open();

                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {

                    Grade grade = new Grade();

                    if (reader["Be_Kursbetyg"].ToString() != "NAN")
                    {
                        grade.GradeType = reader["Be_Kursbetyg"].ToString();
                        grade.Frequency = Convert.ToInt16(reader["Frequency"]);
                        stats.Add(grade);
                    }

                }
                reader.Close();
                return stats;

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                dbConnection.Close();
            }

        }

        public List<Course> GetCourses(out string errormsg, string username)
        {


            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            // sqlstring och lägg till en user i databasen
            String sqlstring = "SELECT Ku_ID, Ku_Namn, Ku_HP, St_Kursstatus FROM Tbl_KursPerson INNER JOIN Tbl_Kurs ON Tbl_KursPerson.KP_Kurs = Tbl_Kurs.Ku_ID INNER JOIN Tbl_Person ON Tbl_KursPerson.KP_Person = Tbl_Person.Pe_ID INNER JOIN Tbl_Status ON Tbl_KursPerson.KP_Status = Tbl_Status.St_ID WHERE Pe_Anvandarnamn = @user ; ";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("user", System.Data.SqlDbType.NVarChar, 30).Value = username;

            SqlDataReader reader = null;

            List<Course> courses = new List<Course>();

            errormsg = "";

            try
            {
                dbConnection.Open();

                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    Course course = new Course();
                    course.ID = Convert.ToInt16(reader["Ku_ID"]);
                    course.Name = reader["Ku_Namn"].ToString();
                    course.HP = Convert.ToDouble(reader["Ku_HP"]);
                    course.Status = reader["St_Kursstatus"].ToString();

                    courses.Add(course);

                }
                reader.Close();
                return courses;

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                dbConnection.Close();
            }



        }


        public Course GetCourse(out string errormsg, string username, int selected)
        {


            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            // sqlstring och lägg till en user i databasen
            String sqlstring = "SELECT Ku_ID, Ku_Namn, Ku_HP, In_Institutionsnamn, St_Kursstatus, Be_Kursbetyg FROM Tbl_KursPerson INNER JOIN Tbl_Kurs ON Tbl_KursPerson.KP_Kurs = Tbl_Kurs.Ku_ID INNER JOIN Tbl_Person ON Tbl_KursPerson.KP_Person = Tbl_Person.Pe_ID INNER JOIN Tbl_Status ON Tbl_KursPerson.KP_Status = Tbl_Status.St_ID INNER JOIN Tbl_Betyg ON Tbl_KursPerson.KP_Betyg = Tbl_Betyg.Be_ID INNER JOIN Tbl_Institution ON Tbl_Kurs.Ku_Institution = Tbl_Institution.In_ID WHERE Pe_Anvandarnamn = @user AND Ku_ID = @sel ";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            SqlDataReader reader = null;

            dbCommand.Parameters.Add("user", System.Data.SqlDbType.NVarChar, 30).Value = username;
            dbCommand.Parameters.Add("sel", System.Data.SqlDbType.Int).Value = selected;


            Course course = new Course();

            errormsg = "";

            try
            {
                dbConnection.Open();

                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    
                    course.ID = Convert.ToInt16(reader["Ku_ID"]);
                    course.Name = reader["Ku_Namn"].ToString();
                    course.HP = Convert.ToDouble(reader["Ku_HP"]);
                    course.Institution = reader["In_Institutionsnamn"].ToString();
                    course.Status = reader["St_Kursstatus"].ToString();
                    course.Betyg = reader["Be_Kursbetyg"].ToString();

                }
                reader.Close();

                return course;

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                dbConnection.Close();
            }
        }





        public List<Status> GetStatuses(out string errormsg)
        {


            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            // sqlstring och lägg till en user i databasen
            String sqlstring = "SELECT * FROM Tbl_Status";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            SqlDataReader reader = null;

            List<Status> statuses = new List<Status>();

            errormsg = "";

            try
            {
                dbConnection.Open();

                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    Status status = new Status();
                    status.StatusType = reader["St_Kursstatus"].ToString();
                    status.Id = Convert.ToInt16(reader["St_ID"]);

                    statuses.Add(status);

                }
                reader.Close();
                return statuses;

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                dbConnection.Close();
            }
        }




        public Status GetStatus(out string errormsg, string name)
        {


            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            // sqlstring och lägg till en user i databasen
            String sqlstring = "SELECT * FROM Tbl_Status WHERE St_Kursstatus = @name ";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            SqlDataReader reader = null;
            dbCommand.Parameters.Add("name", System.Data.SqlDbType.NVarChar, 30).Value = name;

            Status status = new Status();

            errormsg = "";

            try
            {
                dbConnection.Open();

                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
               
                    status.StatusType = reader["St_Kursstatus"].ToString();
                    status.Id = Convert.ToInt16(reader["St_ID"]);

                }
                reader.Close();
                return status;

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public List<Grade> GetGrades(out string errormsg)
        {


            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            // sqlstring och lägg till en user i databasen
            String sqlstring = "SELECT * FROM Tbl_Betyg";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            SqlDataReader reader = null;

            List<Grade> grades = new List<Grade>();

            errormsg = "";

            try
            {
                dbConnection.Open();

                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    Grade grade = new Grade();
                    grade.GradeType = reader["Be_Kursbetyg"].ToString();
                    grade.Id = Convert.ToInt16(reader["Be_ID"]);

                    grades.Add(grade);

                }
                reader.Close();
                return grades;

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public Grade GetGrade(out string errormsg, string name)
        {


            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            // sqlstring och lägg till en user i databasen
            String sqlstring = "SELECT * FROM Tbl_Betyg WHERE Be_Kursbetyg = @name ";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            SqlDataReader reader = null;
            dbCommand.Parameters.Add("name", System.Data.SqlDbType.NVarChar, 30).Value = name;

            Grade grade = new Grade();

            errormsg = "";

            try
            {
                dbConnection.Open();

                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {

                    grade.GradeType = reader["Be_Kursbetyg"].ToString();
                    grade.Id = Convert.ToInt16(reader["Be_ID"]);

                }
                reader.Close();
                return grade;

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                dbConnection.Close();
            }
        }






        public bool UpdateCourse(out string errormsg, Course course, string user, string status, string betyg)
        {


            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            // sqlstring och lägg till en user i databasen
            String sqlstring = "UPDATE Tbl_KursPerson SET KP_Status = @status , KP_Betyg = @betyg FROM Tbl_KursPerson INNER JOIN Tbl_Person ON Tbl_KursPerson.KP_Person = Tbl_Person.Pe_ID WHERE KP_Kurs = @kurs AND Tbl_Person.Pe_Anvandarnamn = @user ";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("status", System.Data.SqlDbType.Int).Value = status;
            dbCommand.Parameters.Add("betyg", System.Data.SqlDbType.Int).Value = betyg;
            dbCommand.Parameters.Add("kurs", System.Data.SqlDbType.Int).Value = course.ID;
            dbCommand.Parameters.Add("user", System.Data.SqlDbType.NVarChar, 50).Value = user;

            errormsg = "";
            bool success = false;

            try
            {
                dbConnection.Open();
                int i = dbCommand.ExecuteNonQuery();
                if (i == 1)
                {
                    success = true;
                    return success;
                }

                else
                {
                    errormsg = "Det går inte att uppdatera en kurs";
                    return false;
                }


            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return false;
            }
            finally
            {
                dbConnection.Close();
            }
        }





        // INSÄTTNING PERSON
        public int CreateCourse(out string errormsg, Course course, int instID)
        {
            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            String sqlstring = "INSERT INTO Tbl_Kurs (Ku_HP, Ku_Namn, Ku_Institution) VALUES(@HP, @Namn, @Institution) SELECT SCOPE_IDENTITY() AS ID";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            SqlDataReader reader = null;

            dbCommand.Parameters.Add("HP", System.Data.SqlDbType.Float).Value = course.HP;
            dbCommand.Parameters.Add("Namn", System.Data.SqlDbType.NVarChar, 50).Value = course.Name;
            dbCommand.Parameters.Add("Institution", System.Data.SqlDbType.Int).Value = instID;

            errormsg = "";
          

            try
            {
                int courseID = 0;

                dbConnection.Open();

                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                
                    courseID = Convert.ToInt16(reader["ID"]);

                }
                reader.Close();
                return courseID;

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }
        }





        public bool AddCourse(out string errormsg, Course course, int instID, string user)
        {

            int courseID = CreateCourse(out string errormsg2, course, instID);

            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            String sqlstring = "INSERT INTO Tbl_KursPerson VALUES((SELECT Pe_ID FROM Tbl_Person WHERE Pe_Anvandarnamn = @user ), @courseID , (SELECT St_ID FROM Tbl_Status WHERE St_Kursstatus = 'Kommande'), (SELECT Be_ID FROM Tbl_Betyg WHERE Be_Kursbetyg = 'NAN'))";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("user", System.Data.SqlDbType.NVarChar, 50).Value = user;
            dbCommand.Parameters.Add("courseID", System.Data.SqlDbType.Int).Value = courseID;

            errormsg = "";
            bool success = false;

            try
            {
                dbConnection.Open();
                int i = dbCommand.ExecuteNonQuery();
                if (i == 1)
                {
                    success = true;
                    return success;
                }

                else
                {
                    errormsg = "Det går inte att uppdatera en kurs";
                    return false;
                }


            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return false;
            }
            finally
            {
                dbConnection.Close();
            }
        }







        public List<Institution> GetInstitutions(out string errormsg)
        {


            SqlConnection dbConnection = new SqlConnection(GetConnection().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);

            String sqlstring = "SELECT * FROM Tbl_Institution";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            SqlDataReader reader = null;

            List<Institution> institutions = new List<Institution>();
            errormsg = "";

            try
            {
                dbConnection.Open();

                reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    Institution institution = new Institution();
                    institution.name = reader["In_Institutionsnamn"].ToString();
                    institution.ID = Convert.ToInt16(reader["In_ID"]);

                    institutions.Add(institution);

                }
                reader.Close();
                return institutions;

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                dbConnection.Close();
            }
        }


    }
}
