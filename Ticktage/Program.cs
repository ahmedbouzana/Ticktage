using OpenQA.Selenium;
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
            driver.Navigate().GoToUrl("https://support-cc.sdc.dz/");

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

                            var array = driver.FindElements(By.XPath("//table[@class='tab_cadrehov']/tbody/tr"));

                            foreach (var tr in array)
                            {
                                var iClass = tr.FindElement(By.XPath("./td[4]/i")).GetAttribute("title");
                                if (iClass.Trim().Equals("Attribué"))
                                {
                                    found = true;
                                    break;
                                }
                            }
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
    }
}
