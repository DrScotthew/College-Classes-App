using System;
using System.Collections.Generic;

using System.Collections;
using System.Net;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;



namespace WpfApp4
{

    

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        bool clickDown = true;

        public class Item
        {
            public string ItemLine1 { get; set; }
            public string ItemLine2 { get; set; }
        }

        public Window1()
        {
            InitializeComponent();

           
                List<Item> list = new List<Item>();
                StreamReader inFile;
                string inLine;

                if (File.Exists("USIINFO201450.txt"))
                {
                    try
                    {
                        inFile = new StreamReader("USIINFO201450.txt");
                        while ((inLine = inFile.ReadLine()) != null)
                        {
                            if (inLine.IndexOf("<value>") >= 1)
                            {
                                int start = inLine.IndexOf("<value>") + 7;
                                int len = inLine.IndexOf("</value>") - start;
                                Item item = new Item();
                                item.ItemLine1 = inLine.Substring(start, len);

                                inLine = inFile.ReadLine();
                                start = inLine.IndexOf("<value>") + 7;
                                len = inLine.IndexOf("</value>") - start;
                                item.ItemLine2 = inLine.Substring(start, len);

                                list.Add(item);

                        }
                            Console.WriteLine(inLine);
                        }
                    }
                    catch (System.IO.IOException exc)
                    {
                        Console.WriteLine("Error");
                    }
                }

                Dispatcher.BeginInvoke(new Action(() => ListBox1.ItemsSource = list));

                //WebClient wc = new WebClient();
                //wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                //wc.DownloadStringAsync(new Uri("http://www.usi.edu/webservices/iphone/USIINFOterms.xml"));

            
        }

        private void ListBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e != null)
            {
                if (clickDown)
                {
                    IEnumerator ie = e.AddedItems.GetEnumerator();
                    ie.MoveNext();
                    PageTitle.Text = ((Item)ie.Current).ItemLine1;
                    clickDown = true;

                }
                else
                {
                    clickDown = true;
                }
            }
        }
    }
}
