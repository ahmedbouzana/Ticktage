using OpenQA.Selenium.Chrome;
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
        static ChromeDriver driver;
        static System.Media.SoundPlayer player;

        static void Main(string[] args)
        {
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

            driver = new ChromeDriver(serverPath);

            driver.Navigate().GoToUrl("google.com");

            var task1 = Task.Factory.StartNew(() => {
                var alarm = false;
                while (true)
                {
                    try
                    {
                        //
                        var check = driver.FindElementByXPath("//*[@id='header']/div[1]/a[2]");
                        //
                        if (true)
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
