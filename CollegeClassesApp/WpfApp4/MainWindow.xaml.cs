//5-1-22
//This program creates a USI app for all the semesters 2013-2014 and displays them for the user to click(select).  Once the user selects a semester, the screen shows the available course departments which the user can
//then select.  After clicking a class, the class details for that specific class section is displayed including the professor's name, full name of class, meet times, and where the class meets (either on the WEB or in-person).

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
using System.Xml.Linq;
using System.IO;
using WpfApp4.DataModels;


namespace WpfApp4
{
   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Object _lastSelected;
        private USITerm _selectedTerm;
        private USIDepartment _selectedDepartment;
        private USICourse _selectedCourse;

        private List<USITerm> _terms;
        bool clickDown = true;

        public MainWindow()
        {
            InitializeComponent();
            ClassDetailsPanel.Visibility = Visibility.Hidden;

            List<Item> list = new List<Item>();
            var termInfoFile = Path.Combine(Environment.CurrentDirectory, @"Data\", "USIINFOterms.xml");
            StreamReader inFile;
            string inLine;

            var xd = GetXMLDocument(termInfoFile);
            _terms = GetTermsFromXML(xd);

            foreach (var term in _terms)
            {
                term.Courses = GetCourses(term.TermId);
                list.Add(term.GetTermItem());
            }


            PageTitle.Text = "Terms";
            Dispatcher.BeginInvoke(new Action(() => ListBox1.ItemsSource = list));  //goes to next 'screen' to show list values

        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            PageTitle.Text = e.Result;
        }


        //triggers once user clicks on something in program
        private void ListBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e != null)
            {
                if (e.AddedItems.Count == 0)
                    return;

                var itemClicked = ((Item)e.AddedItems[0]).DataItem;
                _lastSelected = itemClicked;

                //triggers if user clicks on term from first page
                if (itemClicked is USITerm)
                {
                    var clickedTerm = (USITerm)itemClicked;
                    _selectedTerm = clickedTerm;
                    var departmentsList = clickedTerm.GetDepartmentItems();
                    PageTitle.Text = "Fields";
                    Dispatcher.BeginInvoke(new Action(() => ListBox1.ItemsSource = departmentsList));
                    ClassDetailsPanel.Visibility = Visibility.Hidden;
                    ListBox1.Visibility = Visibility.Visible;
                }

                //triggers if user clicks on a class department from second page
                if (itemClicked is USIDepartment)
                {
                    var clickedDept = (USIDepartment)itemClicked;
                    _selectedDepartment = clickedDept;
                    var coursesList = _selectedTerm.GetCourseItems(clickedDept.Abbreviation);
                    PageTitle.Text = "Classes";
                    Dispatcher.BeginInvoke(new Action(() => ListBox1.ItemsSource = coursesList));
                    ClassDetailsPanel.Visibility = Visibility.Hidden;
                    ListBox1.Visibility = Visibility.Visible;
                }

                //triggers if user clicks on a specific course from third page
                if (itemClicked is USICourse)
                {
                    var clickedCourse = (USICourse)itemClicked;
                    _selectedCourse = clickedCourse;
                    var courseDetails = new USICourseDetail(clickedCourse);
                    _lastSelected = courseDetails;
                    PageTitle.Text = "Class Details";
                    Dispatcher.BeginInvoke(new Action(() => ClassDetailsPanel.DataContext = courseDetails));
                    Dispatcher.BeginInvoke(new Action(() => ListBox1.ItemsSource = new List<Item>()));
                    ClassDetailsPanel.Visibility = Visibility.Visible;
                    ListBox1.Visibility = Visibility.Hidden;
                }

                e.Handled = true;
            }
        }

        //ignore below code as it isn't used in this program
        private void BackButton_Clicked(object sender, RoutedEventArgs e)
        {

            var x = _lastSelected.GetType();
            var y = "";
            //User clicked back button while showing terms list... nothing to go "back" to so do nothing.
            if (_lastSelected is USITerm)
            {
                return;
            }

            //User clicked back button while showing departments list... go back to showing terms
            if (_lastSelected is USIDepartment)
            {
                var termsList = new List<Item>();
                foreach (var term in _terms)
                {
                    termsList.Add(term.GetTermItem());
                }

                _lastSelected = _selectedTerm;
                PageTitle.Text = "Terms";
                Dispatcher.BeginInvoke(new Action(() => ListBox1.ItemsSource = termsList));
                ClassDetailsPanel.Visibility = Visibility.Hidden;
                ListBox1.Visibility = Visibility.Visible;
                return;
            }

            //User clicked back button while showing course list... go back to showing departments (aka fields)
            if (_lastSelected is USICourse)
            {
                _lastSelected = _selectedDepartment;
                var departmentsList = _selectedTerm.GetDepartmentItems();
                PageTitle.Text = "Fields";
                Dispatcher.BeginInvoke(new Action(() => ListBox1.ItemsSource = departmentsList));
                ClassDetailsPanel.Visibility = Visibility.Hidden;
                ListBox1.Visibility = Visibility.Visible;
                return;
            }

            //User clicked back button while showing course details... go back to showing courses
            if (_lastSelected is USICourseDetail)
            {
                _lastSelected = _selectedCourse;
                var coursesList = _selectedTerm.GetCourseItems(_selectedDepartment.Abbreviation);
                PageTitle.Text = "Classes";
                Dispatcher.BeginInvoke(new Action(() => ListBox1.ItemsSource = coursesList));
                ClassDetailsPanel.Visibility = Visibility.Hidden;
                ListBox1.Visibility = Visibility.Visible;
                return;
            }
        }

        /// <summary>
        /// This function reads an xml file and fixes the corrupted XML at the beginning so the XMl parser can actually work
        /// </summary>
        /// <param name="filePath">Path to the XML file</param>
        /// <returns></returns>
        public XDocument GetXMLDocument(string filePath)
        {

            //read file text
            var fileData = File.ReadAllText(filePath);

            //find start point of bad xml
            var startPoint = fileData.IndexOf("<dataset");

            //find ending point of bad xml (everything up to start of metadata tag)
            var endPoint = fileData.IndexOf("<metadata>");

            var badChunk = fileData.Substring(startPoint, endPoint - startPoint);

            //replace bad xml chunk with good chunk
            var newXML = fileData.Replace(badChunk, "<dataset>");

            //create xdoc object using corrected data string
            var xdoc = XDocument.Parse(newXML);

            //return the xdoc
            return xdoc;
        }

        //gets elements from xml document to use in program easily
        public List<USITerm> GetTermsFromXML(XDocument xdoc)
        {
            var terms = new List<USITerm>();

            var datasetElement = xdoc.Element("dataset");
            var dataElement = datasetElement.Element("data");

            var dataRows = dataElement.Elements("row");
            foreach (var dataRow in dataRows)
            {
                var rowValues = dataRow.Elements("value");
                var termName = rowValues.ElementAt(0).Value;
                var termNumber = Convert.ToInt32(rowValues.ElementAt(1).Value);

                var term = new USITerm()
                {
                    TermId = termNumber,
                    TermName = termName
                };

                terms.Add(term);
            }

            return terms;
        }

        //gets list of courses from xml documents
        public List<USICourse> GetCourses(int termId)
        {

            var courses = new List<USICourse>();

            var rootPath = Path.Combine(Environment.CurrentDirectory, @"Data");
            var filePath = $"{rootPath}\\USIINFO{termId}.xml";
            var xdoc = GetXMLDocument(filePath);

            var datasetElement = xdoc.Element("dataset");
            var dataElement = datasetElement.Element("data");

            var dataRows = dataElement.Elements("row");
            foreach (var dataRow in dataRows)
            {
                courses.Add(new USICourse(dataRow));
            }

            return courses.Distinct().ToList();

        }

        
    }


}
