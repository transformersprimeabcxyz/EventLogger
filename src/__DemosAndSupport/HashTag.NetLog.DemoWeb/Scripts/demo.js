function doApiCall(e) {

    e.preventDefault();
    $.ajax({
        url: "/api/demo",
        method: 'GET',
        success: function (data) {
            $('#apiResult').text(data);
        }
    }
   );
}
function doErrorCall(e) {

    e.preventDefault();
    $.ajax({
        url: "/api/demo-with-error",
        method: 'GET',
        success: function (data) {
            $('#apiResult').text(data);
        },
        error: function (d1, d2, d3) {
            $('#errorResult').text(d1.responseJSON.ExceptionMessage);
        }

    }
   );
}
function doError2Call(e) {

    e.preventDefault();
    $.ajax({
        url: "/api/demo-with-error-handled",
        method: 'GET',
        success: function (data) {
            $('#apiResult').text(data);
        },
        error: function (d1, d2, d3) {
            $('#errorResult2').text(d1.responseText);
        }

    }
   );
}