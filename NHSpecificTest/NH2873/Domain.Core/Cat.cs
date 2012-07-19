using System;

namespace NHibernate.Test.NHSpecificTest.NH2873.Domain.Core
{
    public class Cat
    {
		#region Instance Variables (1) 

        private readonly Person master;

		#endregion Instance Variables 

		#region Public Properties (3) 

        public virtual Guid Id { get; protected set; }

        public virtual Person Master { get { return master; } }

        public virtual string Name { get; set; }

		#endregion Public Properties 

		#region Constructors (2) 

        protected Cat()
        {}

        public Cat(Person master):this()
        {
            this.master = master;
        }

		#endregion Constructors 
    }

}