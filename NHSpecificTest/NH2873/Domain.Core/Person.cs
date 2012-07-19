using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2873.Domain.Core
{
    public class Person
    {
		#region Instance Variables (4) 

        private readonly ISet<Cat> cats;
        private readonly ISet<Child> children;
        private readonly ISet<Todo> todos;
        private readonly ISet<Toy> toys;

		#endregion Instance Variables 

		#region Public Properties (8) 

        public virtual IEnumerable<Cat> Cats
        {
            get { return cats; }
        }

        public virtual IEnumerable<Child> Children
        {
            get { return children; }
        }

        public virtual Guid Id { get; protected set; }


        public virtual string Name { get; set; }

        public virtual string NickName { get; set; }

        public virtual Person Partner { get; set; }

        public virtual IEnumerable<Todo> Todos { get { return todos; } }

        public virtual IEnumerable<Toy> Toys
        {
            get { return toys; }
        }

		#endregion Public Properties 

		#region Constructors (2) 

        protected Person()
        {
            cats = new HashedSet<Cat>();
            children = new HashedSet<Child>();
            toys = new HashedSet<Toy>();
            todos = new HashedSet<Todo>();
        }

        public Person(string name):this()
        {
            Name = name;
        }

		#endregion Constructors 

		#region Public Methods (4) 

        public virtual void AddCat(Cat cat)
        {
            cats.Add(cat);
        }

        public virtual void AddChild(Child child)
        {
            children.Add(child);
            if(Partner != null)
                Partner.AddChild(child);
        }

        public virtual void AddTodo(Todo todo)
        {
            todos.Add(todo);
        }

        public virtual void AddToy(Toy newToy)
        {
            toys.Add(newToy);
        }

		#endregion Public Methods 
    }
}