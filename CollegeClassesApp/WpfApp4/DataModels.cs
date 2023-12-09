using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WpfApp4.DataModels
{
    public class Item
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public Object DataItem { get; set; }
    }

    public class USITerm
    {
        public int TermId { get; set; }
        public string TermName{ get; set; }

        public List<USICourse> Courses { get; set; }
        public List<USIDepartment> Departments => GetDepartmentList();

        private List<USIDepartment> GetDepartmentList()
        {
            return Courses.Select(s => new USIDepartment() {
                        Abbreviation = s.Subj,
                        Name = s.SubjectDesc
                    })
                    .Distinct()
                    .ToList();
        }

        public Item GetTermItem()
        {
            var item = new Item();
            item.Title = TermName;
            item.Subtitle = "Classes";
            item.DataItem = this;
            return item;
        }

        public List<Item> GetDepartmentItems()
        {
            var departmentItems = new List<Item>();
            foreach (var department in Departments)
            {
                departmentItems.Add(department.GetDepartmentItem());
            }
            return departmentItems; 
        }

        public List<Item> GetCourseItems()
        {
            var courseItems = new List<Item>();
            foreach (var course in Courses)
            {
                courseItems.Add(course.GetCourseItem());
            }
            return courseItems;
        }

        public List<Item> GetCourseItems(string abbrev)
        {
            var courseItems = new List<Item>();
            foreach (var course in Courses.Where(c=>c.Subj == abbrev))
            {
                courseItems.Add(course.GetCourseItem());
            }
            return courseItems;
        }

    }

    public class USIDepartment : IEquatable<USIDepartment>
    {
        public string Abbreviation { get; set; }
        public string Name { get; set; }

        public bool Equals(USIDepartment other)
        {
            return this.Abbreviation == other.Abbreviation;
        }

        public override int GetHashCode()
        {
            return this.Abbreviation.GetHashCode();
        }

        public Item GetDepartmentItem()
        {
            var item = new Item();
            item.Title = Name;
            item.Subtitle = Abbreviation;
            item.DataItem = this;
            return item;
        }
    }

    public class USICourseDetail
    {
        private USICourse _course;
        public string CourseNumber => $"{_course.Subj}{_course.Numb}.{_course.Section}";
        public string Title => _course.Title;
        public string Instructor => $"{_course.InstructorLname}, {_course.InstructorFname}";
        public string Meeting => $"{_course.Mon} {_course.Tue} {_course.Wed} {_course.Thur} {_course.Fri} {_course.Sat} {_course.Sun}";
        public string Room => $"{_course.Campus} {_course.Building} {_course.Room}";
        public string Meets => $"{_course.StartDate} {_course.EndDate}";

        public USICourseDetail(USICourse course)
        {
            _course = course;
        }

    }

    public class USICourse : IEquatable<USICourse>
    {
        public string Term { get; set; }
        public string Subj { get; set; }
        public string SubjectDesc { get; set; }
        public string Numb { get; set; }
        public string Section { get; set; }
        public string Title { get; set; }
        public string InstructorFname { get; set; }
        public string InstructorLname { get; set; }
        public string Crn { get; set; }
        public string Campus { get; set; }
        public string Building { get; set; }
        public string Room { get; set; }
        public string Mon { get; set; }
        public string Tue { get; set; }
        public string Wed { get; set; }
        public string Thur { get; set; }
        public string Fri { get; set; }
        public string Sat { get; set; }
        public string Sun { get; set; }
        public string BeginTime { get; set; }
        public string EndTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public USICourse(XElement xmlData)
        {
            var rowValues = xmlData.Elements("value");

            Term = rowValues.ElementAt(0).Value;
            Subj = rowValues.ElementAt(1).Value;
            SubjectDesc = rowValues.ElementAt(2).Value;
            Numb = rowValues.ElementAt(3).Value;
            Section = rowValues.ElementAt(4).Value;
            Title = rowValues.ElementAt(5).Value;
            InstructorFname = rowValues.ElementAt(6).Value;
            InstructorLname = rowValues.ElementAt(7).Value;
            Crn = rowValues.ElementAt(8).Value;
            Campus = rowValues.ElementAt(9).Value;
            Building = rowValues.ElementAt(10).Value;
            Room = rowValues.ElementAt(11).Value;
            Mon = rowValues.ElementAt(12).Value;
            Tue = rowValues.ElementAt(13).Value;
            Wed = rowValues.ElementAt(14).Value;
            Thur = rowValues.ElementAt(15).Value;
            Fri = rowValues.ElementAt(16).Value;
            Sat = rowValues.ElementAt(17).Value;
            Sun = rowValues.ElementAt(18).Value;
            BeginTime = rowValues.ElementAt(19).Value;
            EndTime = rowValues.ElementAt(20).Value;
            StartDate = DateTime.Parse(rowValues.ElementAt(21).Value);
            EndDate = DateTime.Parse(rowValues.ElementAt(22).Value);

        }

        public bool Equals(USICourse other)
        {
            return this.Crn == other.Crn;
        }

        public override int GetHashCode()
        {
            return this.Crn.GetHashCode();
        }

        public Item GetCourseItem()
        {
            var item = new Item();
            item.Title = Title;
            item.Subtitle = InstructorLname;
            item.DataItem = this;
            return item;
        }
    }
}
