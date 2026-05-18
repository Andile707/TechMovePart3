using TechMove.Models;

namespace TechMove.Observers
{
    public class ServiceRequestSubject
    {
        private readonly List<IServiceRequestObserver> _observers = new();

        public void Attach(IServiceRequestObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IServiceRequestObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(ServiceRequestModel serviceRequest)
        {
            foreach (var observer in _observers)
            {
                observer.Update(serviceRequest);
            }
        }
    }
}