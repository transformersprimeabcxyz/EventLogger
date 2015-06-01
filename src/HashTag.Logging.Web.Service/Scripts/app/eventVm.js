function eventRowVm() {

    var self = this;
    self.eventDate = "";
    self.eventTypeName = "";
    self.eventApplication = "";
    self.eventHost = "";
    self.eventUser = "";
    self.eventSource = "";
    self.eventMessage = "";
    self.eventUUID = "";
    self.detailsLink = "";
}
function eventVm(divIdForKnockoutBinding) {
    var self = this;
    self.eventRows = ko.observableArray([]);
    self.divName = divIdForKnockoutBinding;

    self.onReady = function () {
        ko.applyBindings(self, $('#' + self.divName).get(0));

        $.ajax(
            {
                url:"events/0/0/O/4?$orderby=EventDate desc", 
                type: 'GET',
                contentType: "application/json",
                success: function (result, status, xhr) {
                    self.mapToGrid(result);
                }
            }
            );
      
    }
    self.mapToGrid = function(response)
    {
        self.eventRows([]);
        for(itemIndex=0;itemIndex<response.value.length;itemIndex++)
        {
            var srcItem = response.value[itemIndex];
            var x = new eventRowVm();

            x.eventDate = srcItem.EventDate;
            
            x.eventTypeName = srcItem.EventTypeName;
            x.eventApplication = srcItem.Application;
            x.eventHost = srcItem.Host;
            x.eventUser = srcItem.User;
            x.eventSource = srcItem.EventSource;
            x.eventUUID = srcItem.UUID;
            x.eventMessage = srcItem.Message;
            x.detailsLink = "/Event/" + srcItem.UUID;
            self.eventRows.push(x);
        }

    }
}