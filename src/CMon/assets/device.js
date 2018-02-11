var request = require("superagent");
var signalr = require("@aspnet/signalr-client");

var deviceId = $(".dashboard-content").data("deviceId"),
	xsrfToken = document.querySelector("input[name=__RequestVerificationToken]").value;

var timeoutId;

var post = function(url, handler) {
	request.post(url)
        .query({ deviceId: deviceId })
		.set("X-XSRF-TOKEN", xsrfToken)
		.end(handler);
}

var requestStatus = function () {

	post("/api/device/status", function(err, res) {
		if (res.status === 200) displayStatus(res.text);
		timeoutId = setTimeout(function() { requestStatus(); }, 3000);
	});
}

var displayStatus = function(status) {
	$(".status").text(status);
}

var requestEvents = function (range) {

	post("/api/device/events?from=" + range.from + "&to=" + range.to + "&t=" + new Date().getTime(),
		function(err, res) {
			console.log(res.body.items);
			if (range.to === "now") {
				setTimeout(function () { requestEvents(range); }, 15000);
			}
		});
}

$(".btn-refresh").click(function () {
	post("/api/device/refresh");
	return false;
});

$(function () {
	let connection = new signalr.HubConnection("/dashboard");

	connection.on("Log", (deviceId, message) => {
		// console.log(message);
		$("#log").text(message);
	});
	
	connection.on("StatusUpdated", (deviceId, status) => {
		// console.log("StatusUpdated", deviceId, status);
		// displayStatus(status);

		window.dispatchEvent(new CustomEvent("status-updated", { detail: { deviceId: deviceId, status: status } } ));
	});
	
	connection.start().catch(err => {
		console.error(err);
	});

	if (deviceId) {
		// requestStatus();

		/*$("#time-range").on("apply", function (e, range) {
			requestEvents(range);
		});*/
	}
});
