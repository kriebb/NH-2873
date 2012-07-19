namespace NHibernate.Test.NHSpecificTest.NH2873.Domain.Core
{
    public class Child:Person
    {
        private Person parent;
        public Child(string childName,Person person):base(childName)
        {
            parent = person;
        }
        protected Child():base()
        {
            
        }

        

        public virtual Person Parent
        {
            get { return parent; }
            protected set { parent = value; }
        }


    }
}