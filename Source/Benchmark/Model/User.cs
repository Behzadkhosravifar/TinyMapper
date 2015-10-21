namespace Model.Entities
{
    public class User
    {
        private int _id;

        public string Name { get; set; }

        public string LastName { get; set; }

        public Address Address { get; set; }

        public User() { }

        public User(string name, string lastname, Address address)
        {
            _id = this.GetHashCode();

            Name = name;

            LastName = lastname;

            Address = address;
        }

        public override string ToString()
        {
            return string.Format("Id:{0},  Name:{1}, LastName:{2}, Address:{3}",
                _id, Name, LastName, Address);
        }
    }
}
