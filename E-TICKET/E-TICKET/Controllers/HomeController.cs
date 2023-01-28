﻿using E_TICKET.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace E_TICKET.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private bool LoginFlag;
        private static bool isLogin; // To know if user logged in or not
        private string journey_from;
        private string journey_to;
        private string departure;
        private string sit_class;
        private static List<Home> L = new List<Home>(); //Static list For Home page

        public object NetworkCredintial { get; private set; }  // TO send email

        [HttpGet]
        public ActionResult Index()
        {
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-03NNHMH\\SQLEXPRESS;Initial Catalog=RAILWAY_DB;User ID=sa;Password=123456");
            conn.Open();

            /*read all districts from database*/
            SqlCommand com = new SqlCommand("Select * from Districts", conn);
            com.ExecuteNonQuery();
            SqlDataReader sdr = com.ExecuteReader();

            List<Home> L = new List<Home>();

            while (sdr.Read())
            {
                L.Add(new Home { From = sdr["DistrictName"].ToString() });
            }
            TempData["a"] = L;
            conn.Close();

            return View();
        }

        [HttpPost]
        public ActionResult Index(Home rd)
        {
            
            journey_from = rd.From;
            journey_to = rd.To;
            departure = rd.Date;
            sit_class = rd.Sit_class;
            //ViewBag.Message = journey_from + " " + journey_to + " " + departure+" "+sit_class+" "+isLogin;
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-03NNHMH\\SQLEXPRESS;Initial Catalog=RAILWAY_DB;User ID=sa;Password=123456"); /*read all districts from database*/
            conn.Open();


            SqlCommand com = new SqlCommand("Select * from Districts", conn);
            com.ExecuteNonQuery();
            SqlDataReader sdr = com.ExecuteReader();
            while (sdr.Read())
            {
                L.Add(new Home { From = sdr["DistrictName"].ToString() });
            }
            TempData["a"] = L;
            conn.Close();
            readFor_TrainSearchResult();
            if (isLogin)
            {
                return View("TrainSearchResult");
            }
            ViewBag.Message = "Plase Login.";// login req to go next step
            return View();
        }
        public ActionResult Resister(Resister reg)
        {
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-03NNHMH\\SQLEXPRESS;Initial Catalog=RAILWAY_DB;User ID=sa;Password=123456");
            conn.Open();



            string name = reg.name;
            string mobile = reg.mobile;
            string email = reg.email;
            string pass = reg.pass;
            string nid = reg.nid;
            bool validOrNOt = true;

            //cheack if nid already in database
            //if already exist warning message will pop up
            SqlCommand com = new SqlCommand("Select * from tb_user where id = '" + mobile + "' or nid = '" + nid + "'", conn);
            com.ExecuteNonQuery();
            SqlDataReader sdr = com.ExecuteReader();

            while (sdr.Read())
            {
                validOrNOt = false;
                break;
            }
            conn.Close();
            //Have some handeling problem
            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(mobile) || String.IsNullOrEmpty(email) || String.IsNullOrEmpty(pass) || String.IsNullOrEmpty(nid)) // cheack if fild is empty 
            {
                ViewBag.Message = null;
            }
            else if (!validOrNOt)
            {
                ViewBag.Message = "User ID or NID already exist.";
            }
            else
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("Insert into tb_user(id,name,email,mobile,pass,nid) values('" + reg.mobile + "','" + reg.name + "','" + reg.email + "','" + reg.mobile + "','" + reg.pass + "','" + reg.nid + "')", conn);
                comm.ExecuteNonQuery();

                ViewBag.Message = "Successful.";
            }
            return View();
        }

        public ActionResult Login(Login lgn)
        {
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-03NNHMH\\SQLEXPRESS;Initial Catalog=RAILWAY_DB;User ID=sa;Password=123456");
            conn.Open();

            string mobile = lgn.mobile;
            string pass = lgn.pass;

            

            SqlCommand com = new SqlCommand("Select * from tb_user where mobile = '" + mobile + "' and pass = '" + pass + "'", conn);
            com.ExecuteNonQuery();
            SqlDataReader sdr = com.ExecuteReader();

            while (sdr.Read())
            {
                isLogin = true;
                LoginFlag = true;
                break;
            }
            conn.Close();

            if (LoginFlag)
            {
                ViewBag.Message = "Login Successful.Please Go Back To Home Tab.";
               

            }
           
            return View();
        }
        public ActionResult ContactUs(ContactUs ct)
        {
            string email = ct.email;
            string name = ct.name;
            string msg = ct.msg;

            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-03NNHMH\\SQLEXPRESS;Initial Catalog=RAILWAY_DB;User ID=sa;Password=123456");
            conn.Open();
            if (!string.IsNullOrEmpty(email)&&!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(msg))
            {
                SqlCommand comm = new SqlCommand("Insert into contactUs(name,email,msg) values('" + name + "','" + email + "','" + msg + "')", conn);
                comm.ExecuteNonQuery();
                //E-mail sending system
                using (MailMessage mm = new MailMessage("aust.decipher@gmail.com", ct.email))
                {
                    mm.Subject = "E-Ticket customer Service";
                    mm.Body = "Dear customer, We just got your message!!.";

                    mm.IsBodyHtml = false;

                    using(SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential cred = new NetworkCredential("aust.decipher@gmail.com", "sijhpnswfycrjsvx");
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = cred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }

                }
                ViewBag.Message = "Successful.";
            }
            conn.Close();
  

            return View();
        }

        public ActionResult TrainInformation(TrainInformation ti)
        {
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-03NNHMH\\SQLEXPRESS;Initial Catalog=RAILWAY_DB;User ID=sa;Password=123456");
            conn.Open();

            string t_name=ti.Train_Name;

            /*read all Train name from database*/
            SqlCommand com = new SqlCommand("Select * from Train", conn);
            com.ExecuteNonQuery();
            SqlDataReader sdr = com.ExecuteReader();

            List<TrainInformation> L = new List<TrainInformation>();

            while (sdr.Read())
            {
                L.Add(new TrainInformation { Train_Name = sdr["Train_Name"].ToString() });
            }
            TempData["a"] = L;
            conn.Close();


            /*for search result*/
            conn.Open();
            SqlCommand comm = new SqlCommand("Select * from Train where Train_Name = '"+t_name+"'", conn);
            comm.ExecuteNonQuery();
            SqlDataReader rd = comm.ExecuteReader();

            List<TrainInformation> LL = new List<TrainInformation>();

            while (rd.Read())
            {
                LL.Add(new TrainInformation {Train_No = (int)rd["Train_No"], Train_Name = rd["Train_Name"].ToString(), Offday = rd["Offday"].ToString(), Departure_Station = rd["Departure_Station"].ToString(),Departure_time= rd["Departure_time"].ToString(), Arrival_Station = rd["Arrival_Station"].ToString(), Arrival_Time = rd["Arrival_Time"].ToString() });
            }
            TempData["b"] = LL;

            return View();
        }

        public ActionResult TrainSearchResult(TrainSearchResult tsr)
        {
            readFor_TrainSearchResult();
            
            return View();
        }

        public void readFor_TrainSearchResult()
        {
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-03NNHMH\\SQLEXPRESS;Initial Catalog=RAILWAY_DB;User ID=sa;Password=123456");
            conn.Open();
            SqlCommand com = new SqlCommand("SELECT Train.Train_No,Coach.Seat_No,Train.Train_Name,Train.Departure_Station,Train.Arrival_Station,Train.Departure_time,Train.Arrival_Time,Coach.Coach_fare,Coach.Class,Coach.Coach_Name FROM Train INNER JOIN Coach ON Train.Train_No = Coach.Train_No and Train.Departure_Station='"+journey_from+"' and Train.Arrival_Station='"+journey_to+"' and Coach.Class = '"+sit_class+ "'", conn);
            com.ExecuteNonQuery();
            SqlDataReader sdr = com.ExecuteReader();

            List<TrainSearchResult> L = new List<TrainSearchResult>();

            while (sdr.Read())
            {
                L.Add(new TrainSearchResult { Train_No = (int)sdr["Train_No"], Seat_No = (int)sdr["Seat_No"], Train_Name = sdr["Train_Name"].ToString(), Departure_Station = sdr["Departure_Station"].ToString(), Arrival_Station = sdr["Arrival_Station"].ToString(), Departure_time = sdr["Departure_time"].ToString(), Arrival_Time = sdr["Arrival_Time"].ToString(), Coach_fare = (int)sdr["Coach_fare"], Class = sdr["Class"].ToString(), Coach_Name = sdr["Coach_Name"].ToString() });
            }
            TempData["tsr"] = L;
            conn.Close();
        }

        /*getter and setter method*/

        public bool getLoginFlag()
        {
            return isLogin;
        }

        public void setLoginFlag(bool bln)
        {
            isLogin = bln;
        }

        public string getJourneyFrom()
        {
            return journey_from;
        }

        public void setJourneyFrom(string journey_from)
        {
            this.journey_from = journey_from;
        }

        public string getJourneyTo()
        {
            return journey_to;
        }

        public void setJourneyTo(string journey_to)
        {
            this.journey_to = journey_to;
        }

        public string getDeparture()
        {
            return departure;
        }

        public void setDeparture(string departure)
        {
            this.departure = departure;
        }

        public string getSit_class()
        {
            return sit_class;
        }

        public void setSit_class(string sit_class)
        {
            this.sit_class = sit_class;
        }
        

    }
}