detailsPropertiesVm = function () {
    group = "";
    name = "";
    value = "";
}
detailsVm = function (divNameToApplyBindingsTo, eventId) {
    var self = this;
    self.divId = divNameToApplyBindingsTo;
    self.eventId = eventId;
    self.properties = ko.observableArray([]);
    self.header = ko.observable(new eventRowVm());

    self.onReady = function () {
        ko.applyBindings(this, $("#" + self.divId).get(0));

        $.ajax(
          {
              url: "/events/0/0/O/4("+self.eventId+")?$expand=Properties",
              type: 'GET',
              contentType: "application/json",
              success: function (result, status, xhr) {

                  if (result.value && result.value.length > 0) {
                      self.mapToGrid(result.value[0]);
                  }
              },              
          }
          );
    }

    self.mapToGrid = function (response) {
        self.properties([]);

        var x = new eventRowVm();
        x.eventDate = response.EventDate;
        x.eventTypeName = response.EventTypeName;
        x.eventApplication = response.Application;
        x.eventHost = response.Host;
        x.eventUser = response.User;
        x.eventSource = response.EventSource;
        x.eventUUID = response.UUID;
        x.eventMessage = response.Message;
        x.detailsLink = "/Event/" + response.UUID;
        self.header(x);

        for (propertyIndex = 0; propertyIndex < response.Properties.length; propertyIndex++) {
            var dbProperty = response.Properties[propertyIndex];
            var detail = new detailsPropertiesVm();

            detail.group = dbProperty.Group;
            detail.name = dbProperty.Name;
            detail.value = dbProperty.Value;

            self.properties.push(detail);
        }

    }
}