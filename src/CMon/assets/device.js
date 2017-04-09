var request = require("superagent");

var timeoutId;

var requestStatus = function(deviceId) {
	request.post("/api/device/status")
        .query({ deviceId: deviceId })
		.set("X-XSRF-TOKEN", document.querySelector("input[name=__RequestVerificationToken]").value)
		.end(function(err, res) {
			$(".status").text(res.text);

            timeoutId = setTimeout(function() { requestStatus(deviceId); }, 3000);
		});
}

$(".device-list button.btn-refresh").click(function () {

	var deviceId = $(this).data("deviceId");

	request.post("/api/device/refresh")
		.query({ deviceId: deviceId })
		.set("X-XSRF-TOKEN", document.querySelector("input[name=__RequestVerificationToken]").value)
		.end(function (err, res) {
			console.log(arguments);
		});

    requestStatus(deviceId);

	return false;

});

$(".device-list button.btn-status").click(function () {
	
	if (timeoutId) {
		clearTimeout(timeoutId);
		timeoutId = null;
    }

	return false;

});
