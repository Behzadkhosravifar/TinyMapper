namespace Model.Entities
{
    public class Address
    {
        private int _id;

        public string Street { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public int No { get; set; }

        public Address() { }

        public Address(string street, string city, string country, int no)
        {
            _id = this.GetHashCode();

            this.Street = street;
            this.City = city;
            this.Country = country;
            this.No = no;
        }

        public override string ToString()
        {
            return string.Format("Id:{0},  Street:{1},  City:{2},  Country:{3},  No:{4}",
                _id, Street, City, Country, No);
        }
    }
}