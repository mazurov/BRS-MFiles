using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using TreatiesService.Conferences;

namespace TreatiesService
{
    public class ConferencesClient
    {
        private readonly ConferencesEntities _ctx;
        private IDictionary<Guid, Meetings> _meetings;

        public ConferencesClient(string serviceUri)
        {
            ServiceUri = serviceUri;
            _ctx = new ConferencesEntities(new Uri(ServiceUri));
            _ctx.SendingRequest2 += (s, e) =>
            {
                Trace.TraceInformation("SASHA {0} {1}", e.RequestMessage.Method, e.RequestMessage.Url);
            };

        }

        public IDictionary<Guid, Meetings> Meetings
        {
            get
            {
                if (_meetings == null)
                {
                    var skip = 0;
                    var results = new List<IDictionary<Guid, Meetings>>();
                    while (true)
                    {
                        var result = _ctx.Meetings.Skip(skip).ToDictionary(x => x.id, x => x);
                        if (result.Count > 0)
                        {
                            results.Add(result);
                            skip += 250;
                        }
                        else
                        {
                            _meetings = results.SelectMany(dict => dict).ToDictionary(x => x.Key, x => x.Value);
                            break;
                        }
                    }

                }

                return _meetings;
            }
        }

        public string ServiceUri { get; }
       
    }
}
