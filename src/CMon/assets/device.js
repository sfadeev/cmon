var request = require("superagent");

$(".device-list button").click(function () {

	request.post("/api/device/refresh")
		.send({ deviceId: 1 })
		.set("X-XSRF-TOKEN", document.querySelector("input[name=__RequestVerificationToken]").value)
		.end(function (err, res) {
			console.log(arguments);
		});

	return false;

});
