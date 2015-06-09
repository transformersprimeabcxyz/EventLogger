using Elmah;
using HashTag.Collections;
using HashTag.Diagnostics;
using HashTag.Diagnostics.Models;
using HashTag.Logging.Service.API.Interfaces;
using HashTag.Logging.Service.API.MEX;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HashTag.Diagnostics.Writers;

namespace HashTag.Logging.Web.Library
{
    public class EventRepository : IEventRepository
    {
        
        private static LogEventProcessor _processor = new LogEventProcessor();

        public EventSaveResponse SubmitEventList(List<LogEvent> request)
        {
            var response = new EventSaveResponse();
            var validatedEvents = new List<LogEvent>();
            for (int x = 0; x < request.Count; x++)
            {
                var logEvent = request[x];
                EventSaveItem validationResponse = validateEvent(logEvent);
                validationResponse.Index = x;
                response.Results.Add(validationResponse);
                if (validationResponse.StatusCode == HttpStatusCode.Accepted)
                {
                    validatedEvents.Add(logEvent);
                }
            }

            if (validatedEvents.Count > 0)
            {
                _processor.Submit(validatedEvents);
            }

            response.SubmittedEventCount = response.Results.Count(r => r.StatusCode == HttpStatusCode.Accepted);
            response.IsOk = response.SubmittedEventCount == response.Results.Count;
            return response;
        }

        private EventSaveItem validateEvent(LogEvent evt)
        {
            var retVal = new EventSaveItem();
            retVal.StatusCode = HttpStatusCode.Accepted;

            if (evt.UUID == Guid.Empty)
            {
                evt.UUID = Guid.NewGuid();
            }
            retVal.EventUUID = evt.UUID;

            if (string.IsNullOrWhiteSpace(evt.Application))
            {
                retVal.StatusCode = HttpStatusCode.BadRequest;
                retVal.Message = "Application field is required.  Event not submitted for storage.";
                return retVal;
            }
            if (evt.Application != null && evt.Application.Length >= 100)
            {
                retVal.StatusCode = HttpStatusCode.BadRequest;
                retVal.Message = "Application field must not be greater than 100 characters.  Event not submitted for storage.";
                return retVal;
            }
            if (string.IsNullOrWhiteSpace(evt.Message))
            {
                retVal.StatusCode = HttpStatusCode.BadRequest;
                retVal.Message = "Message field is required.  Event not submitted for storage.";
                return retVal;
            }
            if (evt.Message != null && evt.Message.Length >= 8000)
            {
                retVal.StatusCode = HttpStatusCode.BadRequest;
                retVal.Message = "Message field must be less than 8,000 characters.  Event not submitted for storage.";
                return retVal;
            }
            if (evt.EventDate == default(DateTime))
            {
                retVal.Message = "Sender should set event date before submitting event to service.  Using event service time instead.";
                evt.EventDate = DateTime.Now;
            }

            if (evt.EventType == null || string.IsNullOrWhiteSpace(evt.EventTypeName))
            {
                retVal.StatusCode = HttpStatusCode.BadRequest;
                retVal.Message = "EventType must be one of these values: Critical, Error, Warning, Information, Verbose, Start, Stop, Suspend, Resume, Transfer.  Event not submitted for storage.";
                return retVal;
            }
            if (evt.Properties != null)
            {
                for (int x = 0; x < evt.Properties.Count; x++)
                {
                    var property = evt.Properties[x];
                    if (property.UUID == Guid.Empty)
                    {
                        property.UUID = Guid.NewGuid();
                    }
                    property.EventUUID = evt.UUID;

                    if (string.IsNullOrWhiteSpace(property.Name))
                    {
                        retVal.Message = string.Format("Property[{0}].Name must not be empty.  Event not submitted for storage", x);
                        retVal.StatusCode = HttpStatusCode.BadRequest;
                        return retVal;
                    }
                    if (property.Name != null && property.Name.Length >= 30)
                    {
                        retVal.Message = string.Format("Property[{0}].Name must not be longer than 30 characters.  Event not submitted for storage", x);
                        retVal.StatusCode = HttpStatusCode.BadRequest;
                        return retVal;
                    }
                    if (property.Value != null && property.Value.Length >= 8000)
                    {
                        retVal.Message = string.Format("Property[{0}].Value must not be longer than 8000 characters.  Event not submitted for storage", x);
                        retVal.StatusCode = HttpStatusCode.BadRequest;
                        return retVal;
                    }
                    if (property.Group != null && property.Group.Length >= 20)
                    {
                        retVal.Message = string.Format("Property[{0}].Group must not be longer than 20 characters.  Event not submitted for storage", x);
                        retVal.StatusCode = HttpStatusCode.BadRequest;
                        return retVal;
                    }
                }
            }
            return retVal;
        }

    }
}
