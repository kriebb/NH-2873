using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Test.NHSpecificTest.NH2873.Domain.Core;
using NHibernate.Test.NHSpecificTest.NH2873.Domain.Shared.Items;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2873.Domain.Tests
{
    [TestFixture]
    public class WeirdBehaviour : BugTestCase
    {
        #region Instance Variables (1) 

        private Guid nicePersonId;

        #endregion Instance Variables 

        #region Public Methods (6) 

        [Test]
        public void AddACat()
        {
            Person person;
            using (var session = OpenSession())
            {
                person = session.Get<Person>(nicePersonId);
                person.AddCat(new Cat(person){Name = "Ducky"});
                session.Flush();
                session.Clear();
            }
            using (var session = OpenSession())
            {
                person = session.Get<Person>(nicePersonId);
                Assert.AreEqual(1, person.Cats.Count());                
            }            
        }

        [Test]
        public void AddAChild()
        {
            Person john;            
            Child johnJunior;
            using (var session = OpenSession())
            {
                john = session.Get<Person>(nicePersonId);
                Assert.IsNull(john.Partner);
                johnJunior = new Child("John Junior",john) {NickName = "John's FirstBorn"};                
                john.AddChild(johnJunior);                
                session.Flush();
                session.Clear();
            }
            using (var session = OpenSession())
            {
                john = session.Get<Person>(john.Id);
                Assert.IsTrue(john.Children.Where(x => x.Id == johnJunior.Id).Count() > 0);
            }
        }

        [Test]
        public void AddAPartner()
        {
            Person john;            
            using (var session = OpenSession())
            {
                john = session.Get<Person>(nicePersonId);
                Assert.IsNull(john.Partner);
                var jane = new Person("Jane") {  NickName = "John's Wife" };
                session.Save(jane);
                john.Partner = jane;
                session.Flush();
                session.Clear();

                john = session.Get<Person>(nicePersonId);
                Assert.IsNotNull(john.Partner);
                Assert.AreEqual("Jane", john.Partner.Name);

            }
            using (var session = OpenSession())
            {
                john = session.Get<Person>(nicePersonId);
                Assert.IsNotNull(john.Partner);
                Assert.AreEqual("Jane", john.Partner.Name);
            }
        }
        [Test]
        public void ChangeTheNameOfTheCatFromSmellyToUgly()
        {
            Cat smellyCat;
            using (var session = OpenSession())
            {
                var john = session.Get<Person>(nicePersonId);
                john.AddCat(new Cat(john) { Name = "Whisky" });

                var johnJunior = new Child("John Junior",john) { NickName = "John's FirstBorn" };
                john.AddChild(johnJunior);                                

                smellyCat = new Cat(johnJunior) {Name = "Smelly Cat"};
                johnJunior.AddCat(smellyCat);
                
                session.Flush();
                session.Clear();
            }
            
            using (var session = OpenSession())
            {
                smellyCat = session.Get<Cat>(smellyCat.Id);
                smellyCat.Name = "Ugly cat";
                session.Flush();               //if you change the PreUpdateEventListenerImpl to Log.Info on the Master.Name you get an exception                 
            }
        }

        [Test]
        public void ChangeTheNameOfTheNewtoyToBoringToy()
        {
            Toy newToy;
            using (var session = OpenSession())
            {
                var john = session.Get<Person>(nicePersonId);
                var johnJunior = new Child("John Junior", john) { NickName = "John's FirstBorn" };
                john.AddChild(johnJunior);

                newToy = new Toy(johnJunior) { Name = "New Toy Of The Day" };
                johnJunior.AddToy(newToy);                
                
                session.Flush();
                session.Clear();
            }

            using (var session = OpenSession())
            {
                newToy = session.Get<Toy>(newToy.Id);
                newToy.Name = "Boring toy";
                session.Flush();
            }
        }

        [Test]
        public void GiveANickName()
        {
            Person person;
            using (var session = OpenSession())
            {
                person = session.Get<Person>(nicePersonId);                
                person.NickName = "Joey";
                session.Flush();
                session.Clear();
            }
            using (var session = OpenSession())
            {
                person = session.Get<Person>(nicePersonId);
                Assert.AreEqual("Joey", person.NickName);

            }            
        }

        [Test]
        public void RetrieveACat()
        {
            using (ISession session = OpenSession())
            {
                var person = session.Get<Person>(nicePersonId);
                person.AddCat(new Cat(person));
                Assert.AreEqual(1, person.Cats.Count());
            }
            using (ISession session = OpenSession())
            {
                var person = session.Get<Person>(nicePersonId);                                
                Assert.AreEqual(1,person.Cats.Count());
            }
        }

        [Test]
        public void CanAddATodo()
        {
            using (ISession session = OpenSession())
            {
                var person = session.Get<Person>(nicePersonId);
                person.AddTodo(new Todo {Name = "I need to get it"});
                Assert.AreEqual(1, person.Todos.Count());
                session.Flush();
            }
            using (ISession session = OpenSession())
            {
                var person = session.Get<Person>(nicePersonId);
                Assert.AreEqual(1, person.Todos.Count());
            }   
        }

        [Test]
        public void CanICountACompositeElement()
        {
            using (ISession session = OpenSession())
            {
                var personCriteria = DetachedCriteria.For<Person>();

                var personProjections = Projections.ProjectionList();
                var todoCriteria = personCriteria.CreateCriteria("Todos");
                todoCriteria.SetProjection(Projections.Count("Name"));

                personCriteria.SetProjection(personProjections.Add(Projections.Property("Name"), "Name"));



                personCriteria.SetResultTransformer(Transformers.AliasToBean(typeof(PersonItem)));
                IList<PersonItem> projectedItems = personCriteria.GetExecutableCriteria(session).List<PersonItem>();
                Assert.AreEqual(1, projectedItems[0].NumberOfTodos);
            }
            using (ISession session = OpenSession())
            {
                var person = session.Get<Person>(nicePersonId);
                Assert.AreEqual(1, person.Todos.Count());
            }
        }

        #endregion Public Methods 

        #region Protected Methods (2) 

        protected override void OnSetUp()
        {
            using (ISession session = OpenSession())
            {
                var nicePerson = new Person("John");                
                session.Save(nicePerson);
                
                session.Flush();
                nicePersonId = nicePerson.Id;
                session.Clear();
            } 
            base.OnSetUp();
        }

        protected override void OnTearDown()
        {
            base.OnTearDown();
            using (ISession session = OpenSession())
            {
                const string hql = "from System.Object";
                session.Delete(hql);
                session.Flush();
            }
        }

        #endregion Protected Methods 
    }
}