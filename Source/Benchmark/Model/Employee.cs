namespace Model.Entities
{
    public class Employee
    {
        private int _id;

        public string Name { get; set; }

        public string LastName { get; set; }

        public Address Address { get; set; }

        public Employee() { }

        public override string ToString()
        {
            return string.Format("Id:{0},  Name:{1}, LastName:{2}, Address:{3}",
                _id, Name, LastName, Address);
        }
    }
}
