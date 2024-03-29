﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ticktage
{
    class Program
    {
        //static ChromeDriver driver;
        static FirefoxDriver driver;
        static System.Media.SoundPlayer player;

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("");
                Console.WriteLine("** Ticktage/Scada Chlef V1.0 **");
                Console.WriteLine("");

                var serverPath = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
                try
                {
                    player = new System.Media.SoundPlayer
                    {
                        SoundLocation = serverPath + "/wav.wav"
                    };

                    player.LoadAsync();
                }
                catch
                {
                }
                var op = new FirefoxOptions
                {
                    AcceptInsecureCertificates = true
                };

                //driver = new ChromeDriver(serverPath);
                driver = new FirefoxDriver(serverPath, op);
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl("https://support-cc.sdc.dz/front/ticket.php");

                driver.FindElement(By.XPath("//*[@id='login_name']")).SendKeys("elezaar.qaddour");
                driver.FindElement(By.XPath("//*[@id='login_password']")).SendKeys("Sonelgaz.123");
                driver.FindElement(By.XPath("//button[@type='submit']")).Click();

                var task1 = Task.Factory.StartNew(() =>
                {
                    var alarm = false;
                    bool found = false;
                    while (true)
                    {
                        try
                        {
                            try
                            {
                                //
                                found = false;
                                var attribue = driver.FindElement(By.XPath("/html/body/div[2]/div/form[2]/div/table/tbody/tr[1]/td[4]")).Text;
                                Console.WriteLine(attribue);
                                if (attribue.Contains("Attribué"))//
                                {
                                    found = true;
                                }

                                //var array = driver.FindElements(By.XPath("//table[@class='tab_cadrehov']/tbody/tr"));

                                //foreach (var tr in array)
                                //{
                                //    var iClass = tr.FindElement(By.XPath("./td[4]/i")).GetAttribute("title");
                                //    if (iClass.Trim().Equals("Attribué"))
                                //    {
                                //        found = true;
                                //        break;
                                //    }
                                //}
                                //
                            }
                            catch (WebDriverException e)
                            {
                                found = false;
                                alarm = false;
                                //Environment.Exit(0);
                            }

                            if (found)
                            {
                                if (!alarm)
                                {
                                    alarm = true;
                                    Task.Factory.StartNew(() => { while (alarm) player.PlaySync(); player.Stop(); });
                                }
                            }
                            else
                            {
                                alarm = false;
                            }
                        }
                        catch
                        {

                        }

                        Thread.Sleep(1000);
                    }
                });

                Task.WaitAll(new[] { task1 });
            }
            catch (Exception ex)
            {
                Console.WriteLine("*Erreur: " + ex.Message);

                if (driver != null)
                    driver.Close();
            }
        }
    }
}
