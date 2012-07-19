using System;

namespace NHibernate.Test.NHSpecificTest.NH2873.Domain.Core
{
    public class Toy
    {
		#region Instance Variables (1) 

        private readonly Child owner;

		#endregion Instance Variables 

		#region Public Properties (3) 

        public virtual Guid Id { get; protected set; }

        public virtual Child Owner { get { return owner; } }

        public virtual string Name { get; set; }

		#endregion Public Properties 

		#region Constructors (2) 

        protected Toy()
        {}

        public Toy(Child owner):this()
        {
            this.owner = owner;
        }

		#endregion Constructors 
    }

    public class Todo
    {
        public virtual string Name { get; set; }
    }


}