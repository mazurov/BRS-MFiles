using System;
using TreatiesService.Conferences;

namespace TreatiesService
{
    class ConferencesClient
    {
        private ConferencesEntities _ctx;
        public ConferencesClient(string serviceUri)
        {
            _ctx = new ConferencesEntities(new Uri(serviceUri));
        }
    }
}
