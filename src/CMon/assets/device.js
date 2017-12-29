var request = require("superagent");

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
		if (res.status === 200) $(".status").text(res.text);
		timeoutId = setTimeout(function() { requestStatus(); }, 3000);
	});
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
	if (deviceId) {
		// requestStatus();

		/*$("#time-range").on("apply", function (e, range) {
			requestEvents(range);
		});*/
	}
});
