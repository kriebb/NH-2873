using System;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using NHibernate.Test.NHSpecificTest.NH2873.Domain.Core;
using log4net;

namespace NHibernate.Test.NHSpecificTest.NH2873.Domain.Services
{
    public class PreUpdateEventListenerImpl: IPreUpdateEventListener
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PreUpdateEventListenerImpl));
        private void SetState(IEntityPersister persister,
        object[] state, string propertyName, object value)
        {
            var index = GetIndex(persister, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }
        private int GetIndex(IEntityPersister persister, string propertyName)
        {
            return Array.IndexOf(persister.PropertyNames,
            propertyName);
        }
        public bool OnPreUpdate(PreUpdateEvent @event)
        {            
            if (@event.Entity == null || @event.Session.EntityMode != EntityMode.Poco) return false;

            /*simple validation*/
            if (@event.Entity as Person != null)
            {
                var person = (Person) @event.Entity;
                if (person.Partner != null && person.Partner.Id != Guid.Empty)
                    Log.Info("Person does not have a partner");
            }
            if (@event.Entity as Cat != null)
            {
                var cat = (Cat)@event.Entity;
                {                    
                    Log.Info(string.Format("Cat {0} has Master Id {1}", cat.Name, cat.Master.Id));                    
                }
            }
            if (@event.Entity as Toy != null)
            {
                var toy = (Toy)@event.Entity;
                {
                    Child xChild;
                    using(var newSession = @event.Session.SessionFactory.OpenSession())                    
                    {                                                
                         xChild = newSession.Get<Child>(toy.Owner.Id);
                    }
                    Log.Info(string.Format("Toy {0} has Owner {1}", toy.Name,xChild.Name));
                }

            }
            return false; 
        }
    }
}