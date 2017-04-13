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
		$(".status").text(res.text);
		timeoutId = setTimeout(function() { requestStatus(); }, 3000);
	});
}

$(".btn-refresh").click(function () {
	post("/api/device/refresh");
	return false;
});

$(function () {
	requestStatus();
});
